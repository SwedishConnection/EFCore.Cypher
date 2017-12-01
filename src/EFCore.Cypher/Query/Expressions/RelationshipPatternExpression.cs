// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query.Cypher;
using Microsoft.EntityFrameworkCore.Utilities;
using Remotion.Linq.Clauses;

namespace Microsoft.EntityFrameworkCore.Query.Expressions {

    public class RelationshipPatternExpression: Expression, IEquatable<RelationshipPatternExpression> {

        public RelationshipPatternExpression(
            [NotNull] RelationshipDirection direction,
            [NotNull] RelationshipDetailExpression details
        ) {
            Direction = direction;
            Details = details;
        }

        /// <summary>
        /// Direction
        /// </summary>
        /// <returns></returns>
        public virtual RelationshipDirection Direction { get; }

        /// <summary>
        /// Details
        /// </summary>
        /// <returns></returns>
        public virtual RelationshipDetailExpression Details { get; }

        /// <summary>
        /// Expression type
        /// </summary>
        public override ExpressionType NodeType => ExpressionType.Extension;

        /// <summary>
        /// Type
        /// </summary>
        /// <returns></returns>
        public override Type Type => typeof(object);

        /// <summary>
        /// Visit children
        /// </summary>
        /// <param name="visitor"></param>
        /// <returns></returns>
        protected override Expression VisitChildren(ExpressionVisitor visitor)
        {
            visitor.Visit(Details);

            return this;
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
                : concrete.VisitRelationshipPattern(this);
        }

        /// <summary>
        /// Equality
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(RelationshipPatternExpression other)
            => Direction == other.Direction &&
                Equals(Details, other.Details);

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

		    return Equals(obj as RelationshipPatternExpression);
        }

        /// <summary>
        /// Hash
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Direction.GetHashCode();
                hashCode = (hashCode * 397) ^ Details.GetHashCode();

                return hashCode;
            }
        }

        /// <summary>
        /// Human readable
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            switch (Direction) {
                case RelationshipDirection.Left:
                    return $"<-{Details}-";
                case RelationshipDirection.Right:
                    return $"-{Details}->";
                case RelationshipDirection.Both:
                    return $"<-{Details}->";
                default:
                    return $"-{Details}-";
            }
        }
    }
}