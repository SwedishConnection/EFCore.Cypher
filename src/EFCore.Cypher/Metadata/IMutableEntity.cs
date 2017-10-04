using System.Collections.Generic;

namespace Microsoft.EntityFrameworkCore.Metadata
{
    public interface IMutableEntity: IEntity, IMutableNode {

        /// <summary>
        /// Relationships (both inbound and outbound)
        /// </summary>
        /// <returns></returns>
        new IEnumerable<IMutableRelationship> GetRelationships();
    }
}