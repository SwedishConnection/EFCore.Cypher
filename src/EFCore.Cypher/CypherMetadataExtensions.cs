// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    /// Tacks on a Cypher extension to metadata (e.g. models, entity types etc.)
    /// </summary>
    public static class CypherMetadataExtensions
    {
        /// <summary>
        /// Cypher specific metadata for a mutable entity
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        public static CypherEntityTypeAnnotations Cypher(
            [NotNull] this IMutableEntityType entityType
        ) => (CypherEntityTypeAnnotations)Cypher((IEntityType)entityType);

        /// <summary>
        /// Cypher specific metadata for a read-only entity
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        public static ICypherEntityTypeAnnotations Cypher(
            [NotNull] this IEntityType entityType
        ) => new CypherEntityTypeAnnotations(Check.NotNull(entityType, nameof(entityType)));

        /// <summary>
        /// Cypher specific metata for a mutable property
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static CypherPropertyAnnotations Cypher(
            [NotNull] this IMutableProperty property
        ) => (CypherPropertyAnnotations)Cypher((IProperty)property);

        /// <summary>
        /// Cypher specific metata for a read-only property
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static CypherPropertyAnnotations Cypher(
            [NotNull] this IProperty property
        ) => new CypherPropertyAnnotations(Check.NotNull(property, nameof(property)));
    }
}