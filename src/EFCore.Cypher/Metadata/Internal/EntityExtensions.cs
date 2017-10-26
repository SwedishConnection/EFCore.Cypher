// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Internal;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    public static class EntityExtensions {

        /// <summary>
        /// Assert (i.e. throw if necessary) that the entity can be removed
        /// </summary>
        /// <param name="entity"></param>
        public static void AssertCanRemove(this Entity entity) {
            // TODO: relationships

            var derived = entity.GetDirectlyDerivedTypes().FirstOrDefault();
            if (derived != null)
            {
                throw new InvalidOperationException(
                    CoreStrings.EntityTypeInUseByDerived(
                        entity.DisplayName(),
                        derived.DisplayName()
                    )
                );
            }
        }

        /// <summary>
        /// Does entity inherit from the other entity
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static bool InheritsFrom(this Entity entity, Entity other) {
            var curr = entity;

            do {
                if (other == curr) {
                    return true;
                }
            }
            while (!((curr = curr.BaseType) is null));

            return false;
        }

        /// <summary>
        /// Dervied entities
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static IEnumerable<Entity> GetDerivedTypes(this Entity entity)
        {
            var derivedTypes = new List<Entity>();
            var type = entity;
            var currentTypeIndex = 0;

            while (type != null)
            {
                derivedTypes.AddRange(type.GetDirectlyDerivedTypes());
                type = derivedTypes.Count > currentTypeIndex
                    ? derivedTypes[currentTypeIndex]
                    : null;

                currentTypeIndex++;
            }

            return derivedTypes;
        }
    }
}