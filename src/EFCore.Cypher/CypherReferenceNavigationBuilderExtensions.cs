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
    public static class CypherReferenceNavigationBuilderExtensions
    {
        /// <summary>
        /// Relationship by name without properties
        /// </summary>
        /// <param name="referenceNavigationBuilder"></param>
        /// <param name="name"></param>
        /// <param name="startingClrType"></param>
        /// <returns></returns>
        public static ReferenceNavigationBuilder HasRelationship(
            [NotNull] this ReferenceNavigationBuilder referenceNavigationBuilder,
            [CanBeNull] string name,
            [CanBeNull] Type startingClrType
        )
        {
            Check.NotNull(referenceNavigationBuilder, nameof(referenceNavigationBuilder));
            Check.NullButNotEmpty(name, nameof(name));

            referenceNavigationBuilder.GetInfrastructure<InternalRelationshipBuilder>()
                .Cypher(ConfigurationSource.Explicit)
                .HasRelationship(name, startingClrType);

            return referenceNavigationBuilder;
        }

        /// <summary>
        /// Relationship by name without properties
        /// </summary>
        /// <param name="referenceNavigationBuilder"></param>
        /// <param name="name"></param>
        /// <param name="startingName"></param>
        /// <returns></returns>
        public static ReferenceNavigationBuilder HasRelationship(
            [NotNull] this ReferenceNavigationBuilder referenceNavigationBuilder,
            [CanBeNull] string name,
            [CanBeNull] string startingName
        ) {
            Check.NotNull(referenceNavigationBuilder, nameof(referenceNavigationBuilder));
            Check.NullButNotEmpty(name, nameof(name));

            referenceNavigationBuilder.GetInfrastructure<InternalRelationshipBuilder>()
                .Cypher(ConfigurationSource.Explicit)
                .HasRelationship(name, startingName);

            return referenceNavigationBuilder;
        }

        /// <summary>
        /// Relationship by name without properties where the declaring entity
        /// is the start of the relationship
        /// </summary>
        /// <param name="referenceNavigationBuilder"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ReferenceNavigationBuilder<TEntity, TRelatedEntity> HasRelationship<TEntity, TRelatedEntity>(
            [NotNull] this ReferenceNavigationBuilder<TEntity, TRelatedEntity> referenceNavigationBuilder,
            [CanBeNull] string name)
            where TEntity : class
            where TRelatedEntity : class
            => HasRelationship(
                (ReferenceNavigationBuilder)referenceNavigationBuilder, 
                name,
                typeof(TEntity)
            ) as ReferenceNavigationBuilder<TEntity, TRelatedEntity>;

        /// <summary>
        /// Relationship by Clr type
        /// </summary>
        /// <param name="referenceNavigationBuilder"></param>
        /// <param name="clrType"></param>
        /// <param name="startingClrType"></param>
        /// <returns></returns>
        public static ReferenceNavigationBuilder HasRelationship(
            [NotNull] this ReferenceNavigationBuilder referenceNavigationBuilder,
            [CanBeNull] Type clrType,
            [CanBeNull] Type startingClrType
        )
        {
            Check.NotNull(referenceNavigationBuilder, nameof(referenceNavigationBuilder));

            referenceNavigationBuilder.GetInfrastructure<InternalRelationshipBuilder>()
                .Cypher(ConfigurationSource.Explicit)
                .HasRelationship(clrType, startingClrType);

            return referenceNavigationBuilder;
        }

        /// <summary>
        /// Relationship by Clr type
        /// </summary>
        /// <param name="referenceNavigationBuilder"></param>
        /// <param name="clrType"></param>
        /// <param name="startingName"></param>
        /// <returns></returns>
        public static ReferenceNavigationBuilder HasRelationship(
            [NotNull] this ReferenceNavigationBuilder referenceNavigationBuilder,
            [CanBeNull] Type clrType,
            [CanBeNull] string startingName
        )
        {
            Check.NotNull(referenceNavigationBuilder, nameof(referenceNavigationBuilder));

            referenceNavigationBuilder.GetInfrastructure<InternalRelationshipBuilder>()
                .Cypher(ConfigurationSource.Explicit)
                .HasRelationship(clrType, startingName);

            return referenceNavigationBuilder;
        }

        /// <summary>
        /// Relationship by Clr type where the declaring type
        /// is the start of the relationship
        /// </summary>
        /// <param name="referenceNavigationBuilder"></param>
        /// <param name="clrType"></param>
        /// <returns></returns>
        public static ReferenceNavigationBuilder<TEntity, TRelatedEntity> HasRelationship<TEntity, TRelatedEntity>(
            [NotNull] this ReferenceNavigationBuilder<TEntity, TRelatedEntity> referenceNavigationBuilder,
            [CanBeNull] Type clrType)
            where TEntity : class
            where TRelatedEntity : class
            => HasRelationship(
                (ReferenceNavigationBuilder)referenceNavigationBuilder, 
                clrType,
                typeof(TEntity)
            ) as ReferenceNavigationBuilder<TEntity, TRelatedEntity>;

    }
}