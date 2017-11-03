// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
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

            var startProperty = FindRelationshipAttributeOnProperty(
                fk.PrincipalEntityType, 
                fk.PrincipalToDependent?.Name
            );

            return relationshipBuilder;
        }

        private string FindRelationshipAttributeOnProperty(EntityType entityType, string navigationName)
        {
            if (string.IsNullOrWhiteSpace(navigationName) || !entityType.HasClrType()) {
                return null;
            }

            string candidateProperty = null;

            foreach (var mi in entityType
                .ClrType
                .GetRuntimeProperties()
                .Cast<MemberInfo>()
                .Concat(entityType.ClrType.GetRuntimeFields())) {
                if (mi is PropertyInfo pi && FindCandidateNavigationPropertyType(pi) != null) {
                    continue;
                }

                var attr = mi.GetCustomAttribute<RelationshipAttribute>(true);

                if (attr != null) {
                    candidateProperty = mi.Name;
                }
            }

            if (!(candidateProperty is null)) {
                var attr = GetRelationshipAttribute(entityType, navigationName);
                // TODO: Finish off
            }

            return null;
        }  

        public virtual Type FindCandidateNavigationPropertyType([NotNull] PropertyInfo pi)
        {
            Check.NotNull(pi, nameof(pi));

            return pi.FindCandidateNavigationPropertyType(_typeMapper.IsTypeMapped);
        }

        private static RelationshipAttribute GetRelationshipAttribute(TypeBase entityType, string propertyName)
        {
            return entityType.ClrType?.GetRuntimeProperties()
                .FirstOrDefault(p => string.Equals(p.Name, propertyName, StringComparison.OrdinalIgnoreCase))
                ?.GetCustomAttribute<RelationshipAttribute>(true);
        }
    }
}