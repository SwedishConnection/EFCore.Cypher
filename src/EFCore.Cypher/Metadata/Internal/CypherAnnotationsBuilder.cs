// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{

    public class CypherAnnotationsBuilder : CypherAnnotations
    {
        public CypherAnnotationsBuilder(
            [NotNull] InternalMetadataBuilder internalBuilder,
            ConfigurationSource configurationSource
        ) : base(internalBuilder.Metadata)
        {
            Check.NotNull(internalBuilder, nameof(internalBuilder));

            MetadataBuilder = internalBuilder;
            ConfigurationSource = configurationSource;
        }

        /// <summary>
        /// Configuration source
        /// </summary>
        /// <returns></returns>
        public virtual ConfigurationSource ConfigurationSource { get; }

        /// <summary>
        /// Wrapped builder
        /// </summary>
        /// <returns></returns>
        public virtual InternalMetadataBuilder MetadataBuilder { get; }
    }
}