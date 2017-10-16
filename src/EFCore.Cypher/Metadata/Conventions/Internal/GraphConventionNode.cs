namespace Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal
{
    public abstract class GraphConventionNode {
        public abstract GraphConventionNode Accept(GraphConventionVisitor visitor);
    }
}