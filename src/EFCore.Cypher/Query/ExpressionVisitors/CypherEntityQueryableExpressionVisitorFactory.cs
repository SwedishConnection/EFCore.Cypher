// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Utilities;
using Remotion.Linq.Clauses;

namespace Microsoft.EntityFrameworkCore.Query.ExpressionVisitors
{
    public class CypherEntityQueryableExpressionVisitorFactory: IEntityQueryableExpressionVisitorFactory {

        public CypherEntityQueryableExpressionVisitorFactory(
            [NotNull] CypherEntityQueryableExpressionVisitorDependencies dependencies
        ) {
            Check.NotNull(dependencies, nameof(dependencies));

            Dependencies = dependencies;
        }

        /// <summary>
        /// Dependencies
        /// </summary>
        /// <returns></returns>
        protected virtual CypherEntityQueryableExpressionVisitorDependencies Dependencies { get; }

        /// <summary>
        /// Create Expression Visitor
        /// </summary>
        /// <param name="queryModelVisitor"></param>
        /// <param name="querySource"></param>
        /// <returns></returns>
        public virtual ExpressionVisitor Create(
            EntityQueryModelVisitor queryModelVisitor, 
            IQuerySource querySource
        ) => new CypherEntityQueryableExpressionVisitor(
                Dependencies,
                (CypherQueryModelVisitor)Check.NotNull(queryModelVisitor, nameof(queryModelVisitor)),
                querySource
            );
    }
}