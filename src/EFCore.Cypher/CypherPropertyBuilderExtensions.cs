// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore
{
    public static class CypherPropertyBuilderExtensions
    {
        /// <summary>
        /// Configures the storage attribute of the database property
        /// </summary>
        /// <param name="propertyBuilder"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static PropertyBuilder HasStorageName(
            [NotNull] this PropertyBuilder propertyBuilder,
            [CanBeNull] string name)
        {
            Check.NotNull(propertyBuilder, nameof(propertyBuilder));
            Check.NullButNotEmpty(name, nameof(name));

            propertyBuilder
                .GetInfrastructure<InternalPropertyBuilder>()
                .Cypher(ConfigurationSource.Explicit)
                .HasStorageName(name);

            return propertyBuilder;
        }

        public static PropertyBuilder<TProperty> HasStorageName<TProperty>(
            [NotNull] this PropertyBuilder<TProperty> propertyBuilder,
            [CanBeNull] string name
        ) => (PropertyBuilder<TProperty>)HasStorageName((PropertyBuilder)propertyBuilder, name);
    }
}