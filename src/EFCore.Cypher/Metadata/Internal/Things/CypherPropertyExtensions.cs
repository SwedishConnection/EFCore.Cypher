// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    public static class CypherPropertyExtensions {
        /// <summary>
        /// Get index (concrete)
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static int GetIndex([NotNull] this CypherProperty property)
            => property.GetPropertyIndexes().Index;

        /// <summary>
        /// Property indexes (concrete)
        /// </summary>
        /// <param name="propertyBase"></param>
        /// <returns></returns>
        public static PropertyIndexes GetPropertyIndexes([NotNull] this CypherProperty propertyBase)
            => propertyBase.PropertyIndexes;

        /// <summary>
        /// Set property indexes (concrete)
        /// </summary>
        /// <param name="propertyBase"></param>
        /// <param name="indexes"></param>
        public static void SetIndexes([NotNull] this CypherProperty propertyBase, [CanBeNull] PropertyIndexes indexes)
            => propertyBase.PropertyIndexes = indexes;

        /// <summary>
        /// Get property accessors (concrete)
        /// </summary>
        /// <param name="propertyBase"></param>
        /// <returns></returns>
        public static PropertyAccessors GetPropertyAccessors([NotNull] this CypherProperty propertyBase)
            => propertyBase.Accessors;

        /// <summary>
        /// Get getter (concrete)
        /// </summary>
        /// <param name="propertyBase"></param>
        /// <returns></returns>
        public static IClrPropertyGetter GetGetter([NotNull] this CypherProperty propertyBase)
            => propertyBase.Getter;

        /// <summary>
        /// Get setter (concrete)
        /// </summary>
        /// <param name="propertyBase"></param>
        /// <returns></returns>
        public static IClrPropertySetter GetSetter([NotNull] this CypherProperty propertyBase)
            => propertyBase.Setter;
    }
}