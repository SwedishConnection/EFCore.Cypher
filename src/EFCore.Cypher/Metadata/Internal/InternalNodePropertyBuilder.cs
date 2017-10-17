using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    public class InternalNodePropertyBuilder: InternalMetadataItemBuilder<NodeProperty> {

        public InternalNodePropertyBuilder(
            [NotNull] NodeProperty metadata, 
            [NotNull] InternalGraphBuilder graphBuilder): base(metadata, graphBuilder) {   
        }

    }
}