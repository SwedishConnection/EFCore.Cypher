using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    public class InternalRelationshipBuilder: InternalNodeBuilder {
        public InternalRelationshipBuilder(
            [NotNull] Relationship metadata, 
            [NotNull] InternalGraphBuilder graphBuilder
        ) : base(metadata, graphBuilder) {
        }
    }
}