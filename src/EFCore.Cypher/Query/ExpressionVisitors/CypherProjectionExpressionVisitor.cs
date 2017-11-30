// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Clauses.StreamedData;

namespace Microsoft.EntityFrameworkCore.Query.ExpressionVisitors
{
    public class CypherProjectionExpressionVisitor: ProjectionExpressionVisitor {

        private readonly ICypherTranslatingExpressionVisitorFactory _cypherTranslatingExpressionVisitorFactory;

        private readonly IEntityMaterializerSource _entityMaterializerSource;

        private readonly IQuerySource _querySource;

        private bool _topLevelReturn;

        private readonly Dictionary<Expression, Expression> _sourceExpressionReturnMapping = new Dictionary<Expression, Expression>();

        public CypherProjectionExpressionVisitor(
            [NotNull] CypherProjectionExpressionVisitorDependencies dependencies,
            [NotNull] CypherQueryModelVisitor queryModelVisitor,
            [NotNull] IQuerySource querySource
        ): base (Check.NotNull(queryModelVisitor, nameof(queryModelVisitor))) {
            Check.NotNull(dependencies, nameof(dependencies));
            Check.NotNull(querySource, nameof(querySource));

            _cypherTranslatingExpressionVisitorFactory = dependencies.CypherTranslatingExpressionVisitorFactory;
            _entityMaterializerSource = dependencies.EntityMaterializerSource;
            _querySource = querySource;
            _topLevelReturn = true;
        }

        /// <summary>
        /// Concrete query model visitor
        /// </summary>
        /// <returns></returns>
        private new CypherQueryModelVisitor QueryModelVisitor
            => (CypherQueryModelVisitor)base.QueryModelVisitor;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="memberInitExpression"></param>
        /// <returns></returns>
        protected override Expression VisitMemberInit(MemberInitExpression memberInitExpression)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodCallExpression"></param>
        /// <returns></returns>
        protected override Expression VisitMethodCall(MethodCallExpression methodCallExpression)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newExpression"></param>
        /// <returns></returns>
        protected override Expression VisitNew(NewExpression newExpression)
        {
            Check.NotNull(newExpression, nameof(newExpression));

            if (newExpression.Type == typeof(AnonymousObject))
            {
            }

            var newNewExpression = base.VisitNew(newExpression);
            var readOnlyExpression = QueryModelVisitor.TryGetQuery(_querySource);

            if (readOnlyExpression != null)
            {
                for (var index = 0; index < newExpression.Arguments.Count; index++) {
                    var sourceExpression = newExpression.Arguments[index];

                    if (_sourceExpressionReturnMapping.TryGetValue(sourceExpression, out var cypherExpression)) {
                        var mi = newExpression?.Members?[index];

                        if (!(mi is null)) {
                            readOnlyExpression.SetReturnForMemberInfo(
                                mi,
                                cypherExpression
                            );
                        }
                    }
                }
            }

            return newNewExpression;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public override Expression Visit(Expression expression)
        {
            // TODO: Group by method
            if (_topLevelReturn) {
                
            }

            _topLevelReturn = false;

            var readOnlyExpression = QueryModelVisitor.TryGetQuery(_querySource);

            if (expression != null
                && !(expression is ConstantExpression)
                && !(expression is NewExpression)
                && !(expression is MemberInitExpression)
                && readOnlyExpression != null)
            {
                var cypherExpression
                    = _cypherTranslatingExpressionVisitorFactory
                        .Create(
                            QueryModelVisitor, 
                            readOnlyExpression, 
                            inReturn: true
                        )
                        .Visit(expression);

                if (cypherExpression is null) {
                    if (expression is QuerySourceReferenceExpression qsre) {
                        // TODO: Handle query source 
                    } else {
                        // TODO: Client projection
                    }
                } else {
                    readOnlyExpression.RemoveRangeFromReturn(
                        readOnlyExpression.ReturnItems.Count
                    );

                    if (!(expression is QuerySourceReferenceExpression)) {
                        if (cypherExpression is NullableExpression nullableExpression) {
                            cypherExpression = nullableExpression.Operand;
                        }

                        if (cypherExpression is StorageExpression) {
                            var index = readOnlyExpression.AddReturnItem(cypherExpression);
                            _sourceExpressionReturnMapping[expression] = readOnlyExpression.ReturnItems[index];

                            return expression;
                        }
                    }

                    if (!(cypherExpression is ConstantExpression))
                    {
                        var targetExpression = QueryModelVisitor
                                .QueryCompilationContext
                                .QuerySourceMapping
                                .GetExpression(_querySource);

                        if (targetExpression.Type == typeof(ValueBuffer))
                        {
                            var index = readOnlyExpression.AddReturnItem(cypherExpression);
                            _sourceExpressionReturnMapping[expression] = readOnlyExpression.ReturnItems[index];

                            var readValueExpression = _entityMaterializerSource
                                .CreateReadValueCallExpression(
                                    targetExpression, 
                                    index
                                );
                            
                            var outputDataInfo = (expression as SubQueryExpression)?
                                .QueryModel
                                .GetOutputDataInfo();

                            if (outputDataInfo is StreamedScalarValueInfo)
                            {
                                readValueExpression = Expression.Coalesce(
                                    readValueExpression,
                                    Expression.Default(expression.Type)
                                );
                            }

                            return Expression.Convert(readValueExpression, expression.Type);
                        }
                    }

                    return expression;
                }
            }

            return base.Visit(expression);
        }
    }
}