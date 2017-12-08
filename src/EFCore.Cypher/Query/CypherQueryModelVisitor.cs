// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Extensions.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors.Internal;
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
            // TODO: Are composite predicate and conditional removing visitors necessary

            ParentQueryModelVisitor = parentQueryModelVisiter;
            ContextOptions = cypherDependencies.ContextOptions;
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
        /// Database context options
        /// </summary>
        /// <returns></returns>
        protected virtual IDbContextOptions ContextOptions { get; }

        /// <summary>
        /// Concrete query compilation context
        /// </summary>
        /// <returns></returns>
        public new virtual CypherQueryCompilationContext QueryCompilationContext
            => (CypherQueryCompilationContext)base.QueryCompilationContext;

        /// <summary>
        /// Demoted selectors in body clauses found prior to finding query 
        /// sources that require materialization
        /// </summary>
        /// <returns></returns>
        public virtual Dictionary<int, Dictionary<string, Expression>> DemotedSelectors { get; set; } = new Dictionary<int, Dictionary<string, Expression>>();

        /// <summary>
        /// Get active ReadOnlyExpression
        /// </summary>
        /// <param name="querySource"></param>
        /// <returns></returns>
        public virtual ReadOnlyExpression TryGetQuery([NotNull] IQuerySource querySource) {
            Check.NotNull(querySource, nameof(querySource));

            return QueriesBySource.TryGetValue(querySource, out ReadOnlyExpression roe)
                ? roe
                : QueriesBySource
                    .Values
                    .LastOrDefault(e => e.HandlesQuerySource(querySource));
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

            foreach (var readOnlyExpression in QueriesBySource.Values) {
                // TODO: Eliminate relationships

                // TODO: Opitimzers for predicates
            }
        }

        /// <summary>
        /// Visit additional from clause
        /// </summary>
        /// <param name="fromClause"></param>
        /// <param name="queryModel"></param>
        /// <param name="index"></param>
        public override void VisitAdditionalFromClause(
            AdditionalFromClause fromClause,
            QueryModel queryModel,
            int index)
        {
            Check.NotNull(fromClause, nameof(fromClause));
            Check.NotNull(queryModel, nameof(queryModel));

            var querySource = QuerySourceAt(queryModel, index);
            var readOnlyExpression = TryGetQuery(querySource);
            var numberOfReturnItems = readOnlyExpression?.ReturnItems.Count ?? 0;
            
            base.VisitAdditionalFromClause(fromClause, queryModel, index);

            bool canSelectMany = TrySelectMany(
                fromClause, 
                queryModel, 
                index, 
                numberOfReturnItems
            );

            if (!canSelectMany) {
                // TODO: Client select many
            }
        }

        /// <summary>
        /// Visit where clause
        /// </summary>
        /// <param name="whereClause"></param>
        /// <param name="queryModel"></param>
        /// <param name="index"></param>
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
        /// Visit join clause
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

            // base 
            base.VisitJoinClause(
                joinClause,
                queryModel,
                index
            );

            // 
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
        /// Visited nested queries
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

            // TODO: Relationships enumerable strings
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
        /// Visit select clause
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

            if (Expression is MethodCallExpression methodCallExpression
                && methodCallExpression.Method.MethodIsClosedFormOf(LinqOperatorProvider.Select)) {
                var shape = methodCallExpression.Arguments[0] as MethodCallExpression;

                if (IsShapedQueryExpression(shape)) {
                    shape = UnwrapShapedQueryExpression(shape);
                    var previous = ExtractShaper(shape, 0);

                    // TODO: Is including result operator part of the previous shape?

                    var materializer = (LambdaExpression)methodCallExpression.Arguments[1];
                    if (selectClause.Selector.Type == typeof(AnonymousObject))
                    {
                        // TODO: when anonymous object
                    }

                    var qsreFinder = new CypherQuerySourceReferenceFindingExpressionVisitor();
                    qsreFinder.Visit(materializer.Body);
                    if (!qsreFinder.FoundAny) {
                        Shaper next = null;

                        if (selectClause.Selector is QuerySourceReferenceExpression qsre) {
                            next = previous.Unwrap(qsre.ReferencedQuerySource);
                        }

                        next = next ?? ProjectionShaper.Create(
                            previous, 
                            materializer
                        );

                        Expression = Expression.Call(
                            shape.Method
                                .GetGenericMethodDefinition()
                                .MakeGenericMethod(Expression.Type.GetSequenceType()),
                            shape.Arguments[0],
                            shape.Arguments[1],
                            Expression.Constant(next)
                        );
                    }

                }
            }
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
        /// Try turning join into a match expression
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

            // break apart method calls that must be shaped
            var joinMethodCallExpression = Expression as MethodCallExpression;
            var outer = joinMethodCallExpression?
                    .Arguments
                    .FirstOrDefault() as MethodCallExpression;
            var inner = joinMethodCallExpression?
                    .Arguments
                    .Skip(1)
                    .FirstOrDefault() as MethodCallExpression;

            if (joinMethodCallExpression is null 
                || !joinMethodCallExpression.Method.MethodIsClosedFormOf(LinqOperatorProvider.Join)
                || !IsShapedQueryExpression(outer)
                || !IsShapedQueryExpression(inner)) {
                return false;
            }

            var cypherTranslatingExpressionVisitor = _cypherTranslatingExpressionVisitorFactory
                .Create(this);

            // process either left or right side of the relationship
            bool joiningRelationship = QueryCompilationContext
                .Model
                .IsRelationship(joinClause.ItemType);

            // grab read only expressions and return items to be appended
            var readOnlyExpression = TryGetQuery(
                QuerySourceAt(queryModel, joiningRelationship ? index : index - 1)
            );
            var innerReadOnlyExpression = TryGetQuery(
                joinClause
            );

            var returnItems = QueryCompilationContext
                .QuerySourceRequiresMaterialization(joinClause)
                ? innerReadOnlyExpression.ReturnItems
                : Enumerable.Empty<Expression>();

            // add half of the match expression
            if (joiningRelationship) {
                // grab query source for the demoted selector
                CypherKeySelectorVisitor keySelectorVisitor = new CypherKeySelectorVisitor();
                keySelectorVisitor.Visit(DemotedSelectors[index]["OuterKeySelector"]);

                if (keySelectorVisitor.QuerySource is null) {
                    return false;
                }

                var referencedQuerySource = keySelectorVisitor.QuerySource;
                var left = readOnlyExpression
                    .GetNodeForQuerySource(referencedQuerySource);
                var relationship = innerReadOnlyExpression
                    .GetNodeForQuerySource(joinClause);

                var entityType = QueryCompilationContext
                    .FindEntityType(referencedQuerySource)
                    ?? QueryCompilationContext
                        .Model
                        .FindEntityType(referencedQuerySource.ItemType);

                var relationshipEntityType = QueryCompilationContext
                    .FindEntityType(joinClause)
                    ?? QueryCompilationContext
                        .Model
                        .FindEntityType(joinClause.ItemType);

                if (relationshipEntityType is null) {
                    return false;
                }

                var labels = relationshipEntityType.Cypher()
                    .Labels;

                // clean-up and force offset for shaper
                QueriesBySource.Remove(joinClause);
                readOnlyExpression.RemoveRangeFromReturn(numberOfReturnItems);

                readOnlyExpression.SetRelationshipLeft(
                    new NodePatternExpression(
                        null, 
                        referencedQuerySource, 
                        left.Alias
                    ),
                    new RelationshipDetailExpression(
                        labels,
                        joinClause,
                        relationship.Alias
                    ),
                    returnItems,
                    (e) => QueryCompilationContext
                        .Model
                        .Direction(entityType, relationshipEntityType, e)
                );                
            } else {
                // grab the query source for the demoted selector
                CypherKeySelectorVisitor keySelectorVisitor = new CypherKeySelectorVisitor();
                keySelectorVisitor.Visit(DemotedSelectors[index]["InnerKeySelector"]);

                if (keySelectorVisitor.QuerySource is null) {
                    return false;
                }

                var referencedQuerySource = keySelectorVisitor.QuerySource;
                var node = TryGetQuery(joinClause)
                    .GetNodeForQuerySource(referencedQuerySource);

                var entityType = QueryCompilationContext
                    .FindEntityType(joinClause)
                    ?? QueryCompilationContext
                        .Model
                        .FindEntityType(joinClause.ItemType);
                
                string[] labels = entityType
                    .Cypher()
                    .Labels;

                // clean-up and force the offset for the shaper
                QueriesBySource.Remove(joinClause);
                readOnlyExpression.RemoveRangeFromReturn(numberOfReturnItems);

                readOnlyExpression.SetRelationshipRight(
                    new NodePatternExpression(
                        labels, 
                        referencedQuerySource, 
                        node.Alias
                    ),
                    returnItems,
                    entityType
                );
            }

            // shape with offset
            var outerShaper = ExtractShaper(outer, 0);
            var innerShaper = ExtractShaper(inner, numberOfReturnItems);

            if (innerShaper.Type == typeof(AnonymousObject)) {
                // TODO: when anonymous
                throw new NotImplementedException();
            } else {
                var materializerLambda = (LambdaExpression)joinMethodCallExpression
                    .Arguments
                    .Last();

                var compositeShaper = CompositeShaper.Create(
                    joinClause, 
                    outerShaper, 
                    innerShaper, 
                    materializerLambda.Compile()
                );

                compositeShaper.SaveAccessorExpression(
                    QueryCompilationContext.QuerySourceMapping
                );
                innerShaper.UpdateQuerySource(joinClause);

                Expression = Expression.Call(
                    outer.Method
                        .GetGenericMethodDefinition()
                        .MakeGenericMethod(materializerLambda.ReturnType),
                    outer.Arguments[0],
                    outer.Arguments[1],
                    Expression.Constant(compositeShaper)
                );
            }

            return true;
        }

        /// <summary>
        /// Try turning the additional from clause into a match expression
        /// </summary>
        /// <param name="fromClause"></param>
        /// <param name="queryModel"></param>
        /// <param name="index"></param>
        /// <param name="numberOfReturnItems"></param>
        /// <returns></returns>
        private bool TrySelectMany(
            AdditionalFromClause fromClause,
            QueryModel queryModel,
            int index,
            int numberOfReturnItems
        ) {
            // TODO: Client join or select many

            // grab read only expressions and return items to be appended
            var outerQuerySource = QuerySourceAt(queryModel, index);
            var readOnlyExpression = TryGetQuery(
                outerQuerySource
            );
            if (readOnlyExpression is null) {
                return false;
            }

            var innerReadOnlyExpression = TryGetQuery(
                fromClause
            );
            if (innerReadOnlyExpression is null) {
                return false;
            }

            // break apart linq method
            var selectManyMethodCallExpression = Expression as MethodCallExpression;
            var outer = selectManyMethodCallExpression?
                    .Arguments
                    .FirstOrDefault() as MethodCallExpression;
            var inner = (selectManyMethodCallExpression?
                    .Arguments
                    .Skip(1)
                    .FirstOrDefault() as LambdaExpression)?
                    .Body as MethodCallExpression;

            if (selectManyMethodCallExpression == null
                || !selectManyMethodCallExpression.Method.MethodIsClosedFormOf(LinqOperatorProvider.SelectMany)
                || !IsShapedQueryExpression(outer)
                || !IsShapedQueryExpression(inner))
            {
                return false;
            }

            if (!QueryCompilationContext.QuerySourceRequiresMaterialization(outerQuerySource))
            {
                readOnlyExpression.RemoveRangeFromReturn(numberOfReturnItems);
            }

            // add reading clause
            var node = innerReadOnlyExpression
                .GetNodeForQuerySource(fromClause);

            var entityType = QueryCompilationContext
                .FindEntityType(fromClause)
                ?? QueryCompilationContext
                    .Model
                    .FindEntityType(fromClause.ItemType);
            
            string[] labels = entityType
                .Cypher()
                .Labels;

            var returnItems = QueryCompilationContext
                .QuerySourceRequiresMaterialization(fromClause)
                ? innerReadOnlyExpression.ReturnItems
                : Enumerable.Empty<Expression>();

            if (!innerReadOnlyExpression.HasCorrelation()) {
                readOnlyExpression.AddReadingClause(
                    new MatchExpression(
                        new PatternExpression(
                            new NodePatternExpression(
                                labels,
                                fromClause,
                                node.Alias
                            )
                        )
                    ),
                    returnItems
                );
            }

            // clean-up
            QueriesBySource.Remove(fromClause);

            // shape
            var outerShaper = ExtractShaper(outer, 0);
            var innerShaper = ExtractShaper(inner, numberOfReturnItems);

            var materializerLambda = (LambdaExpression)selectManyMethodCallExpression
                .Arguments
                .Last();

            var compositeShaper = CompositeShaper.Create(
                fromClause, 
                outerShaper, 
                innerShaper, 
                materializerLambda.Compile()
            );

            compositeShaper.SaveAccessorExpression(
                QueryCompilationContext.QuerySourceMapping
            );
            innerShaper.UpdateQuerySource(fromClause);

            Expression = Expression.Call(
                outer.Method
                    .GetGenericMethodDefinition()
                    .MakeGenericMethod(materializerLambda.ReturnType),
                outer.Arguments[0],
                outer.Arguments[1],
                Expression.Constant(compositeShaper)
            );

            return true;
        }

        /// <summary>
        /// Has the base visitor made shaped (Linq) queries that can 
        /// become a <see cref="QueryingEnumerable"> (that actual issues the Cypher
        /// over a database connection)
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

        /// <summary>
        /// Unwrap the method call inside the shaped query
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private MethodCallExpression UnwrapShapedQueryExpression(MethodCallExpression expression)
        {
            if (expression.Method.MethodIsClosedFormOf(LinqOperatorProvider.DefaultIfEmpty)
                || expression.Method.MethodIsClosedFormOf(LinqOperatorProvider.DefaultIfEmptyArg))
            {
                return (MethodCallExpression)expression.Arguments[0];
            }

            return expression;
        }

        /// <summary>
        /// Get the shaper <see cref="IShaper"/> in the shaped query <see cref="QueryMethodProvider"/>
        /// </summary>
        /// <param name="shapedQueryExpression"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        private Shaper ExtractShaper(MethodCallExpression shapedQueryExpression, int offset)
        {
            var shaper = (Shaper)((ConstantExpression)UnwrapShapedQueryExpression(shapedQueryExpression)
                .Arguments[2])
                .Value;

            return shaper
                .WithOffset(offset);
        }
    }
}