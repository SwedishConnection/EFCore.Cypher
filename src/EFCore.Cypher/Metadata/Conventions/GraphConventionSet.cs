using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;

namespace Microsoft.EntityFrameworkCore.Metadata.Conventions
{
    /// <summary>
    /// See https://msdn.microsoft.com/en-us/library/jj679962(v=vs.113).aspx
    /// for information about conventions
    /// </summary>
    public class GraphConventionSet {
        public virtual IList<IEntityAddedConvention> EntityAddedConventions { get; } = new List<IEntityAddedConvention>();
    }
}