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
    public static class CypherReferenceReferenceBuilderExtensions
    {
        /// <summary>
        /// Relationship by name without properties
        /// </summary>
        /// <param name="referenceReferenceBuilder"></param>
        /// <param name="name"></param>
        /// <param name="startingClrType"></param>
        /// <returns></returns>
        public static ReferenceReferenceBuilder HasRelationship(
            [NotNull] this ReferenceReferenceBuilder referenceReferenceBuilder,
            [CanBeNull] string name,
            [CanBeNull] Type startingClrType
        )
        {
            Check.NotNull(referenceReferenceBuilder, nameof(referenceReferenceBuilder));
            Check.NullButNotEmpty(name, nameof(name));

            referenceReferenceBuilder.GetInfrastructure<InternalRelationshipBuilder>()
                .Cypher(ConfigurationSource.Explicit)
                .HasRelationship(name, startingClrType);

            return referenceReferenceBuilder;
        }

        /// <summary>
        /// Relationship by name without properties
        /// </summary>
        /// <param name="referenceReferenceBuilder"></param>
        /// <param name="name"></param>
        /// <param name="startingName"></param>
        /// <returns></returns>
        public static ReferenceReferenceBuilder HasRelationship(
            [NotNull] this ReferenceReferenceBuilder referenceReferenceBuilder,
            [CanBeNull] string name,
            [CanBeNull] string startingName
        ) {
            Check.NotNull(referenceReferenceBuilder, nameof(referenceReferenceBuilder));
            Check.NullButNotEmpty(name, nameof(name));

            referenceReferenceBuilder.GetInfrastructure<InternalRelationshipBuilder>()
                .Cypher(ConfigurationSource.Explicit)
                .HasRelationship(name, startingName);

            return referenceReferenceBuilder;
        }


        /// <summary>
        /// Relationship by name without properties where the declaring entity
        /// is the start of the relationship
        /// </summary>
        /// <param name="referenceReferenceBuilder"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ReferenceReferenceBuilder<TEntity, TRelatedEntity> HasRelationship<TEntity, TRelatedEntity>(
            [NotNull] this ReferenceReferenceBuilder<TEntity, TRelatedEntity> referenceReferenceBuilder,
            [CanBeNull] string name)
            where TEntity : class
            where TRelatedEntity : class
            => HasRelationship(
                (ReferenceReferenceBuilder)referenceReferenceBuilder, 
                name,
                typeof(TEntity)
            ) as ReferenceReferenceBuilder<TEntity, TRelatedEntity>;

        /// <summary>
        /// Relationship by Clr type
        /// </summary>
        /// <param name="referenceReferenceBuilder"></param>
        /// <param name="clrType"></param>
        /// <param name="startingClrType"></param>
        /// <returns></returns>
        public static ReferenceReferenceBuilder HasRelationship(
            [NotNull] this ReferenceReferenceBuilder referenceReferenceBuilder,
            [CanBeNull] Type clrType,
            [CanBeNull] Type startingClrType
        )
        {
            Check.NotNull(referenceReferenceBuilder, nameof(referenceReferenceBuilder));

            referenceReferenceBuilder.GetInfrastructure<InternalRelationshipBuilder>()
                .Cypher(ConfigurationSource.Explicit)
                .HasRelationship(clrType, startingClrType);

            return referenceReferenceBuilder;
        }

        /// <summary>
        /// Relationship by Clr type
        /// </summary>
        /// <param name="referenceReferenceBuilder"></param>
        /// <param name="clrType"></param>
        /// <param name="startingName"></param>
        /// <returns></returns>
        public static ReferenceReferenceBuilder HasRelationship(
            [NotNull] this ReferenceReferenceBuilder referenceReferenceBuilder,
            [CanBeNull] Type clrType,
            [CanBeNull] string startingName
        )
        {
            Check.NotNull(referenceReferenceBuilder, nameof(referenceReferenceBuilder));

            referenceReferenceBuilder.GetInfrastructure<InternalRelationshipBuilder>()
                .Cypher(ConfigurationSource.Explicit)
                .HasRelationship(clrType, startingName);

            return referenceReferenceBuilder;
        }

        /// <summary>
        /// Relationship by Clr type where the declaring type
        /// is the start of the relationship
        /// </summary>
        /// <param name="referenceReferenceBuilder"></param>
        /// <param name="clrType"></param>
        /// <returns></returns>
        public static ReferenceReferenceBuilder<TEntity, TRelatedEntity> HasRelationship<TEntity, TRelatedEntity>(
            [NotNull] this ReferenceReferenceBuilder<TEntity, TRelatedEntity> referenceReferenceBuilder,
            [CanBeNull] Type clrType)
            where TEntity : class
            where TRelatedEntity : class
            => HasRelationship(
                (ReferenceReferenceBuilder)referenceReferenceBuilder, 
                clrType,
                typeof(TEntity)
            ) as ReferenceReferenceBuilder<TEntity, TRelatedEntity>;
    }
}