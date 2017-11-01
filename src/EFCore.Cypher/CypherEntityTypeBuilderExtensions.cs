// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore
{
    public static class CypherEntityTypeBuilderExtensions {

        /// <summary>
        /// Sets the labels that the entity is associated with
        /// </summary>
        /// <param name="entityTypeBuilder"></param>
        /// <param name="labels"></param>
        /// <returns></returns>
        public static EntityTypeBuilder HasLabels(
            [NotNull] this EntityTypeBuilder entityTypeBuilder,
            [NotNull] string[] labels
        ) {
            Check.NotNull(entityTypeBuilder, nameof(entityTypeBuilder));
            Check.NotEmpty(labels, nameof(labels));

            entityTypeBuilder.GetInfrastructure<InternalEntityTypeBuilder>()
                .Cypher(ConfigurationSource.Explicit)
                .HasLabels(labels);

            return entityTypeBuilder;
        }
    }
}