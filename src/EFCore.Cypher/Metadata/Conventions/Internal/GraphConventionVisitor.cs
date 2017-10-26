using System.Collections.Generic;

namespace Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal
{
    public partial class GraphConventionDispatcher {
        private abstract class GraphConventionVisitor
        {
            /// <summary>
            /// Generic Visit calling the node's Accept
            /// </summary>
            /// <param name="node"></param>
            /// <returns></returns>
            public virtual GraphConventionNode Visit(GraphConventionNode node) => node?.Accept(this);

            public virtual GraphConventionScope VisitGraphConventionScope(GraphConventionScope node)
            {
                List<GraphConventionNode> visitations = null;

                foreach(var child in node.Children) {
                    var visited = Visit(child);
                    if (visited == null) {
                        continue;
                    }

                    if (visitations == null) {
                        visitations = new List<GraphConventionNode>();
                    }

                    visitations.Add(visited);
                }

                return (visitations?.Count ?? 0) == 0
                    ? null
                    : new GraphConventionScope(node.Parent, visitations);
            }

            public virtual OnEntityAddedNode VisitOnEntityAdded(OnEntityAddedNode node) => node;

            public virtual OnEntityIgnoredNode VisitOnEntityIgnored(OnEntityIgnoredNode node) => node;

            public virtual OnBaseEntityChangedNode VisitOnBaseEntityChanged(OnBaseEntityChangedNode node) => node;
        }

        private class RunVisitor: GraphConventionVisitor {
            public RunVisitor(GraphConventionDispatcher dispatcher) {
                Dispatcher = dispatcher;   
            }

            private GraphConventionDispatcher Dispatcher { get; }

            public override OnEntityAddedNode VisitOnEntityAdded(OnEntityAddedNode node)
            {
                Dispatcher._immediateScope.OnEntityAdded(node.EntityBuilder);
                return null;
            }

            public override OnEntityIgnoredNode VisitOnEntityIgnored(OnEntityIgnoredNode node)
            {
                Dispatcher._immediateScope.OnEntityIgnored(node.GraphBuilder, node.Name, node.Type);
                return null;
            }

            public override OnBaseEntityChangedNode VisitOnBaseEntityChanged(OnBaseEntityChangedNode node)
            {
                Dispatcher._immediateScope.OnBaseEntityChanged(node.EntityBuilder, node.Previous);
                return null;
            }
        }
    }
}