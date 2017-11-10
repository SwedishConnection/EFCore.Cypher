// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Internal;
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
        public virtual ICypherRelationship Relationship {
            get => (CypherRelationship)Annotations.Metadata[CypherAnnotationNames.Relationship]
                ?? null;

            [param: CanBeNull]
            set => SetRelationship(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="relationship"></param>
        /// <returns></returns>
        private bool SetRelationship(
            [CanBeNull] ICypherRelationship relationship
        ) {
            // TODO: Should a relation be segregated from the rest of the entity type pool?
            AssertMember(relationship.Starting);
            AssertMember(relationship.Ending);

            return Annotations.SetAnnotation(
                CypherAnnotationNames.Relationship,
                relationship
            );
        }


        /// <summary>
        /// Set relationship
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        protected virtual bool SetRelationship(
            [CanBeNull] EntityType entityType,
            [NotNull] EntityType starting
        ) => SetRelationship(
            new CypherRelationship(
                entityType,
                starting,
                ForeignKey.DeclaringEntityType == starting
                    ? ForeignKey.PrincipalEntityType
                    : ForeignKey.DeclaringEntityType
            )
        );

        /// <summary>
        /// Assert that entity is a foreign key member
        /// </summary>
        /// <param name="entityType"></param>
        private void AssertMember(IEntityType entityType) {
            if (entityType != ForeignKey.DeclaringEntityType && 
                entityType != ForeignKey.PrincipalEntityType) {
                throw new InvalidOperationException(
                    CypherStrings.NotAForeignKeyMember(
                        entityType.DisplayName(),
                        ForeignKey.DeclaringEntityType.DisplayName(),
                        ForeignKey.PrincipalEntityType.DisplayName()
                    )
                );
            }
        }
    }
}