// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal
{
    public class CypherConventionSetBuilder : ICypherConventionSetBuilder
    {
        public CypherConventionSetBuilder([NotNull] CypherConventionSetBuilderDependencies dependencies) {
             Check.NotNull(dependencies, nameof(dependencies));

             Dependencies = dependencies;
        }

        protected virtual CypherConventionSetBuilderDependencies Dependencies { get; }

        public CypherConventionSet CreateCypherConventionSet()
        {
            var cypherConventionSet = new CypherConventionSet();

            return cypherConventionSet;
        }
    }
}