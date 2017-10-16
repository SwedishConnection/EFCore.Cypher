using System.Collections.Generic;

namespace Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal
{
    public abstract class GraphConventionVisitor {
        public virtual GraphConventionNode Visit(GraphConventionNode node) => node?.Accept(this);

        public virtual GraphConventionScope VisitGraphConventionScope(GraphConventionScope node) {
            List<GraphConventionNode> children = null;

            foreach (var child in node.Children) {
                var visited = Visit(child);

                if (visited is null) {
                    continue;
                }

                if (children is null) {
                    children = new List<GraphConventionNode>();
                }

                children.Add(visited);
            }

            return (children?.Count ?? 0) == 0 
                ? null
                : new GraphConventionScope(node.Parent, children);
        }

        public virtual OnPropertyFieldChangedNode VisitOnPropertyFieldChanged(OnPropertyFieldChangedNode node) => node;
    }
}