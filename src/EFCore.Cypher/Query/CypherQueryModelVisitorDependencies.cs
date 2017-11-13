// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Microsoft.EntityFrameworkCore.Query
{
    public class CypherQueryModelVisitorDependencies {

        public CypherQueryModelVisitorDependencies(
            [NotNull] ICypherResultOperatorHandler cypherResultOperatorHandler,
            [NotNull] IDbContextOptions contextOptions
        ) {
            CypherResultOperatorHandler = cypherResultOperatorHandler;
            ContextOptions = contextOptions;
        }

        public ICypherResultOperatorHandler CypherResultOperatorHandler { get; }

        public IDbContextOptions ContextOptions { get; }
    }
}