// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    public static class CypherEntityExtensions {

        /// <summary>
        /// Throw if necessary that the entity can be removed (originally an instance member)
        /// </summary>
        /// <param name="entity"></param>
        public static void AssertCanRemove(this CypherEntity entity) {
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
        /// Does entity inherit from another entity (originally an instance member)
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static bool InheritsFrom(this CypherEntity entity, CypherEntity other) {
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
        /// Find defining navigation (concrete)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static CypherNavigation FindDefiningNavigation([NotNull] this CypherEntity entity)
            => (CypherNavigation)((IEntityType)entity).FindDefiningNavigation();

        /// <summary>
        /// Find in definitipm path (concrete)
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public static CypherEntity FindInDefinitionPath(
            [NotNull] this CypherEntity entity, 
            [NotNull] Type targetType
        ) => (CypherEntity)((IEntityType)entity).FindInDefinitionPath(targetType);

        /// <summary>
        /// Least derived type (concrete)
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static CypherEntity LeastDerivedType([NotNull] this CypherEntity entity, [NotNull] CypherEntity other)
            => (CypherEntity)((IEntityType)entity).LeastDerivedType(other);

        /// <summary>
        /// Property counts (concrete)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static PropertyCounts GetCounts([NotNull] this CypherEntity entity)
            => entity.Counts;

        /// <summary>
        /// Declared keys (concrete)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static IEnumerable<IKey> GetDeclaredKeys([NotNull] this CypherEntity entity)
            => entity.GetDeclaredKeys();

        /// <summary>
        /// Declared foreign keys (concrete)
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static IEnumerable<IForeignKey> GetDeclaredForeignKeys([NotNull] this CypherEntity entity)
            => entity.GetDeclaredForeignKeys();
    }
}