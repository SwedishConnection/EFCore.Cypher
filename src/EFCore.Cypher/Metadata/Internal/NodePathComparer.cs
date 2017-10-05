using System.Collections.Generic;
using System.Linq;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    public class NodePathComparer : IComparer<INode>
    {
        public static readonly NodePathComparer Instance = new NodePathComparer();

        public int Compare(INode x, INode y)
        {
            x.Labels.OrderByDescending(l => l).SequenceEqual(y.Labels.OrderByDescending(l => l));
            
            throw new System.NotImplementedException();
        }
    }
}