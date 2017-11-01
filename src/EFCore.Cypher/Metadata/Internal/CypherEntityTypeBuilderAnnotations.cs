// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    public class CypherEntityTypeBuilderAnnotations : CypherEntityTypeAnnotations
    {
        public CypherEntityTypeBuilderAnnotations(
            [NotNull] InternalEntityTypeBuilder internalBuilder,
            ConfigurationSource configurationSource
        )
            : base(new CypherAnnotationsBuilder(internalBuilder, configurationSource))
        {
        }

        /// <summary>
        /// Set labels
        /// </summary>
        /// <param name="labels"></param>
        /// <returns></returns>
        public virtual bool HasLabels([NotNull] string[] labels) {
            Check.NotEmpty(labels, nameof(labels));

            return SetLabels(labels);
        }
    }
}