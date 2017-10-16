using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    public abstract class InternalNodeBuilder : InternalBaseItemBuilder<Node>
    {
        public InternalNodeBuilder(
            [NotNull] Node node, 
            [NotNull] InternalGraphBuilder graphBuilder
        ) : base(node, graphBuilder)
        {
        }

    }
}