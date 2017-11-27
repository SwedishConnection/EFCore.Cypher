// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors;

namespace Microsoft.EntityFrameworkCore.Query
{
    public class CypherQueryModelVisitorDependencies {

        public CypherQueryModelVisitorDependencies(
            [NotNull] ICypherResultOperatorHandler cypherResultOperatorHandler,
            [NotNull] ICypherTranslatingExpressionVisitorFactory cypherTranslatingExpressionVisitorFactory,
            [NotNull] IDbContextOptions contextOptions
        ) {
            CypherResultOperatorHandler = cypherResultOperatorHandler;
            CypherTranslatingExpressionVisitorFactory = cypherTranslatingExpressionVisitorFactory;
            ContextOptions = contextOptions;
        }

        /// <summary>
        /// Result operator 
        /// </summary>
        /// <returns></returns>
        public ICypherResultOperatorHandler CypherResultOperatorHandler { get; }

        /// <summary>
        /// Translating expression visitor
        /// </summary>
        /// <returns></returns>
        public ICypherTranslatingExpressionVisitorFactory CypherTranslatingExpressionVisitorFactory { get; }

        /// <summary>
        /// Database context options
        /// </summary>
        /// <returns></returns>
        public IDbContextOptions ContextOptions { get; }
    }
}