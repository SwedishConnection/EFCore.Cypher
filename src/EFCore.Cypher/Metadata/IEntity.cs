

using System.Collections.Generic;

namespace Microsoft.EntityFrameworkCore.Metadata
{
    public interface IEntity: INode {
        /// <summary>
        /// Entities derive only from entities
        /// </summary>
        /// <returns></returns>
        new IEntity BaseType { get; }

        /// <summary>
        /// Defining navigation name 
        /// </summary>
        /// <returns></returns>
        string DefiningNavigationName { get; }

        /// <summary>
        /// Defining navigation type (note: builder will hold the path either defined or unnamed variable length)
        /// </summary>
        /// <returns></returns>
        IEntity DefiningType { get; }

        /// <summary>
        /// Relationships (both inbound and outbound)
        /// </summary>
        /// <returns></returns>
        IEnumerable<IRelationship> GetRelationships();
    }
}