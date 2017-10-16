using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    public class InternalNodePropertyBuilder: InternalBaseItemBuilder<NodeProperty> {

        public InternalNodePropertyBuilder(
            [NotNull] NodeProperty baze, 
            [NotNull] InternalGraphBuilder graphBuilder): base(baze, graphBuilder) {   
        }

    }
}