

using System.Collections.Generic;

namespace Microsoft.EntityFrameworkCore.Metadata
{
    public interface IEntity: INode {
        /// <summary>
        /// Primary (unique) key
        /// </summary>
        /// <returns></returns>
        IKey FindPrimaryKey();

        /// <summary>
        /// Geys the primary and alternate keys (e.g. asserts etc.)
        /// </summary>
        /// <returns></returns>
        IEnumerable<IKey> GetKeys();

        
        /// <summary>
        /// Relationships (both inbound and outbound)
        /// </summary>
        /// <returns></returns>
        IEnumerable<IRelationship> GetRelationships();
    }
}