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

        /// <summary>
        /// Wrap the internal property type builder
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configurationSource"></param>
        /// <returns></returns>
        public static CypherPropertyBuilderAnnotations Cypher(
            [NotNull] this InternalPropertyBuilder builder,
            ConfigurationSource configurationSource)
            => new CypherPropertyBuilderAnnotations(builder, configurationSource);

        /// <summary>
        /// Wrap the internal relationship builder
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configurationSource"></param>
        /// <returns></returns>
        public static CypherForeignKeyBuilderAnnotations Cypher(
            [NotNull] this InternalRelationshipBuilder builder,
            ConfigurationSource configurationSource)
            => new CypherForeignKeyBuilderAnnotations(builder, configurationSource);
    }
}