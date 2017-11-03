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
        /// Configure relationship name
        /// </summary>
        /// <param name="referenceReferenceBuilder"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ReferenceReferenceBuilder HasRelationship(
            [NotNull] this ReferenceReferenceBuilder referenceReferenceBuilder,
            [CanBeNull] string name
        )
        {
            Check.NotNull(referenceReferenceBuilder, nameof(referenceReferenceBuilder));
            Check.NullButNotEmpty(name, nameof(name));

            referenceReferenceBuilder.GetInfrastructure<InternalRelationshipBuilder>()
                .Cypher(ConfigurationSource.Explicit)
                .HasRelationship(name);

            return referenceReferenceBuilder;
        }

        /// <summary>
        /// Configure relationship name
        /// </summary>
        /// <param name="ReferenceReferenceBuilder<TEntity"></param>
        /// <param name="referenceReferenceBuilder"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ReferenceReferenceBuilder<TEntity, TRelatedEntity> HasRelationship<TEntity, TRelatedEntity>(
            [NotNull] this ReferenceReferenceBuilder<TEntity, TRelatedEntity> referenceReferenceBuilder,
            [CanBeNull] string name)
            where TEntity : class
            where TRelatedEntity : class
            => (ReferenceReferenceBuilder<TEntity, TRelatedEntity>)HasRelationship(
                (ReferenceReferenceBuilder)referenceReferenceBuilder, name);

        /// <summary>
        /// Configure relationship with Clr type
        /// </summary>
        /// <param name="referenceReferenceBuilder"></param>
        /// <param name="clrType"></param>
        /// <returns></returns>
        public static ReferenceReferenceBuilder HasRelationship(
            [NotNull] this ReferenceReferenceBuilder referenceReferenceBuilder,
            [CanBeNull] Type clrType
        )
        {
            Check.NotNull(referenceReferenceBuilder, nameof(referenceReferenceBuilder));
            Check.NotNull(clrType, nameof(clrType));

            referenceReferenceBuilder.GetInfrastructure<InternalRelationshipBuilder>()
                .Cypher(ConfigurationSource.Explicit)
                .HasRelationship(clrType);

            return referenceReferenceBuilder;
        }

        /// <summary>
        /// Configure relationship with Clr type
        /// </summary>
        /// <param name="ReferenceReferenceBuilder<TEntity"></param>
        /// <param name="referenceReferenceBuilder"></param>
        /// <param name="clrType"></param>
        /// <returns></returns>
        public static ReferenceReferenceBuilder<TEntity, TRelatedEntity> HasRelationship<TEntity, TRelatedEntity>(
            [NotNull] this ReferenceReferenceBuilder<TEntity, TRelatedEntity> referenceReferenceBuilder,
            [CanBeNull] Type clrType)
            where TEntity : class
            where TRelatedEntity : class
            => (ReferenceReferenceBuilder<TEntity, TRelatedEntity>)HasRelationship(
                (ReferenceReferenceBuilder)referenceReferenceBuilder, clrType);
    }
}