// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors.Internal;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Query.Sql;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;

namespace Microsoft.EntityFrameworkCore.Query.ExpressionVisitors
{
    public class CypherEntityQueryableExpressionVisitor: EntityQueryableExpressionVisitor {

        private readonly IModel _model;

        private readonly IQuerySource _querySource;

        private readonly IReadOnlyExpressionFactory _readOnlyExpressionFactory;

        private readonly ICypherMaterializerFactory _materializerFactory;

        private readonly IShaperCommandContextFactory _shaperCommandContextFactory;

        public CypherEntityQueryableExpressionVisitor(
            [NotNull] CypherEntityQueryableExpressionVisitorDependencies dependencies,
            [NotNull] CypherQueryModelVisitor queryModelVisitor,
            [CanBeNull] IQuerySource querySource
        ) : base(Check.NotNull(queryModelVisitor, nameof(queryModelVisitor))) {
            Check.NotNull(dependencies, nameof(dependencies));

            _model = dependencies.Model;
            _querySource = querySource;
            _readOnlyExpressionFactory = dependencies.ReadOnlyExpressionFactory;
            _materializerFactory = dependencies.MaterializerFactory;
            _shaperCommandContextFactory = dependencies.ShaperCommandContextFactory;
        }

        /// <summary>
        /// Concreate query model visitor
        /// </summary>
        /// <returns></returns>
        private new CypherQueryModelVisitor QueryModelVisitor => 
            (CypherQueryModelVisitor)base.QueryModelVisitor;

        /// <summary>
        /// Resolve read only expression
        /// </summary>
        /// <param name="elementType"></param>
        /// <returns></returns>
        protected override Expression VisitEntityQueryable([NotNull] Type elementType)
        {
            Check.NotNull(elementType, nameof(elementType));

            // create then add read only expression
            var cypherQueryCompilationContext = QueryModelVisitor
                .QueryCompilationContext;

            var entityType = cypherQueryCompilationContext.FindEntityType(_querySource)
                ?? _model.FindEntityType(elementType);

            var readOnlyExpression = _readOnlyExpressionFactory
                .Create(cypherQueryCompilationContext);

            QueryModelVisitor.AddQuery(
                _querySource, 
                readOnlyExpression
            );


            // unique node alias either from the Relinq query source if not generated or the first label
            string[] labels = entityType.Cypher().Labels;
            string alias = cypherQueryCompilationContext
                .CreateUniqueNodeAlias(
                    _querySource.HasGeneratedItemName()
                        ? labels[0][0].ToString().ToLowerInvariant()
                        : _querySource.ItemName
                );

            // TODO: Use from SQL annotation?
            // default match
            readOnlyExpression.AddReadingClause(
                new MatchExpression(
                    labels,
                    alias,
                    _querySource
                )
            );
            
            // bundle the sql (cypher) generator inside the shaper command factory
            Func<IQuerySqlGenerator> querySqlGeneratorFunc = readOnlyExpression
                .CreateDefaultQueryCypherGenerator;
            var shaper = CreateShaper(elementType, entityType, readOnlyExpression);

            return Expression.Call(
                QueryModelVisitor.QueryCompilationContext.QueryMethodProvider
                    .ShapedQueryMethod
                    .MakeGenericMethod(shaper.Type),
                EntityQueryModelVisitor.QueryContextParameter,
                Expression.Constant(_shaperCommandContextFactory.Create(querySqlGeneratorFunc)),
                Expression.Constant(shaper));
        }

        /// <summary>
        /// TODO: Necessary ?
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        protected override Expression VisitSubQuery(SubQueryExpression expression)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// e.g. when member assignment in body of select
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitMember(MemberExpression node)
        {
            Check.NotNull(node, nameof(node));

            QueryModelVisitor
                .BindMemberExpression(
                    node,
                    (p, qs, roe) => roe.AddReturnItem(p, qs),
                    bindSubQueries: true
                );

            return base.VisitMember(node);
        }

        /// <summary>
        /// TODO: Necessary ?
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            Check.NotNull(node, nameof(node));

            QueryModelVisitor
                .BindMethodCallExpression(
                    node,
                    (p, qs, roe) => roe.AddReturnItem(p, qs),
                    bindSubQueries: true
                );
            
            return base.VisitMethodCall(node);
        }

        /// <summary>
        /// Shaper (?) helps transfer entities potentially from state
        /// </summary>
        /// <param name="elementType"></param>
        /// <param name="entityType"></param>
        /// <param name="readOnlyExpression"></param>
        /// <returns></returns>
        private Shaper CreateShaper(
            Type elementType,
            IEntityType entityType,
            ReadOnlyExpression readOnlyExpression
        ) {
            Shaper shaper;

            if (QueryModelVisitor
                    .QueryCompilationContext
                    .QuerySourceRequiresMaterialization(_querySource)) {
                var materializer = _materializerFactory
                    .CreateMaterializer(
                        entityType,
                        readOnlyExpression,
                        (p, roe) => roe.AddReturnItem(p, _querySource),
                        _querySource,
                        out var typeIndexMapping
                    )
                    .Compile();

                shaper = (Shaper)_createEntityShaperMethodInfo.MakeGenericMethod(elementType)
                    .Invoke(
                        null, new object[]
                        {
                            _querySource,
                            QueryModelVisitor.QueryCompilationContext.IsTrackingQuery,
                            entityType.FindPrimaryKey(),
                            materializer,
                            typeIndexMapping,
                            QueryModelVisitor.QueryCompilationContext.IsQueryBufferRequired
                        });
            } else {
                // TODO: Handle discriminate?

                shaper = new ValueBufferShaper(_querySource);
            }

            return shaper;
        }

        /// <summary>
        /// Create entity shaper footprint
        /// </summary>
        /// <returns></returns>
        private static readonly MethodInfo _createEntityShaperMethodInfo
            = typeof(CypherEntityQueryableExpressionVisitor)
                .GetTypeInfo()
                .GetDeclaredMethod(nameof(CreateEntityShaper));

        /// <summary>
        /// Either a buffered or unbuffered entity shaper
        /// </summary>
        /// <param name="querySource"></param>
        /// <param name="trackingQuery"></param>
        /// <param name="key"></param>
        /// <param name="Func<ValueBuffer"></param>
        /// <param name="materializer"></param>
        /// <param name="Dictionary<Type"></param>
        /// <param name="typeIndexMapping"></param>
        /// <param name="useQueryBuffer"></param>
        /// <returns></returns>
        [UsedImplicitly]
        private static IShaper<TEntity> CreateEntityShaper<TEntity>(
            IQuerySource querySource,
            bool trackingQuery,
            IKey key,
            Func<ValueBuffer, object> materializer,
            Dictionary<Type, int[]> typeIndexMapping,
            bool useQueryBuffer)
            where TEntity : class
            => !useQueryBuffer
                ? (IShaper<TEntity>)new UnbufferedEntityShaper<TEntity>(
                    querySource,
                    trackingQuery,
                    key,
                    materializer)
                : new BufferedEntityShaper<TEntity>(
                    querySource,
                    trackingQuery,
                    key,
                    materializer,
                    typeIndexMapping);
    }
}