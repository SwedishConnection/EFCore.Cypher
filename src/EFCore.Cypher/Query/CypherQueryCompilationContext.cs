// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Query
{
    public class CypherQueryCompilationContext: QueryCompilationContext {

        public CypherQueryCompilationContext(
            [NotNull] QueryCompilationContextDependencies dependencies,
            [NotNull] ILinqOperatorProvider linqOperatorProvider,
            [NotNull] IQueryMethodProvider queryMethodProvider,
            bool trackQueryResults
        ) : base(dependencies, linqOperatorProvider, trackQueryResults) {
            Check.NotNull(queryMethodProvider, nameof(queryMethodProvider));

            QueryMethodProvider = queryMethodProvider;
        }

        public virtual IQueryMethodProvider QueryMethodProvider { get; }
    }
}