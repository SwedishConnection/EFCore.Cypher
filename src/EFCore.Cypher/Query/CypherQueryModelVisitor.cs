// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors;
using Microsoft.EntityFrameworkCore.Utilities;
using Remotion.Linq;
using Remotion.Linq.Clauses;

namespace Microsoft.EntityFrameworkCore.Query {

    public class CypherQueryModelVisitor : EntityQueryModelVisitor
    {
        /// <summary>
        /// Cypher read parts can have multiple reading clauses (expressions)
        /// </summary>
        /// <returns></returns>
        protected virtual Dictionary<IQuerySource, ReadOnlyExpression> QueriesBySource { get; } =
            new Dictionary<IQuerySource, ReadOnlyExpression>();

        /// <summary>
        /// Visitor factory
        /// </summary>    
        private readonly ICypherTranslatingExpressionVisitorFactory _cypherTranslatingExpressionVisitorFactory;
        
        public CypherQueryModelVisitor(
            [NotNull] EntityQueryModelVisitorDependencies dependencies,
            [NotNull] CypherQueryModelVisitorDependencies cypherDependencies,
            [NotNull] CypherQueryCompilationContext queryCompilationContext,
            [CanBeNull] CypherQueryModelVisitor parentQueryModelVisiter
        ) : base(dependencies, queryCompilationContext)
        {
            _cypherTranslatingExpressionVisitorFactory = cypherDependencies.CypherTranslatingExpressionVisitorFactory;
            // TODO: Unwrap dependencies
        }

        /// <summary>
        /// Active reading expressions
        /// </summary>
        public virtual ICollection<ReadOnlyExpression> Queries => QueriesBySource.Values;

        /// <summary>
        /// Concrete query compilation context
        /// </summary>
        /// <returns></returns>
        public new virtual CypherQueryCompilationContext QueryCompilationContext
            => (CypherQueryCompilationContext)base.QueryCompilationContext;

        /// <summary>
        /// Get active ReadOnlyExpression
        /// </summary>
        /// <param name="querySource"></param>
        /// <returns></returns>
        public virtual ReadOnlyExpression TryGetQuery([NotNull] IQuerySource querySource) {
            // TODO: What happens when no reading expression is found?
            QueriesBySource.TryGetValue(querySource, out ReadOnlyExpression expr);

            return expr;
        }

        /// <summary>
        /// Add read only expression
        /// </summary>
        /// <param name="querySource"></param>
        /// <param name="readOnlyExpression"></param>
        public virtual void AddQuery(
            [NotNull] IQuerySource querySource,
            [NotNull] ReadOnlyExpression readOnlyExpression
        ) {
            Check.NotNull(querySource, nameof(querySource));
            Check.NotNull(readOnlyExpression, nameof(readOnlyExpression));

            QueriesBySource.Add(querySource, readOnlyExpression);
        }

        /// <summary>
        /// Walk query model
        /// </summary>
        /// <param name="queryModel"></param>
        public override void VisitQueryModel(QueryModel queryModel) {
            Check.NotNull(queryModel, nameof(queryModel));
            
            base.VisitQueryModel(queryModel);

            foreach (var selectExpression in QueriesBySource.Values) {
                // TODO: Eliminate relationships

                // TODO: Opitimzers for predicates
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fromClause"></param>
        /// <param name="queryModel"></param>
        /// <param name="index"></param>
        public override void VisitAdditionalFromClause(
            AdditionalFromClause fromClause,
            QueryModel queryModel,
            int index)
        {
            // TODO: Fold SelectMany (e.g. unwinds)
            base.VisitAdditionalFromClause(fromClause, queryModel, index);
        }

        public override void VisitWhereClause(
            WhereClause whereClause, 
            QueryModel queryModel, 
            int index
        ) {
            Check.NotNull(whereClause, nameof(whereClause));
            Check.NotNull(queryModel, nameof(queryModel));

            var readOnlyExpression = TryGetQuery(queryModel.MainFromClause);
            var requiresClientFilter = readOnlyExpression is null;
            
            if (!requiresClientFilter) {
                var cypherTranslatingExpressionVisitor = _cypherTranslatingExpressionVisitorFactory.Create(
                    queryModelVisitor: this,
                    targetReadOnlyExpression: readOnlyExpression,
                    topLevelWhere: whereClause.Predicate
                );

                var cypherWhereExpression = cypherTranslatingExpressionVisitor.Visit(
                    whereClause.Predicate
                );

                if (!(cypherWhereExpression is null)) {
                    // TODO: Conditional removing expression visitor

                    readOnlyExpression
                        .ReadingClauses
                        .Where(rc => rc is MatchExpression)
                        .Cast<MatchExpression>()
                        .Last()
                        ?.AddToWhere(cypherWhereExpression);
                } else {
                    requiresClientFilter = true;
                }

                // TODO: Client filtering
            }
        }

        /// <summary>
        /// Wrapped queries
        /// </summary>
        /// <param name="queryModel"></param>
        public virtual void VisitSubQueryModel([NotNull] QueryModel queryModel) {
            // TODO
        }

        /// <summary>
        /// Compile main from clause
        /// </summary>
        /// <param name="mainFromClause"></param>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        protected override Expression CompileMainFromClauseExpression(
            MainFromClause mainFromClause,
            QueryModel queryModel
        ) {
            Check.NotNull(mainFromClause, nameof(mainFromClause));
            Check.NotNull(queryModel, nameof(queryModel));

            // TODO: If mainFromClause is a wrapped query then lift otherwise pass to base
            return base.CompileMainFromClauseExpression(mainFromClause, queryModel);
        }

        /// <summary>
        /// Compile additional from clause
        /// </summary>
        /// <param name="additionalFromClause"></param>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        protected override Expression CompileAdditionalFromClauseExpression(
            AdditionalFromClause additionalFromClause,
            QueryModel queryModel)
        {
            // TODO: If additional is a wrapped query then lift otherwise pass to base
            return base.CompileAdditionalFromClauseExpression(additionalFromClause, queryModel);
        }

