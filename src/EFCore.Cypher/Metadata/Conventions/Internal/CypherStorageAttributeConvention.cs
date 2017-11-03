// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal
{
    /// <summary>
    ///     This API supports the Entity Framework Core infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class CypherStorageAttributeConvention : PropertyAttributeConvention<StorageAttribute>
    {
        public override InternalPropertyBuilder Apply(
            InternalPropertyBuilder propertyBuilder, StorageAttribute attribute, MemberInfo clrMember)
        {
            if (!string.IsNullOrWhiteSpace(attribute.Name))
            {
                propertyBuilder
                    .Cypher(ConfigurationSource.DataAnnotation)
                    .HasStorageName(attribute.Name);
            }

            if (!string.IsNullOrWhiteSpace(attribute.TypeName))
            {
                propertyBuilder
                    .Cypher(ConfigurationSource.DataAnnotation)
                    .HasStorageType(attribute.TypeName);
            }

            return propertyBuilder;
        }
    }
}