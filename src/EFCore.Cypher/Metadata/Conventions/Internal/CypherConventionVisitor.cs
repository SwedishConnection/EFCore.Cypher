// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal
{
    public partial class CypherConventionDispatcher {
        private abstract class CypherConventionVisitor
        {
            /// <summary>
            /// Generic Visit calling the node's Accept
            /// </summary>
            /// <param name="node"></param>
            /// <returns></returns>
            public virtual CypherConventionNode Visit(CypherConventionNode node) => node?.Accept(this);

            public virtual CypherConventionScope VisitCypherConventionScope(CypherConventionScope node)
            {
                List<CypherConventionNode> visitations = null;

                foreach(var child in node.Children) {
                    var visited = Visit(child);
                    if (visited == null) {
                        continue;
                    }

                    if (visitations == null) {
                        visitations = new List<CypherConventionNode>();
                    }

                    visitations.Add(visited);
                }

                return (visitations?.Count ?? 0) == 0
                    ? null
                    : new CypherConventionScope(node.Parent, visitations);
            }

            public virtual OnEntityAddedNode VisitOnEntityAdded(OnEntityAddedNode node) => node;

            public virtual OnEntityIgnoredNode VisitOnEntityIgnored(OnEntityIgnoredNode node) => node;

            public virtual OnBaseEntityChangedNode VisitOnBaseEntityChanged(OnBaseEntityChangedNode node) => node;
        }

        private class RunVisitor: CypherConventionVisitor {
            public RunVisitor(CypherConventionDispatcher dispatcher) {
                Dispatcher = dispatcher;   
            }

            private CypherConventionDispatcher Dispatcher { get; }

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