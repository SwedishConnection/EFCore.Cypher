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
    public static class CypherReferenceOwnershipBuilderExtensions
    {
        /// <summary>
        /// Relationship by name without properties
        /// </summary>
        /// <param name="referenceOwnershipBuilder"></param>
        /// <param name="name"></param>
        /// <param name="startingClrType"></param>
        /// <returns></returns>
        public static ReferenceOwnershipBuilder HasRelationship(
            [NotNull] this ReferenceOwnershipBuilder referenceOwnershipBuilder,
            [CanBeNull] string name,
            [CanBeNull] Type startingClrType
        )
        {
            Check.NotNull(referenceOwnershipBuilder, nameof(referenceOwnershipBuilder));
            Check.NullButNotEmpty(name, nameof(name));

            referenceOwnershipBuilder.GetInfrastructure<InternalRelationshipBuilder>()
                .Cypher(ConfigurationSource.Explicit)
                .HasRelationship(name, startingClrType);

            return referenceOwnershipBuilder;
        }

        /// <summary>
        /// Relationship by name without properties
        /// </summary>
        /// <param name="referenceOwnershipBuilder"></param>
        /// <param name="name"></param>
        /// <param name="startingName"></param>
        /// <returns></returns>
        public static ReferenceOwnershipBuilder HasRelationship(
            [NotNull] this ReferenceOwnershipBuilder referenceOwnershipBuilder,
            [CanBeNull] string name,
            [CanBeNull] string startingName
        ) {
            Check.NotNull(referenceOwnershipBuilder, nameof(referenceOwnershipBuilder));
            Check.NullButNotEmpty(name, nameof(name));

            referenceOwnershipBuilder.GetInfrastructure<InternalRelationshipBuilder>()
                .Cypher(ConfigurationSource.Explicit)
                .HasRelationship(name, startingName);

            return referenceOwnershipBuilder;
        }

        /// <summary>
        /// Relationship by name without properties where the declaring entity
        /// is the start of the relationship
        /// </summary>
        /// <param name="referenceOwnershipBuilder"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ReferenceOwnershipBuilder<TEntity, TRelatedEntity> HasRelationship<TEntity, TRelatedEntity>(
            [NotNull] this ReferenceOwnershipBuilder<TEntity, TRelatedEntity> referenceOwnershipBuilder,
            [CanBeNull] string name)
            where TEntity : class
            where TRelatedEntity : class
            => HasRelationship(
                (ReferenceOwnershipBuilder)referenceOwnershipBuilder, 
                name,
                typeof(TEntity)
            ) as ReferenceOwnershipBuilder<TEntity, TRelatedEntity>;

        /// <summary>
        /// Relationship by Clr type
        /// </summary>
        /// <param name="referenceOwnershipBuilder"></param>
        /// <param name="clrType"></param>
        /// <param name="startingClrType"></param>
        /// <returns></returns>
        public static ReferenceOwnershipBuilder HasRelationship(
            [NotNull] this ReferenceOwnershipBuilder referenceOwnershipBuilder,
            [CanBeNull] Type clrType,
            [CanBeNull] Type startingClrType
        )
        {
            Check.NotNull(referenceOwnershipBuilder, nameof(referenceOwnershipBuilder));

            referenceOwnershipBuilder.GetInfrastructure<InternalRelationshipBuilder>()
                .Cypher(ConfigurationSource.Explicit)
                .HasRelationship(clrType, startingClrType);

            return referenceOwnershipBuilder;
        }

        /// <summary>
        /// Relationship by Clr type
        /// </summary>
        /// <param name="referenceOwnershipBuilder"></param>
        /// <param name="clrType"></param>
        /// <param name="startingName"></param>
        /// <returns></returns>
        public static ReferenceOwnershipBuilder HasRelationship(
            [NotNull] this ReferenceOwnershipBuilder referenceOwnershipBuilder,
            [CanBeNull] Type clrType,
            [CanBeNull] string startingName
        )
        {
            Check.NotNull(referenceOwnershipBuilder, nameof(referenceOwnershipBuilder));

            referenceOwnershipBuilder.GetInfrastructure<InternalRelationshipBuilder>()
                .Cypher(ConfigurationSource.Explicit)
                .HasRelationship(clrType, startingName);

            return referenceOwnershipBuilder;
        }

        /// <summary>
        /// Relationship by Clr type where the declaring type
        /// is the start of the relationship
        /// </summary>
        /// <param name="referenceOwnershipBuilder"></param>
        /// <param name="clrType"></param>
        /// <returns></returns>
        public static ReferenceOwnershipBuilder<TEntity, TRelatedEntity> HasRelationship<TEntity, TRelatedEntity>(
            [NotNull] this ReferenceOwnershipBuilder<TEntity, TRelatedEntity> referenceOwnershipBuilder,
            [CanBeNull] Type clrType)
            where TEntity : class
            where TRelatedEntity : class
            => HasRelationship(
                (ReferenceOwnershipBuilder)referenceOwnershipBuilder, 
                clrType,
                typeof(TEntity)
            ) as ReferenceOwnershipBuilder<TEntity, TRelatedEntity>;
    }
}