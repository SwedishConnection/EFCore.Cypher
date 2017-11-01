// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    public static class CypherInternalMetadataBuilderExtensions
    {
        /// <summary>
        /// Wrap the internal entity type builder
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configurationSource"></param>
        /// <returns></returns>
        public static CypherEntityTypeBuilderAnnotations Cypher(
            [NotNull] this InternalEntityTypeBuilder builder,
            ConfigurationSource configurationSource
        ) => new CypherEntityTypeBuilderAnnotations(builder, configurationSource);
    }
}