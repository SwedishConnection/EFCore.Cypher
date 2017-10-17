using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal
{
    public class OnEntityAddedNode: GraphConventionNode {

        public OnEntityAddedNode(InternalEntityBuilder builder) {
            EntityBuilder = builder;
        }

        public InternalEntityBuilder EntityBuilder { get; }

        public override GraphConventionNode Accept(GraphConventionVisitor visitor) => visitor.VisitOnEntityAdded(this);
    }
}