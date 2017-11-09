// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal
{
    public class CypherRelationshipAttributeConvention:
        IForeignKeyAddedConvention 
    {
        private readonly ITypeMapper _typeMapper;

        public CypherRelationshipAttributeConvention([NotNull] ITypeMapper typeMapper) {
            _typeMapper = typeMapper;
        }

        public virtual InternalRelationshipBuilder Apply(InternalRelationshipBuilder relationshipBuilder)
        {
            Check.NotNull(relationshipBuilder, nameof(relationshipBuilder));
            
            var fk = relationshipBuilder.Metadata;

            var fromDeclaring = FindRelationshipAttributeOnProperty(
                fk.DeclaringEntityType, 
                fk.DependentToPrincipal?.Name
            );

            var fromPrincipal = FindRelationshipAttributeOnProperty(
                fk.PrincipalEntityType, 
                fk.PrincipalToDependent?.Name
            );

            if (!(fromDeclaring is null) && !(fromPrincipal is null)) {
                throw new InvalidOperationException(
                    CypherStrings.DuplicateRelationshipAttribute(
                        fk.DeclaringEntityType.DisplayName(), 
                        fk.DependentToPrincipal?.Name,
                        fk.PrincipalEntityType.DisplayName(),
                        fk.PrincipalToDependent?.Name
                    )
                );
            }
            
            if (!(fromDeclaring is null)) {
                relationshipBuilder
                    .Cypher(ConfigurationSource.DataAnnotation)
                    .HasRelationship(
                        fromDeclaring.Name ?? fromDeclaring.Type.DisplayName(),
                        fk.DeclaringEntityType.Name ?? fk.DeclaringEntityType.ClrType.DisplayName()
                    );
            }

            if (!(fromPrincipal is null)) {
                relationshipBuilder
                    .Cypher(ConfigurationSource.DataAnnotation)
                    .HasRelationship(
                        fromPrincipal.Name ?? fromPrincipal.Type.DisplayName(),
                        fk.PrincipalEntityType.Name ?? fk.PrincipalEntityType.ClrType.DisplayName()
                    );
            }

            return relationshipBuilder;
        }

        /// <summary>
        /// Candiate property 
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="navigationName"></param>
        /// <returns></returns>
        private RelationshipAttribute FindRelationshipAttributeOnProperty(EntityType entityType, string navigationName)
        {
            if (string.IsNullOrWhiteSpace(navigationName) || !entityType.HasClrType()) {
                return null;
            }

            var member = entityType
                .ClrType
                .GetRuntimeProperties()
                .Cast<MemberInfo>()
                .Concat(entityType.ClrType.GetRuntimeFields())
                .SingleOrDefault(mi => mi.Name == navigationName);

            return member?.GetCustomAttribute<RelationshipAttribute>(true);
        }  

    }
}