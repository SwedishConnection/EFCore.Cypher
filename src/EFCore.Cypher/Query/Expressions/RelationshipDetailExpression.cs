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

    public class RelationshipDetailExpression: NodeExpressionBase, IEquatable<RelationshipDetailExpression> {

        public RelationshipDetailExpression(
            [CanBeNull] string[] kinds,
            [CanBeNull] IQuerySource querySource,
            [CanBeNull] string alias
        ) : base(querySource, alias) {
            Kinds = kinds ?? new string[] {};
        }

        /// <summary>
        /// Kinds (types)
        /// </summary>
        /// <returns></returns>
        public virtual string[] Kinds { get; }

        /// <summary>
        /// Range
        /// </summary>
        /// <returns></returns>
        public virtual Tuple<byte?, byte?> Range { 
            get; 

            [param: CanBeNull]
            set;
        }

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
                : concrete.VisitRelationshipDetail(this);
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
        public bool Equals(RelationshipDetailExpression other)
            => Enumerable.SequenceEqual(Kinds.OrderBy(e => e), other.Kinds.OrderBy(e => e))
                && string.Equals(Alias, other.Alias)
                && Equals(Range, other.Range);

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

		    return Equals(obj as RelationshipDetailExpression);
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
                hashCode = (hashCode * 397) ^ Kinds?.Aggregate(
                    hashCode,
                    (hc, e) => { return hc ^ e.GetHashCode(); }
                ) ?? 0;
                hashCode = (hashCode * 397) ^ Range?.Item1.Value ?? 0;
                hashCode = (hashCode * 397) ^ Range?.Item2.Value ?? 0;

                return hashCode;
            }
        }

        /// <summary>
        /// Human readable
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            var Types = Kinds.Count() == 0
                ? String.Empty
                : ":" + String.Join("|:", Kinds);

            var RangeLiteral = Range is null
                ? String.Empty
                : "*" + 
                    (Range.Item1.HasValue ? Range.Item1.Value.ToString() : String.Empty) +
                    ".." +
                    (Range.Item2.HasValue ? Range.Item2.Value.ToString() : String.Empty);

            return $"[{Alias}{Types}{RangeLiteral}]";
        }
    }
}