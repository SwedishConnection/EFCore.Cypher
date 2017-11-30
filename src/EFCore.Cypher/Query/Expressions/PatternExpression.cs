// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query.Cypher;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Query.Expressions {

    public class PatternExpression: Expression, IEquatable<PatternExpression> {

        public PatternExpression(
            [NotNull] NodePatternExpression nodePattern,
            [CanBeNull] Tuple<RelationshipPatternExpression, NodePatternExpression>[] patternElementChain = null
        ) {
            Check.NotNull(nodePattern, nameof(nodePattern));

            NodePattern = nodePattern;
            PatternElementChain = patternElementChain;
        }

        /// <summary>
        /// Node pattern
        /// </summary>
        /// <returns></returns>
        public virtual NodePatternExpression NodePattern { get; }

        /// <summary>
        /// Pattern element chain
        /// </summary>
        /// <returns></returns>
        public virtual Tuple<RelationshipPatternExpression, NodePatternExpression>[] PatternElementChain { get; }

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
            visitor.Visit(NodePattern);

            if (!(PatternElementChain is null)) {
                foreach (Tuple<RelationshipPatternExpression, NodePatternExpression> chain in PatternElementChain) {
                    visitor.Visit(chain.Item1);
                    visitor.Visit(chain.Item2);
                }
            }

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
                : concrete.VisitPattern(this);
        }

        /// <summary>
        /// Equality
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(PatternExpression other)
            => Equals(NodePattern, other.NodePattern) &&
                Enumerable.SequenceEqual(
                    PatternElementChain ?? 
                        Enumerable.Empty<Tuple<RelationshipPatternExpression, NodePatternExpression>>(), 
                    other.PatternElementChain ?? 
                        Enumerable.Empty<Tuple<RelationshipPatternExpression, NodePatternExpression>>()
                );

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

		    return Equals(obj as PatternExpression);
        }

        /// <summary>
        /// Hash
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = NodePattern.GetHashCode();
                hashCode = (hashCode * 397) ^ PatternElementChain?.Aggregate(
                    hashCode,
                    (hc, e) => { return hc ^ e.Item1.GetHashCode() ^ e.Item2.GetHashCode(); }
                ) ?? 0;

                return hashCode;
            }
        }

        /// <summary>
        /// Human readable
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return $"{NodePattern}{PatternElementChain}";
        }
    }
}