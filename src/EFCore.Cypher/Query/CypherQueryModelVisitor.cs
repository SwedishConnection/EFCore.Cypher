// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq.Expressions;
using JetBrains.Annotations;
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
        // private readonly ICypherTranslatingExpressionVisitorFactory _cypherTranslatingExpressionVisitorFactory;
        
        public CypherQueryModelVisitor(
            [NotNull] EntityQueryModelVisitorDependencies dependencies,
            [NotNull] CypherQueryModelVisitorDependencies cypherDependencies,
            [NotNull] CypherQueryCompilationContext queryCompilationContext,
            [CanBeNull] CypherQueryModelVisitor parentQueryModelVisiter
        ) : base(dependencies, queryCompilationContext)
        {
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
    }
}