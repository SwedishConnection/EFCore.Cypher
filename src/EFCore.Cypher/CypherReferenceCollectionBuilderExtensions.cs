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
    public static class CypherReferenceCollectionBuilderExtensions
    {
        /// <summary>
        /// Relationship by name without properties
        /// </summary>
        /// <param name="ReferenceCollectionBuilder"></param>
        /// <param name="name"></param>
        /// <param name="startingClrType"></param>
        /// <returns></returns>
        public static ReferenceCollectionBuilder HasRelationship(
            [NotNull] this ReferenceCollectionBuilder referenceCollectionBuilder,
            [CanBeNull] string name,
            [CanBeNull] Type startingClrType
        )
        {
            Check.NotNull(referenceCollectionBuilder, nameof(ReferenceCollectionBuilder));
            Check.NullButNotEmpty(name, nameof(name));

            referenceCollectionBuilder.GetInfrastructure<InternalRelationshipBuilder>()
                .Cypher(ConfigurationSource.Explicit)
                .HasRelationship(name, startingClrType);

            return referenceCollectionBuilder;
        }

        /// <summary>
        /// Relationship by name without properties
        /// </summary>
        /// <param name="ReferenceCollectionBuilder"></param>
        /// <param name="name"></param>
        /// <param name="startingName"></param>
        /// <returns></returns>
        public static ReferenceCollectionBuilder HasRelationship(
            [NotNull] this ReferenceCollectionBuilder referenceCollectionBuilder,
            [CanBeNull] string name,
            [CanBeNull] string startingName
        ) {
            Check.NotNull(referenceCollectionBuilder, nameof(referenceCollectionBuilder));
            Check.NullButNotEmpty(name, nameof(name));

            referenceCollectionBuilder.GetInfrastructure<InternalRelationshipBuilder>()
                .Cypher(ConfigurationSource.Explicit)
                .HasRelationship(name, startingName);

            return referenceCollectionBuilder;
        }

        /// <summary>
        /// Relationship by name without properties where the declaring entity
        /// is the start of the relationship
        /// </summary>
        /// <param name="ReferenceCollectionBuilder<TEntity"></param>
        /// <param name="ReferenceCollectionBuilder"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ReferenceCollectionBuilder<TEntity, TRelatedEntity> HasRelationship<TEntity, TRelatedEntity>(
            [NotNull] this ReferenceCollectionBuilder<TEntity, TRelatedEntity> referenceCollectionBuilder,
            [CanBeNull] string name)
            where TEntity : class
            where TRelatedEntity : class
            => HasRelationship(
                (ReferenceCollectionBuilder)referenceCollectionBuilder, 
                name,
                typeof(TEntity)
            ) as ReferenceCollectionBuilder<TEntity, TRelatedEntity>;

        /// <summary>
        /// Relationship by Clr type
        /// </summary>
        /// <param name="ReferenceCollectionBuilder"></param>
        /// <param name="clrType"></param>
        /// <param name="startingClrType"></param>
        /// <returns></returns>
        public static ReferenceCollectionBuilder HasRelationship(
            [NotNull] this ReferenceCollectionBuilder referenceCollectionBuilder,
            [CanBeNull] Type clrType,
            [CanBeNull] Type startingClrType
        )
        {
            Check.NotNull(referenceCollectionBuilder, nameof(referenceCollectionBuilder));

            referenceCollectionBuilder.GetInfrastructure<InternalRelationshipBuilder>()
                .Cypher(ConfigurationSource.Explicit)
                .HasRelationship(clrType, startingClrType);

            return referenceCollectionBuilder;
        }

        /// <summary>
        /// Relationship by Clr type
        /// </summary>
        /// <param name="referenceCollectionBuilder"></param>
        /// <param name="clrType"></param>
        /// <param name="startingName"></param>
        /// <returns></returns>
        public static ReferenceCollectionBuilder HasRelationship(
            [NotNull] this ReferenceCollectionBuilder referenceCollectionBuilder,
            [CanBeNull] Type clrType,
            [CanBeNull] string startingName
        )
        {
            Check.NotNull(referenceCollectionBuilder, nameof(referenceCollectionBuilder));

            referenceCollectionBuilder.GetInfrastructure<InternalRelationshipBuilder>()
                .Cypher(ConfigurationSource.Explicit)
                .HasRelationship(clrType, startingName);

            return referenceCollectionBuilder;
        }

        /// <summary>
        /// Relationship by Clr type where the declaring type
        /// is the start of the relationship
        /// </summary>
        /// <param name="referenceCollectionBuilder"</param>
        /// <param name="clrType"></param>
        /// <returns></returns>
        public static ReferenceCollectionBuilder<TEntity, TRelatedEntity> HasRelationship<TEntity, TRelatedEntity>(
            [NotNull] this ReferenceCollectionBuilder<TEntity, TRelatedEntity> referenceCollectionBuilder,
            [CanBeNull] Type clrType)
            where TEntity : class
            where TRelatedEntity : class
            => HasRelationship(
                (ReferenceCollectionBuilder)referenceCollectionBuilder, 
                clrType,
                typeof(TEntity)
            ) as ReferenceCollectionBuilder<TEntity, TRelatedEntity>;
    }
}