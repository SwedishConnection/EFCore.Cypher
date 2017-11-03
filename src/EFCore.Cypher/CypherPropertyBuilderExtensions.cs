// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    /// Semantic differences with <see cref="RelationalPropertyBuilderExtensions" /> where
    /// column is very SQL specific over the term storage
    /// 
    /// TODO: Raise issue
    /// </summary>
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

        /// <summary>
        /// Configures the storage attribute of the database property
        /// </summary>
        /// <param name="propertyBuilder"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static PropertyBuilder<TProperty> HasStorageName<TProperty>(
            [NotNull] this PropertyBuilder<TProperty> propertyBuilder,
            [CanBeNull] string name
        ) => (PropertyBuilder<TProperty>)HasStorageName((PropertyBuilder)propertyBuilder, name);

        /// <summary>
        /// Configures the storage type of the database property
        /// </summary>
        /// <param name="propertyBuilder"></param>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static PropertyBuilder HasStorageType(
            [NotNull] this PropertyBuilder propertyBuilder,
            [CanBeNull] string typeName
        )
        {
            Check.NotNull(propertyBuilder, nameof(propertyBuilder));
            Check.NullButNotEmpty(typeName, nameof(typeName));

            propertyBuilder.GetInfrastructure<InternalPropertyBuilder>()
                .Cypher(ConfigurationSource.Explicit)
                .HasStorageType(typeName);

            return propertyBuilder;
        }

        /// <summary>
        /// Configures the storage type of the database property
        /// </summary>
        /// <param name="propertyBuilder"></param>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static PropertyBuilder<TProperty> HasStorageType<TProperty>(
            [NotNull] this PropertyBuilder<TProperty> propertyBuilder,
            [CanBeNull] string typeName
        ) => (PropertyBuilder<TProperty>)HasStorageType((PropertyBuilder)propertyBuilder, typeName);

        /// <summary>
        /// Configures the default storage constraint 
        /// </summary>
        /// <param name="propertyBuilder"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static PropertyBuilder HasDefaultStorageConstraint(
            [NotNull] this PropertyBuilder propertyBuilder,
            [CanBeNull] string cypher
        )
        {
            Check.NotNull(propertyBuilder, nameof(propertyBuilder));
            Check.NullButNotEmpty(cypher, nameof(cypher));

            var internalPropertyBuilder = propertyBuilder.GetInfrastructure<InternalPropertyBuilder>();
            internalPropertyBuilder
                .Cypher(ConfigurationSource.Explicit)
                .HasDefaultStorageConstraint(cypher);

            return propertyBuilder;
        }

        /// <summary>
        /// Configures the default storage constraint
        /// </summary>
        /// <param name="propertyBuilder"></param>
        /// <param name="cypher"></param>
        /// <returns></returns>
        public static PropertyBuilder<TProperty> HasDefaultStorageConstraint<TProperty>(
            [NotNull] this PropertyBuilder<TProperty> propertyBuilder,
            [CanBeNull] string cypher
        ) => (PropertyBuilder<TProperty>)HasDefaultStorageConstraint((PropertyBuilder)propertyBuilder, cypher);

        /// <summary>
        /// Configures the computed storage constraint
        /// </summary>
        /// <param name="propertyBuilder"></param>
        /// <param name="cypher"></param>
        /// <returns></returns>
        public static PropertyBuilder HasComputedStorageConstraint(
            [NotNull] this PropertyBuilder propertyBuilder,
            [CanBeNull] string cypher
        )
        {
            Check.NotNull(propertyBuilder, nameof(propertyBuilder));
            Check.NullButNotEmpty(cypher, nameof(cypher));

            var internalPropertyBuilder = propertyBuilder.GetInfrastructure<InternalPropertyBuilder>();
            internalPropertyBuilder
                .Cypher(ConfigurationSource.Explicit)
                .HasComputedStorageConstraint(cypher);

            return propertyBuilder;
        }

        /// <summary>
        /// Configures the computed storage constraint
        /// </summary>
        /// <param name="propertyBuilder"></param>
        /// <param name="cypher"></param>
        /// <returns></returns>
        public static PropertyBuilder<TProperty> HasComputedStorageConstraint<TProperty>(
            [NotNull] this PropertyBuilder<TProperty> propertyBuilder,
            [CanBeNull] string cypher
        ) => (PropertyBuilder<TProperty>)HasComputedStorageConstraint((PropertyBuilder)propertyBuilder, cypher);

        /// <summary>
        /// Configures the default value
        /// </summary>
        /// <param name="propertyBuilder"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static PropertyBuilder HasDefaultValue(
            [NotNull] this PropertyBuilder propertyBuilder,
            [CanBeNull] object value = null)
        {
            Check.NotNull(propertyBuilder, nameof(propertyBuilder));

            var internalPropertyBuilder = propertyBuilder.GetInfrastructure<InternalPropertyBuilder>();
            internalPropertyBuilder
                .Cypher(ConfigurationSource.Explicit)
                .HasDefaultValue(value ?? DBNull.Value);

            return propertyBuilder;
        }

        /// <summary>
        /// Configures the default value
        /// </summary>
        /// <param name="propertyBuilder"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static PropertyBuilder<TProperty> HasDefaultValue<TProperty>(
            [NotNull] this PropertyBuilder<TProperty> propertyBuilder,
            [CanBeNull] object value = null)
            => (PropertyBuilder<TProperty>)HasDefaultValue((PropertyBuilder)propertyBuilder, value);
    }
}