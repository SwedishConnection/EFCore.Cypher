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
    public static class CypherCollectionNavigationBuilderExtensions
    {
        /// <summary>
        /// Relationship by name without properties
        /// </summary>
        /// <param name="collectionNavigationBuilder"></param>
        /// <param name="name"></param>
        /// <param name="startingClrType"></param>
        /// <returns></returns>
        public static CollectionNavigationBuilder HasRelationship(
            [NotNull] this CollectionNavigationBuilder collectionNavigationBuilder,
            [CanBeNull] string name,
            [CanBeNull] Type startingClrType
        )
        {
            Check.NotNull(collectionNavigationBuilder, nameof(collectionNavigationBuilder));
            Check.NullButNotEmpty(name, nameof(name));

            collectionNavigationBuilder.GetInfrastructure<InternalRelationshipBuilder>()
                .Cypher(ConfigurationSource.Explicit)
                .HasRelationship(name, startingClrType);

            return collectionNavigationBuilder;
        }

        /// <summary>
        /// Relationship by name without properties
        /// </summary>
        /// <param name="collectionNavigationBuilder"></param>
        /// <param name="name"></param>
        /// <param name="startingName"></param>
        /// <returns></returns>
        public static CollectionNavigationBuilder HasRelationship(
            [NotNull] this CollectionNavigationBuilder collectionNavigationBuilder,
            [CanBeNull] string name,
            [CanBeNull] string startingName
        ) {
            Check.NotNull(collectionNavigationBuilder, nameof(collectionNavigationBuilder));
            Check.NullButNotEmpty(name, nameof(name));

            collectionNavigationBuilder.GetInfrastructure<InternalRelationshipBuilder>()
                .Cypher(ConfigurationSource.Explicit)
                .HasRelationship(name, startingName);

            return collectionNavigationBuilder;
        }

        /// <summary>
        /// Relationship by name without properties where the declaring entity
        /// is the start of the relationship
        /// </summary>
        /// <param name="collectionNavigationBuilder"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static CollectionNavigationBuilder<TEntity, TRelatedEntity> HasRelationship<TEntity, TRelatedEntity>(
            [NotNull] this CollectionNavigationBuilder<TEntity, TRelatedEntity> collectionNavigationBuilder,
            [CanBeNull] string name)
            where TEntity : class
            where TRelatedEntity : class
            => HasRelationship(
                (CollectionNavigationBuilder)collectionNavigationBuilder, 
                name,
                typeof(TEntity)
            ) as CollectionNavigationBuilder<TEntity, TRelatedEntity>;

        /// <summary>
        /// Relationship by Clr type
        /// </summary>
        /// <param name="collectionNavigationBuilder"></param>
        /// <param name="clrType"></param>
        /// <param name="startingClrType"></param>
        /// <returns></returns>
        public static CollectionNavigationBuilder HasRelationship(
            [NotNull] this CollectionNavigationBuilder collectionNavigationBuilder,
            [CanBeNull] Type clrType,
            [CanBeNull] Type startingClrType
        )
        {
            Check.NotNull(collectionNavigationBuilder, nameof(collectionNavigationBuilder));

            collectionNavigationBuilder.GetInfrastructure<InternalRelationshipBuilder>()
                .Cypher(ConfigurationSource.Explicit)
                .HasRelationship(clrType, startingClrType);

            return collectionNavigationBuilder;
        }

        /// <summary>
        /// Relationship by Clr type
        /// </summary>
        /// <param name="collectionNavigationBuilder"></param>
        /// <param name="clrType"></param>
        /// <param name="startingName"></param>
        /// <returns></returns>
        public static CollectionNavigationBuilder HasRelationship(
            [NotNull] this CollectionNavigationBuilder collectionNavigationBuilder,
            [CanBeNull] Type clrType,
            [CanBeNull] string startingName
        )
        {
            Check.NotNull(collectionNavigationBuilder, nameof(collectionNavigationBuilder));

            collectionNavigationBuilder.GetInfrastructure<InternalRelationshipBuilder>()
                .Cypher(ConfigurationSource.Explicit)
                .HasRelationship(clrType, startingName);

            return collectionNavigationBuilder;
        }

        /// <summary>
        /// Relationship by Clr type where the declaring type
        /// is the start of the relationship
        /// </summary>
        /// <param name="collectionNavigationBuilder"></param>
        /// <param name="clrType"></param>
        /// <returns></returns>
        public static CollectionNavigationBuilder<TEntity, TRelatedEntity> HasRelationship<TEntity, TRelatedEntity>(
            [NotNull] this CollectionNavigationBuilder<TEntity, TRelatedEntity> collectionNavigationBuilder,
            [CanBeNull] Type clrType)
            where TEntity : class
            where TRelatedEntity : class
            => HasRelationship(
                (CollectionNavigationBuilder)collectionNavigationBuilder, 
                clrType,
                typeof(TEntity)
            ) as CollectionNavigationBuilder<TEntity, TRelatedEntity>;

    }
}