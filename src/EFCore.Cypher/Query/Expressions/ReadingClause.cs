// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.


using System;
using System.Linq.Expressions;

namespace Microsoft.EntityFrameworkCore.Query.Expressions
{
    public abstract class ReadingClause: Expression {
        
        /// <summary>
        /// Third-party expression
        /// </summary>
        public override ExpressionType NodeType => ExpressionType.Extension;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override Type Type => typeof(object);

        /// <summary>
        /// Visit children
        /// </summary>
        /// <param name="visitor"></param>
        /// <returns></returns>
        protected override Expression VisitChildren(ExpressionVisitor visitor) => this;
    }
}