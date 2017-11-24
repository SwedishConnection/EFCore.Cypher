// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query.Cypher;
using Microsoft.EntityFrameworkCore.Utilities;
using Remotion.Linq.Clauses;

namespace Microsoft.EntityFrameworkCore.Query.Expressions {
    public class MatchExpression : NodeExpressionBase, IEquatable<MatchExpression>
    {
        public MatchExpression(
            string[] labels,
            [CanBeNull] string alias,
            [CanBeNull] IQuerySource querySource
        ) : base(querySource, alias) {
            Check.NotEmpty(labels, nameof(labels));

            Labels = labels;
        }

        /// <summary>
        /// Labels
        /// </summary>
        /// <returns></returns>
        public virtual string[] Labels { get; }

        /// <summary>
        /// Where (predicate)
        /// </summary>
        /// <returns></returns>
        public virtual Expression Where { 
            get; 
            
            [param: CanBeNull] set; 
        }

        /// <summary>
        /// Optional match
        /// </summary>
        /// <returns></returns>
        public virtual bool Optional {
            get; 

            set;
        }

        /// <summary>
        /// Append with AndAlso expression predicate to where clause
        /// </summary>
        /// <param name="where"></param>
        public virtual void AppendWhere([NotNull] Expression where) {
            Check.NotNull(where, nameof(where));

            Where = Where is null
                ? where 
                : AndAlso(Where, where);
        }

        /// <summary>
        /// Dispatcher
        /// </summary>
        /// <param name="visitor"></param>
        /// <returns></returns>
        protected override Expression Accept(ExpressionVisitor visitor)
        {
            Check.NotNull(visitor, nameof(visitor));

            var concrete = visitor as ICypherExpressionVisitor;

            return concrete is null
                ? base.Accept(visitor)
                : concrete.VisitMatch(this);
        }

        /// <summary>
        /// Visit children
        /// </summary>
        /// <param name="visitor"></param>
        /// <returns></returns>
        protected override Expression VisitChildren(ExpressionVisitor visitor)
        {
            visitor.Visit(Where);

            return this;
        }

        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(MatchExpression other)
            => Enumerable.SequenceEqual(Labels.OrderBy(l => l), other.Labels.OrderBy(l => l))
                && string.Equals(Alias, other.Alias)
                && Equals(QuerySource, other.QuerySource);

        /// <summary>
        /// Equals (object)
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) {
                return false;
            }

            if (ReferenceEquals(this, obj)) {
                return true;
            }

            if (obj.GetType() != GetType()) {
                return false;
            }

		    return Equals(obj as MatchExpression);
        }

        /// <summary>
        /// Hash
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() {
            unchecked
            {
                var hashCode = Alias?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ (QuerySource?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ Labels?.Aggregate(
                    hashCode,
                    (hc, l) => { return hc ^ l.GetHashCode(); }
                ) ?? 0;

                return hashCode;
            }
        }

        /// <summary>
        /// Human readable
        /// </summary>
        /// <returns></returns>
        public override string ToString()
            => String.Join(":", Labels) + " " + Alias;
    }
}