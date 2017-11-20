// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query.Cypher;
using Microsoft.EntityFrameworkCore.Query.Sql;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.EntityFrameworkCore.Storage;
using Remotion.Linq.Clauses;
using System.Linq;

namespace Microsoft.EntityFrameworkCore.Query.Expressions
{
    /// <summary>
    /// See https://github.com/opencypher/openCypher/blob/master/grammar/cypher.xml
    /// 
    /// Read only expression with a Reading clause like Match or Unwind which maps 
    /// to a Relinq QuerySource then a Return.  Unfolding a Match might have an optional 
    /// match, hints and a predicate.  Returns can have a distinct aggregation.
    /// 
    /// A Cypher read only expression can contain multiple reading clauses.
    /// </summary>
    public class ReadOnlyExpression: NodeExpressionBase {

        private readonly CypherQueryCompilationContext _queryCompliationContext;

        private readonly List<Expression> _returnItems = new List<Expression>();

        private readonly List<NodeExpressionBase> _readingClauses = new List<NodeExpressionBase>();

        private readonly List<Ordering> _order = new List<Ordering>();

        private Expression _limit;

        private Expression _skip;

        private bool _isReturnStar;

        public ReadOnlyExpression(
            [NotNull] ReadOnlyExpressionDependencies dependencies,
            [NotNull] CypherQueryCompilationContext queryCompilationContext
        ): base(null, null) {
            Check.NotNull(dependencies, nameof(dependencies));
            Check.NotNull(queryCompilationContext, nameof(queryCompilationContext));
            
            Dependencies = dependencies;
            _queryCompliationContext = queryCompilationContext;
        }

        /// <summary>
        /// Dependencies
        /// </summary>
        /// <returns></returns>
        protected virtual ReadOnlyExpressionDependencies Dependencies { get; }

        /// <summary>
        /// Type of single read item or the type of the node expression base
        /// </summary>
        public override Type Type => _returnItems.Count == 1
            ? _returnItems[0].Type
            : base.Type;

        /// <summary>
        /// Visit the read items, reading clauses (plus predicates) and ordering expressions
        /// </summary>
        /// <param name="visitor"></param>
        /// <returns></returns>
        protected override Expression VisitChildren(ExpressionVisitor visitor) {
            foreach (var readItem in ReturnItems) {
                visitor.Visit(readItem);
            }

            foreach (var readingClause in ReadingClauses) {
                visitor.Visit(readingClause);
            }

            foreach (var ordering in Order) {
                visitor.Visit(ordering.Expression);
            }

            return this;
        }

        /// <summary>
        /// Return items (projections)
        /// </summary>
        public virtual IReadOnlyList<Expression> ReturnItems => _returnItems;

        /// <summary>
        /// Reading clauses (match, unwind etc.) which including the where clause (predicate)
        /// </summary>
        public virtual IReadOnlyList<NodeExpressionBase> ReadingClauses => _readingClauses;

        /// <summary>
        /// Order clause
        /// </summary>
        public virtual IReadOnlyList<Ordering> Order => _order;

        /// <summary>
        /// Limit
        /// </summary>
        /// <returns></returns>
        public virtual Expression Limit {
            get => _limit;

            [param: CanBeNull]
            set {
                // TODO: if limit is already set
                _limit = value;
            }
        }

        /// <summary>
        /// Skip
        /// </summary>
        /// <returns></returns>
        public virtual Expression Skip {
            get => _skip;

            [param: CanBeNull]
            set {
                // TODO: if limit is already set
                _skip = value;
            }
        }

        /// <summary>
        /// Dispatch (cypher expression visitor)
        /// </summary>
        /// <param name="visitor"></param>
        /// <returns></returns>
        protected override Expression Accept(ExpressionVisitor visitor) {
            Check.NotNull(visitor, nameof(visitor));

            var concrete = visitor as ICypherExpressionVisitor;

            return concrete is null
                ? base.Accept(visitor)
                : concrete.VisitReadOnly(this);
        }

        /// <summary>
        /// SQL generator (really cypher used by the shaper command context)
        /// </summary>
        /// <returns></returns>
        public virtual IQuerySqlGenerator CreateDefaultQueryCypherGenerator() => 
            Dependencies.QueryCypherGeneratorFactory.CreateDefault(this);

        /// <summary>
        /// Return everything (star)
        /// </summary>
        /// <returns></returns>
        public virtual bool IsReturnStar {
            get => _isReturnStar;

            set {
                _isReturnStar = value;

                // TODO: add range or clear
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<Type> GetReturnTypes() {
            if (_returnItems.Any() || !IsReturnStar) {
                return _returnItems
                    .Select(i => i.NodeType == ExpressionType.Convert && i.Type == typeof(object)
                        ? ((UnaryExpression)i).Operand.Type
                        : i.Type
                    );
            }

            return _readingClauses
                .OfType<ReadOnlyExpression>()
                .SelectMany(r => r.GetReturnTypes());
        }
    }
}