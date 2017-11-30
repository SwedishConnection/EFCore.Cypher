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

    public class NodePatternExpression: NodeExpressionBase, IEquatable<NodePatternExpression> {

        public NodePatternExpression(
            [NotNull] string[] labels,
            [CanBeNull] IQuerySource querySource,
            [CanBeNull] string alias
        ) : base(querySource, alias) {
            Labels = labels;
        }

        /// <summary>
        /// Labels
        /// </summary>
        /// <returns></returns>
        public virtual string[] Labels { get; }

        /// <summary>
        /// Dispatch
        /// </summary>
        /// <param name="visitor"></param>
        /// <returns></returns>
        protected override Expression Accept(ExpressionVisitor visitor)
        {
            Check.NotNull(visitor, nameof(visitor));

            var concrete = visitor as ICypherExpressionVisitor;

            return concrete is null
                ? base.Accept(visitor)
                : concrete.VisitNodePattern(this);
        }

        /// <summary>
        /// Visit children
        /// </summary>
        /// <param name="visitor"></param>
        /// <returns></returns>
        protected override Expression VisitChildren(ExpressionVisitor visitor) => this;

        /// <summary>
        /// Equality
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(NodePatternExpression other) 
            => Enumerable.SequenceEqual(Labels.OrderBy(e => e), other.Labels.OrderBy(e => e))
                && string.Equals(Alias, other.Alias);

        /// <summary>
        /// Equals
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

		    return Equals(obj as NodePatternExpression);
        }

        /// <summary>
        /// Hash
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Alias?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ Labels?.Aggregate(
                    hashCode,
                    (hc, e) => { return hc ^ e.GetHashCode(); }
                ) ?? 0;

                return hashCode;
            }
        }

        /// <summary>
        /// Human readable
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            var NodeLabels = String.Join(":", Labels);

            return $"({Alias}:{NodeLabels})";
        }
    }
}