// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Query
{
    public class CypherQueryModelVisitorFactory: EntityQueryModelVisitorFactory {

        public CypherQueryModelVisitorFactory(
            [NotNull] EntityQueryModelVisitorDependencies dependencies,
            [NotNull] CypherQueryModelVisitorDependencies cypherDependencies
        ) : base(dependencies) {
            Check.NotNull(cypherDependencies, nameof(cypherDependencies));

            CypherDependencies = cypherDependencies;
        }

        /// <summary>
        /// Dependencies
        /// </summary>
        /// <returns></returns>
        protected virtual CypherQueryModelVisitorDependencies CypherDependencies { get; }

        public override EntityQueryModelVisitor Create(
            QueryCompilationContext queryCompilationContext,
            EntityQueryModelVisitor parentEntityQueryModelVisitor
        ) => new CypherQueryModelVisitor(
                Dependencies,
                CypherDependencies,
                (RelationalQueryCompilationContext)Check.NotNull(queryCompilationContext, nameof(queryCompilationContext)),
                (CypherQueryModelVisitor)parentEntityQueryModelVisitor
            );
    }
}