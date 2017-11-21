// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Query
{
    public class CypherQueryCompilationContextFactory: QueryCompilationContextFactory {

        public CypherQueryCompilationContextFactory(
            [NotNull] QueryCompilationContextDependencies dependencies,
            [NotNull] RelationalQueryCompilationContextDependencies relationalDependencies
        ) : base(dependencies) {
            Check.NotNull(relationalDependencies, nameof(relationalDependencies));

            // TODO: Add any Cypher specific nodes (relinq)
        }

        /// <summary>
        /// TODO: 
        /// </summary>
        /// <param name="async"></param>
        /// <returns></returns>
        public override QueryCompilationContext Create(bool async)
            => async
                ? new CypherQueryCompilationContext(
                    Dependencies,
                    new AsyncLinqOperatorProvider(),
                    new AsyncQueryMethodProvider(),
                    TrackQueryResults)
                : new CypherQueryCompilationContext(
                    Dependencies,
                    new LinqOperatorProvider(),
                    new QueryMethodProvider(),
                    TrackQueryResults);
    }
}