        /// <summary>
        /// Visit <see cref="SelectClause" /> 
        /// </summary>
        /// <param name="selectClause"></param>
        /// <param name="queryModel"></param>
        public override void VisitSelectClause(
            SelectClause selectClause, 
            QueryModel queryModel
        ) {
            Check.NotNull(selectClause, nameof(selectClause));
            Check.NotNull(queryModel, nameof(queryModel));

            base.VisitSelectClause(selectClause, queryModel);
        }

        /// <summary>
        /// See <see cref="RelationalQueryModelVisitor" />
        /// </summary>
        /// <param name="memberExpression"></param>
        /// <returns></returns>
        public virtual Expression BindMemberToOuterQueryParameter(
            [NotNull] MemberExpression memberExpression)
            => base.BindMemberExpression(
                memberExpression,
                null,
                (property, qs) => BindPropertyToOuterParameter(qs, property, true));

        /// <summary>
        /// See <see cref="RelationalQueryModelVisitor" />
        /// </summary>
        /// <param name="memberExpression"></param>
        /// <param name="Func<IProperty"></param>
        /// <param name="memberBinder"></param>
        /// <param name="bindSubQueries"></param>
        /// <returns></returns>
        public virtual TResult BindMemberExpression<TResult>(
            [NotNull] MemberExpression memberExpression,
            [NotNull] Func<IProperty, IQuerySource, ReadOnlyExpression, TResult> memberBinder,
            bool bindSubQueries = false
        ) {
            Check.NotNull(memberExpression, nameof(memberExpression));
            Check.NotNull(memberBinder, nameof(memberBinder));

            return BindMemberExpression(memberExpression, null, memberBinder, bindSubQueries);
        }

        /// <summary>
        /// See <see cref="RelationalQueryModelVisitor" />
        /// </summary>
        /// <param name="memberExpression"></param>
        /// <param name="querySource"></param>
        /// <param name="memberBinder"></param>
        /// <param name="bindSubQueries"></param>
        /// <returns></returns>
        private TResult BindMemberExpression<TResult>(
            [NotNull] MemberExpression memberExpression,
            [CanBeNull] IQuerySource querySource,
            Func<IProperty, IQuerySource, ReadOnlyExpression, TResult> memberBinder,
            bool bindSubQueries)
        {
            Check.NotNull(memberExpression, nameof(memberExpression));
            Check.NotNull(memberBinder, nameof(memberBinder));

            return base.BindMemberExpression(
                memberExpression, querySource,
                (property, qs) => BindMemberOrMethod(memberBinder, qs, property, bindSubQueries)
            );
        }

        /// <summary>
        /// See <see cref="RelationalQueryModelVisitor" />
        /// </summary>
        /// <param name="methodCallExpression"></param>
        /// <param name="querySource"></param>
        /// <param name="Func<IProperty"></param>
        /// <param name="memberBinder"></param>
        /// <param name="bindSubQueries"></param>
        /// <returns></returns>
        private TResult BindMethodCallExpression<TResult>(
            MethodCallExpression methodCallExpression,
            IQuerySource querySource,
            Func<IProperty, IQuerySource, ReadOnlyExpression, TResult> memberBinder,
            bool bindSubQueries)
            => base.BindMethodCallExpression(
                methodCallExpression,
                querySource,
                (property, qs) => BindMemberOrMethod(memberBinder, qs, property, bindSubQueries));

        /// <summary>
        /// See <see cref="RelationalQueryModelVisitor" />
        /// </summary>
        /// <param name="memberBinder"></param>
        /// <param name="querySource"></param>
        /// <param name="property"></param>
        /// <param name="bindSubQueries"></param>
        /// <returns></returns>
        private TResult BindMemberOrMethod<TResult>(
            Func<IProperty, IQuerySource, ReadOnlyExpression, TResult> memberBinder,
            IQuerySource querySource,
            IProperty property,
            bool bindSubQueries)
        {
            if (!(querySource is null)) {
                var readOnlyExpression = TryGetQuery(querySource);

                if (readOnlyExpression is null && bindSubQueries) {
                    throw new NotImplementedException();
                }

                if (!(readOnlyExpression is null)) {
                    return memberBinder(property, querySource, readOnlyExpression);
                }

                // TODO: Parent query model visitor

                readOnlyExpression?.AddReturnItem(
                    property,
                    querySource
                );
            }

            return default(TResult);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodCallExpression"></param>
        /// <param name="memberBinder"></param>
        /// <param name="bindSubQueries"></param>
        /// <returns></returns>
        public virtual TResult BindMethodCallExpression<TResult>(
            [NotNull] MethodCallExpression methodCallExpression,
            [NotNull] Func<IProperty, IQuerySource, ReadOnlyExpression, TResult> memberBinder,
            bool bindSubQueries = false
        ) {
            Check.NotNull(methodCallExpression, nameof(methodCallExpression));
            Check.NotNull(memberBinder, nameof(memberBinder));

            return BindMethodCallExpression(methodCallExpression, null, memberBinder, bindSubQueries);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="querySource"></param>
        /// <param name="property"></param>
        /// <param name="isMemberExpression"></param>
        /// <returns></returns>
        private ParameterExpression BindPropertyToOuterParameter(
            IQuerySource querySource, 
            IProperty property, 
            bool isMemberExpression)
        {
            throw new NotImplementedException();
        }
    }
}