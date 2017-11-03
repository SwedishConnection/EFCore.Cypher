// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Metadata
{
    public class CypherForeignKeyAnnotations : ICypherForeignKeyAnnotations
    {
        public CypherForeignKeyAnnotations(
            [NotNull] IForeignKey foreignKey
        ) : this(new CypherAnnotations(foreignKey)) {
        }

        protected CypherForeignKeyAnnotations(
            [NotNull] CypherAnnotations annotations
        ) => Annotations = annotations;

        /// <summary>
        /// Annotations
        /// </summary>
        /// <returns></returns>
        protected virtual CypherAnnotations Annotations { get; }

        /// <summary>
        /// Wrapped foreign key
        /// </summary>
        /// <returns></returns>
        protected virtual IForeignKey ForeignKey => (IForeignKey)Annotations.Metadata;

        /// <summary>
        /// Relationship (yes it is an entity type)
        /// </summary>
        /// <returns></returns>
        public virtual EntityType Relationship {
            get => (EntityType)Annotations.Metadata[CypherAnnotationNames.Relationship]
                ?? null;

            [param: CanBeNull]
            set => SetRelationship(value);
        }

        /// <summary>
        /// Set relationship
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        protected virtual bool SetRelationship([CanBeNull] EntityType entityType) 
            => Annotations.SetAnnotation(
                CypherAnnotationNames.Relationship,
                entityType
            );
    }
}