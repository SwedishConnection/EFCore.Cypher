// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.


using System.Collections.Generic;
using System.Linq;

namespace Microsoft.EntityFrameworkCore.Metadata
{
    public static class CypherEntityTypeExtensions {

        /// <summary>
        /// Is the entity type a relationship
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        public static bool IsRelationship(this IEntityType entityType) {
            return entityType.Model
                .Relationships()
                .Any(r => r.Relation == entityType);
        }
    }
}