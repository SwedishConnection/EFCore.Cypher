// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    /// <summary>
    /// Relationship with a starting and ending entity
    /// </summary>
    public class CypherRelationship: ICypherRelationship {
        private readonly IEntityType _relation;

        private readonly IEntityType _starting;

        private readonly IEntityType _ending;

        public CypherRelationship(
            [NotNull] IEntityType relation,
            [NotNull] IEntityType starting,
            [NotNull] IEntityType ending
        ) {
            _relation = relation;
            _starting = starting;
            _ending = ending;
        }

        /// <summary>
        /// Relationship (edge)
        /// </summary>
        /// <returns></returns>
        public IEntityType Relation { get { return _relation; } }

        /// <summary>
        /// Start of a relationship
        /// </summary>
        /// <returns></returns>
        public IEntityType Starting { get { return _starting; } }

        /// <summary>
        /// End of a relationship
        /// </summary>
        /// <returns></returns>
        public IEntityType Ending { get { return _ending; } }
    }
}