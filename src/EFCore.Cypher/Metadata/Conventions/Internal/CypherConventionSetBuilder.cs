// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal
{
    public abstract class CypherConventionSetBuilder: IConventionSetBuilder {

        protected CypherConventionSetBuilder(
            [NotNull] RelationalConventionSetBuilderDependencies dependencies
        ) {
            Check.NotNull(dependencies, nameof(dependencies));
        }

        /// <summary>
        /// Access to type mappings (relinq), the database context and finder
        /// </summary>
        /// <returns></returns>
        protected virtual RelationalConventionSetBuilderDependencies Dependencies { get; }

        /// <summary>
        /// Replace or add cypher conventions
        /// </summary>
        /// <param name="conventionSet"></param>
        /// <returns></returns>
        public virtual ConventionSet AddConventions(ConventionSet conventionSet)
        {
            conventionSet.EntityTypeAddedConventions.Add(new CypherLabelsAttributeConvention());

            return conventionSet;
        }
    }
}