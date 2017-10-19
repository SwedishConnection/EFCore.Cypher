using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Internal;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    public static class EntityExtensions {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static bool HasDefiningNavigation([NotNull] this IEntity entity)
            => entity.DefiningType != null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static IEnumerable<IEntity> GetDirectlyDerivedTypes([NotNull] this IEntity entity)
        {
            foreach (var derivedType in entity.Graph.GetEntities())
            {
                if (derivedType.BaseType == entity)
                {
                    yield return derivedType;
                }
            }
        }

        /// <summary>
        /// Actual dervived types
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Entity> GetDerivedTypes(this Entity entity)
        {
            var derivedTypes = new List<Entity>();

            var type = entity;
            var currentTypeIndex = 0;

            // Favorite spread from the EF code base!
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        public static void AssertCanRemove(this Entity entity) {
            // TODO: Check relationships

            var aDerivedEntity = entity.GetDirectlyDerivedTypes().FirstOrDefault();
            if (aDerivedEntity != null)
            {
                throw new InvalidOperationException(
                    CoreCypherStrings.EntityInUseByDerived(
                        entity.DisplayLabels(),
                        aDerivedEntity.DisplayLabels()));
            }
        }
    }
}