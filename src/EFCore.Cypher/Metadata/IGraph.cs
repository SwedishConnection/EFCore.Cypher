using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Microsoft.EntityFrameworkCore.Metadata
{
    public interface IGraph: IAnnotatable {

        /// <summary>
        /// Entities
        /// </summary>
        /// <returns></returns>
        IEnumerable<IEntity> GetEntities();

        /// <summary>
        /// Entities by labels
        /// </summary>
        /// <param name="labels"></param>
        /// <returns></returns>
        IEntity FindEntity([NotNull] string[] labels);

        /// <summary>
        /// Relationships
        /// </summary>
        /// <returns></returns>
        IEnumerable<IRelationship> GetRelationships();

        /// <summary>
        /// Relationships by labels
        /// </summary>
        /// <param name="labels"></param>
        /// <returns></returns>
        IRelationship FindRelationship([NotNull] string[] labels);

        // TODO: Entities through navigation
    }
}