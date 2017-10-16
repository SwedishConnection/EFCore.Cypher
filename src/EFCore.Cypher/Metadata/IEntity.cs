

using System.Collections.Generic;

namespace Microsoft.EntityFrameworkCore.Metadata
{
    public interface IEntity: INode {        
        /// <summary>
        /// TODO: Do we want to expose relationships?  Even paths?
        /// 
        /// Relationships (both inbound and outbound)
        /// </summary>
        /// <returns></returns>
        IEnumerable<IRelationship> GetRelationships();
    }
}