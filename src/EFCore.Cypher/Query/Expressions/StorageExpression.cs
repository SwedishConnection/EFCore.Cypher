// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query.Cypher;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Query.Expressions
{
    public class StorageExpression: Expression, IEquatable<StorageExpression> {

        public StorageExpression(
            [NotNull] string name,
            [NotNull] IProperty property,
            [NotNull] NodeExpressionBase nodeExpression
        ) {
            Name = name;
            Property = property;
            Node = nodeExpression;
        }

        /// <summary>
        /// Storage name
        /// </summary>
        /// <returns></returns>
        public virtual string Name { get; }

        /// <summary>
        /// Node
        /// </summary>
        /// <returns></returns>
        public virtual NodeExpressionBase Node { get; }

        /// <summary>
        /// Property
        /// </summary>
        /// <returns></returns>
        #pragma warning disable 108
        public virtual IProperty Property { get; }
        #pragma warning restore 108

        /// <summary>
        /// Extension
        /// </summary>
        public override ExpressionType NodeType => ExpressionType.Extension;

        /// <summary>
        /// Type
        /// </summary>
        public override Type Type => Property.ClrType;

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
                : concrete.VisitStorage(this);
        }

        /// <summary>
        /// Visit children
        /// </summary>
        /// <param name="visitor"></param>
        /// <returns></returns>
        protected override Expression VisitChildren(ExpressionVisitor visitor) => this;

        /// <summary>
        /// Equals
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(StorageExpression other)
            => string.Equals(Name, other.Name)
                && Type == other.Type
                && Equals(Node, other.Node);


        /// <summary>
        /// Hash
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Type.GetHashCode();
                hashCode = (hashCode * 397) ^ Node.GetHashCode();
                hashCode = (hashCode * 397) ^ Name.GetHashCode();

                return hashCode;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Node.Alias + "." + Name;
    }
}