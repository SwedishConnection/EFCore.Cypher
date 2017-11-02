// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore
{
    public static class CypherMetadataExtensions
    {
        /// <summary>
        /// Cypher specific metadata for an entity
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        public static CypherEntityTypeAnnotations Cypher(
            [NotNull] this IMutableEntityType entityType
        ) => (CypherEntityTypeAnnotations)Cypher((IEntityType)entityType);

        /// <summary>
        /// Cypher specific metadata for an entity
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        public static ICypherEntityTypeAnnotations Cypher(
            [NotNull] this IEntityType entityType
        ) => new CypherEntityTypeAnnotations(Check.NotNull(entityType, nameof(entityType)));
    }
}