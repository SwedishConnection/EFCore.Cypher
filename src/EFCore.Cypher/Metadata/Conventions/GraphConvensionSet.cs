using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;

namespace Microsoft.EntityFrameworkCore.Metadata.Conventions
{
    public class GraphConventionSet {
        
        public virtual IList<IEntityAddedConvention> EntityAddedConventions { get; } = new List<IEntityAddedConvention>();
        
        public virtual IList<IGraphInitializedConvention> GraphInitializedConventions { get; } = new List<IGraphInitializedConvention>();

    }
}