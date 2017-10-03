using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Microsoft.EntityFrameworkCore.Metadata
{
    public interface IGraph: IAnnotatable {

        IEnumerable<INode> GetNodes();

        INode FindNode([NotNull] string[] labels);
    }
}