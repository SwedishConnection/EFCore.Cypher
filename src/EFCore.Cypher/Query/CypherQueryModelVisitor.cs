// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Extensions.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Utilities;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;

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

            ParentQueryModelVisitor = parentQueryModelVisiter;
        }

        /// <summary>
        /// Active reading expressions
        /// </summary>
        public virtual ICollection<ReadOnlyExpression> Queries => QueriesBySource.Values;

        /// <summary>
        /// Parent query model visitor
        /// </summary>
        /// <returns></returns>
        public virtual CypherQueryModelVisitor ParentQueryModelVisitor { get; }

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
        /// Visit join
        /// </summary>
        /// <param name="joinClause"></param>
        /// <param name="queryModel"></param>
        /// <param name="index"></param>
        public override void VisitJoinClause(
            JoinClause joinClause, 
            QueryModel queryModel, 
            int index) {
            Check.NotNull(joinClause, nameof(joinClause));
            Check.NotNull(queryModel, nameof(queryModel));

            // store before moving on to the base visit
            var querySource = QuerySourceAt(queryModel, index);
            var readOnlyExpression = TryGetQuery(querySource);
            var numberOfReturnItems = readOnlyExpression?.ReturnItems.Count ?? 0;
            var parameter = CurrentParameter;
            var mapping = SnapshotQuerySourceMapping(queryModel);

            base.VisitJoinClause(
                joinClause, 
                queryModel, 
                index
            );

            bool canJoin = TryJoin(
                joinClause, 
                queryModel, 
                index, 
                numberOfReturnItems, 
                parameter, 
                mapping
            );

            // TODO: Requires client join
            if (canJoin) {

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
        /// Compile join clause
        /// </summary>
        /// <param name="joinClause"></param>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        protected override Expression CompileJoinClauseInnerSequenceExpression(
            JoinClause joinClause, 
            QueryModel queryModel
        ) {
            Check.NotNull(joinClause, nameof(joinClause));
            Check.NotNull(queryModel, nameof(queryModel));

            // TODO: Nested query

            Expression expression = base.CompileJoinClauseInnerSequenceExpression(
                joinClause, 
                queryModel
            );

            return expression;
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

                readOnlyExpression = ParentQueryModelVisitor?
                    .TryGetQuery(querySource);

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

        /// <summary>
        /// Start at index grabbing the query source from the 
        /// first body clause or when the index drops to zero 
        /// from the main "from" clause
        /// </summary>
        /// <param name="queryModel"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static IQuerySource QuerySourceAt(
            QueryModel queryModel, 
            int index
        ) {
            for (var i = index; i >= 0; i--) {
                var candidate = i == 0
                    ? queryModel.MainFromClause
                    : queryModel.BodyClauses[i - 1] as IQuerySource;

                if (!(candidate is null)) {
                    return candidate;
                }
            }

            return null;
        }

        /// <summary>
        /// Pair the query model from and body clauses with the expression for the 
        /// query source
        /// </summary>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        private Dictionary<IQuerySource, Expression> SnapshotQuerySourceMapping(
            QueryModel queryModel
        ) {
            var mapping = new Dictionary<IQuerySource, Expression> {
                { 
                    queryModel.MainFromClause,
                    QueryCompilationContext
                        .QuerySourceMapping
                        .GetExpression(queryModel.MainFromClause)
                }
            };

            foreach (var querySource in queryModel.BodyClauses.OfType<IQuerySource>()) {
                bool hasQuerySource = QueryCompilationContext
                    .QuerySourceMapping
                    .ContainsMapping(querySource);

                if (hasQuerySource) {
                    mapping[querySource] = QueryCompilationContext
                        .QuerySourceMapping
                        .GetExpression(querySource);

                    // TODO: Group Join
                }
            }

            return mapping;
        }

        /// <summary>
        /// Try join
        /// </summary>
        /// <param name="joinClause"></param>
        /// <param name="queryModel"></param>
        /// <param name="index"></param>
        /// <param name="numberOfReturnItems"></param>
        /// <param name="parameter"></param>
        /// <param name="Dictionary<IQuerySource"></param>
        /// <param name="mapping"></param>
        /// <returns></returns>
        private bool TryJoin(
            JoinClause joinClause,
            QueryModel queryModel,
            int index,
            int numberOfReturnItems,
            ParameterExpression parameter,
            Dictionary<IQuerySource, Expression> mapping
        ) {
            // TODO: Client join

            var joinMethodCallExpression = Expression as MethodCallExpression;
            if (joinMethodCallExpression is null 
                || !joinMethodCallExpression.Method.MethodIsClosedFormOf(LinqOperatorProvider.Join)) {
                return false;
            }

            var cypherTranslatingExpressionVisitor = _cypherTranslatingExpressionVisitorFactory
                .Create(this);

            if (IsItemTypeARelationship(joinClause)) {
                var outer = joinMethodCallExpression?
                    .Arguments
                    .FirstOrDefault() as MethodCallExpression;

                if (!IsShapedQueryExpression(outer)) {
                    return false;
                }

                var readOnlyExpression = TryGetQuery(
                    QuerySourceAt(queryModel, index)
                );

                if (!(joinClause.OuterKeySelector is QuerySourceReferenceExpression)) {
                    return false;
                }

                var referencedQuerySource = ((QuerySourceReferenceExpression)joinClause.OuterKeySelector)
                    .ReferencedQuerySource;
                var node = readOnlyExpression
                    .GetNodeForQuerySource(referencedQuerySource);

                var labels = ItemToRelationshipTypes(joinClause);
                string alias = QueryCompilationContext
                    .CreateUniqueNodeAlias(
                        ((IQuerySource)joinClause).HasGeneratedItemName()
                        ? labels[0][0].ToString().ToLowerInvariant()
                        : joinClause.ItemName
                );
                
                readOnlyExpression.SetRelationshipLeft(
                    new NodePatternExpression(
                        null, 
                        referencedQuerySource, 
                        node.Alias
                    ),
                    new RelationshipDetailExpression(
                        labels,
                        joinClause,
                        alias
                    )
                );

                // clean up
                QueriesBySource.Remove(joinClause);
                readOnlyExpression.RemoveRangeFromReturn(numberOfReturnItems);
            } else {
                var inner = joinMethodCallExpression?
                    .Arguments
                    .Skip(1)
                    .FirstOrDefault() as MethodCallExpression;

                if (!IsShapedQueryExpression(inner)) {
                    return false;
                }

                var readOnlyExpression = TryGetQuery(
                    QuerySourceAt(queryModel, index)
                );

                var referencedQuerySource = ((QuerySourceReferenceExpression)joinClause.InnerKeySelector)
                    .ReferencedQuerySource;
                var node = TryGetQuery(joinClause)
                    .GetNodeForQuerySource(referencedQuerySource);

                if (node is null) {
                    var entityType = QueryCompilationContext
                        .FindEntityType(joinClause)
                        ?? QueryCompilationContext.Model.FindEntityType(joinClause.ItemType);
                    
                    string[] labels = entityType.Cypher().Labels;
                    string alias = QueryCompilationContext
                        .CreateUniqueNodeAlias(
                            ((IQuerySource)joinClause).HasGeneratedItemName()
                            ? labels[0][0].ToString().ToLowerInvariant()
                            : joinClause.ItemName
                    );

                    readOnlyExpression.SetRelationshipRight(
                        new NodePatternExpression(
                            labels, 
                            referencedQuerySource, 
                            alias
                        )
                    );
                } else {
                    readOnlyExpression.SetRelationshipRight(
                        new NodePatternExpression(
                            null, 
                            referencedQuerySource, 
                            node.Alias
                        )
                    );
                }

                // TODO: Shaper
            }


            return true;
        }

        /// <summary>
        /// Is the item type a relationship
        /// </summary>
        /// <param name="joinClause"></param>
        /// <returns></returns>
        private bool IsItemTypeARelationship(JoinClause joinClause) {
            if (joinClause.ItemType == typeof(string)) {
                if (joinClause.InnerSequence is ConstantExpression constantExpression) {
                    if (typeof(IEnumerable<string>).IsAssignableFrom(constantExpression.Type)) {
                        return QueryCompilationContext
                            .Model
                            .IsRelationship(
                                (constantExpression.Value as IEnumerable<string>).SingleOrDefault()
                            );
                    }
                }
            }

            return QueryCompilationContext
                .Model
                .IsRelationship(joinClause.ItemType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="joinClause"></param>
        /// <returns></returns>
        private string[] ItemToRelationshipTypes(JoinClause joinClause) {
            if (joinClause.ItemType == typeof(string)) {
                if (joinClause.InnerSequence is ConstantExpression constantExpression) {
                    if (typeof(IEnumerable<string>).IsAssignableFrom(constantExpression.Type)) {
                        return constantExpression.Value as string[];
                    }
                }
            }

            return QueryCompilationContext
                .Model
                .FindEntityType(joinClause.ItemType)?
                .Cypher()
                .Labels ?? new string[] {};
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private bool IsShapedQueryExpression(Expression expression) {
            var methodCallExpression = expression as MethodCallExpression;

            if (methodCallExpression == null)
            {
                return false;
            }

            var linqMethods = QueryCompilationContext.LinqOperatorProvider;

            if (methodCallExpression.Method.MethodIsClosedFormOf(linqMethods.DefaultIfEmpty)
                || methodCallExpression.Method.MethodIsClosedFormOf(linqMethods.DefaultIfEmptyArg))
            {
                methodCallExpression = methodCallExpression.Arguments[0] as MethodCallExpression;

                if (methodCallExpression == null)
                {
                    return false;
                }
            }

            var queryMethods = QueryCompilationContext.QueryMethodProvider;

            if (methodCallExpression.Method.MethodIsClosedFormOf(queryMethods.ShapedQueryMethod)
                || methodCallExpression.Method.MethodIsClosedFormOf(queryMethods.DefaultIfEmptyShapedQueryMethod))
            {
                return true;
            }

            return false;
        }
    }
}