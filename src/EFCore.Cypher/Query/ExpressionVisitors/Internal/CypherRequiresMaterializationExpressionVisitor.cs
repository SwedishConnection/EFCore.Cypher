// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Utilities;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;

namespace Microsoft.EntityFrameworkCore.Query.ExpressionVisitors.Internal
{
    /// <summary>
    /// Demotes selectors in body clauses saving references in the query
    /// model visitor otherwise the normal promotion/demotion process fails
    /// </summary>
    public class CypherRequiresMaterializationExpressionVisitor: RequiresMaterializationExpressionVisitor {
        private readonly EntityQueryModelVisitor _qmv;

        public CypherRequiresMaterializationExpressionVisitor(
            [NotNull] IModel model,
            [NotNull] EntityQueryModelVisitor queryModelVisitor
        ): base(model, queryModelVisitor) {
            _qmv = queryModelVisitor;
        }

        /// <summary>
        /// Cypher query model visitor
        /// </summary>
        /// <returns></returns>
        private CypherQueryModelVisitor QueryModelVisitor => 
            (CypherQueryModelVisitor)_qmv;

        /// <summary>
        /// Entry point
        /// </summary>
        /// <param name="queryModel"></param>
        /// <returns></returns>
        public override ISet<IQuerySource> FindQuerySourcesRequiringMaterialization(
            [NotNull] Remotion.Linq.QueryModel queryModel
        ) {
            // store demoted selectors
            QueryModelVisitor.DemotedSelectors = queryModel.BodyClauses
                .OfType<JoinClause>()
                .Select((x, i) => { 
                    return new {
                        Key = i, 
                        Value = new Dictionary<string, Expression>() { 
                            { "OuterKeySelector", x.OuterKeySelector },
                            { "InnerKeySelector", x.InnerKeySelector }
                        } 
                    };
                })
                .ToDictionary(
                    x => x.Key, x => x.Value
                );

            DemoteSelectorsVisitor visitor = new DemoteSelectorsVisitor(
                QueryModelVisitor
                    .DemotedSelectors
                    .SelectMany(x => x.Value.Values)
            );

            // relinq transform on the query model
            foreach (var clause in queryModel.BodyClauses.OfType<JoinClause>()) {
                var InnerSequence = clause.InnerSequence;
                clause.TransformExpressions(visitor.Visit);

                if (clause.OuterKeySelector != DemoteSelectorsVisitor.Demotion
                    || clause.InnerKeySelector != DemoteSelectorsVisitor.Demotion 
                    || clause.InnerSequence != InnerSequence) {
                    throw new ArgumentException(
                        CypherStrings.UnableToDemoteSelectors                    
                    );
                }
            }

            // let EF find query source requiring materialization (i.e. returned items)
            var materializing = base.FindQuerySourcesRequiringMaterialization(queryModel);

            return materializing;
        }


        /// <summary>
        /// Replaces demoted selectors with a constant expression
        /// </summary>
        private class DemoteSelectorsVisitor: ExpressionVisitorBase {
            
            private readonly IEnumerable<Expression> _selectors;

            public static Expression Demotion { get; } = Expression.Constant(1);

            public DemoteSelectorsVisitor(IEnumerable<Expression> selectors) {
                _selectors = selectors;
            }

            protected override Expression VisitQuerySourceReference(QuerySourceReferenceExpression expression)
            {
                if (_selectors.Contains(expression)) {
                    return Demotion;
                }

                return base.VisitQuerySourceReference(expression);
            }

            protected override Expression VisitMember(MemberExpression expression) {
                if (_selectors.Contains(expression)) {
                    return Demotion;
                }

                return base.VisitMember(expression);
            }

            protected override Expression VisitParameter(ParameterExpression expression) {
                if (_selectors.Contains(expression)) {
                    return Demotion;
                }

                return base.VisitParameter(expression);
            }

            protected override Expression VisitNew(NewExpression expression) {
                if (_selectors.Contains(expression)) {
                    return Demotion;
                }

                return base.VisitNew(expression);
            }
        }
    }
}