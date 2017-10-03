
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Microsoft.EntityFrameworkCore.Storage
{
    public abstract class CypherTypeMapper : ICypherTypeMapper
    {

        /// <summary>
        /// Mappings from Net types to database types
        /// </summary>
        /// <returns></returns>
        protected abstract IReadOnlyDictionary<Type, GraphTypeMapping> GetClrTypeMappings();

        /// <summary>
        /// Mappings from database types to Net types
        /// </summary>
        /// <returns></returns>
        protected abstract IReadOnlyDictionary<string, GraphTypeMapping> GetStoreTypeMappings();

        /// <summary>
        /// Is the Net type mapped to a database type
        /// </summary>
        /// <param name="clrType"></param>
        /// <returns></returns>
        public bool IsTypeMapped(Type clrType)
        {
            // TODO: Check clrType not null

            // TODO: 
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get database type for a node property
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        public virtual GraphTypeMapping FindMapping(INodeProperty prop) {
            // TODO: Check prop not null

            // Has annotation PropertyType defined for a database type
            var storeType = GetPropertyType(prop);

            if (storeType == null) {
                return FindMapping(prop.ClrType);
            } else {
                var mapping = FindMapping(storeType);

                if (mapping != null) {
                    return mapping;
                } else {
                    return EscapeHatchFindMapping(prop);
                }
            }
        }

        /// <summary>
        /// Get database type for Net type
        /// </summary>
        /// <param name="clrType"></param>
        /// <returns></returns>
        public virtual GraphTypeMapping FindMapping(Type clrType) {
            // TODO: Check clrType not null

            return GetClrTypeMappings().TryGetValue(clrType.UnwrapNullableType().UnwrapEnumType(), out var mapping)
                ? mapping
                : null;
        }

        /// <summary>
        /// Get database type for a specfic database type
        /// </summary>
        /// <param name="storeType"></param>
        /// <returns></returns>
        public virtual GraphTypeMapping FindMapping(string storeType) {
            // TODO: Check storeType not null

            GetStoreTypeMappings().TryGetValue(storeType, out var mapping);
            return mapping;
        }

        /// <summary>
        /// When nothing else works pop the escape hatch to find a mapping by property
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        protected abstract GraphTypeMapping EscapeHatchFindMapping([NotNull] INodeProperty prop);

        /// <summary>
        /// Get property type for the node property
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        protected virtual string GetPropertyType([NotNull] INodeProperty prop) => (string)prop[GraphAnnotationNames.PropertyType];
    }
}