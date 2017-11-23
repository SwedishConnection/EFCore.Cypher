// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Utilities;
using Remotion.Linq.Clauses;

namespace Microsoft.EntityFrameworkCore.Query.ExpressionVisitors
{
    public class CypherProjectionExpressionVisitorFactory: IProjectionExpressionVisitorFactory {

        public CypherProjectionExpressionVisitorFactory(
            [NotNull] CypherProjectionExpressionVisitorDependencies dependencies
        ) {
            Check.NotNull(dependencies, nameof(dependencies));

            Dependencies = dependencies;
        }

        /// <summary>
        /// Dependencies
        /// </summary>
        /// <returns></returns>
        protected virtual CypherProjectionExpressionVisitorDependencies Dependencies { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityQueryModelVisitor"></param>
        /// <param name="querySource"></param>
        /// <returns></returns>
        public virtual ExpressionVisitor Create(
            EntityQueryModelVisitor entityQueryModelVisitor, IQuerySource querySource)
            => new CypherProjectionExpressionVisitor(
                Dependencies,
                (CypherQueryModelVisitor)Check.NotNull(entityQueryModelVisitor, nameof(entityQueryModelVisitor)),
                Check.NotNull(querySource, nameof(querySource)));
    }
}