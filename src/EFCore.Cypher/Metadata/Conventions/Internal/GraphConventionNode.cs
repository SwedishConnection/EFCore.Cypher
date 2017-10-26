using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal
{
    public partial class GraphConventionDispatcher
    {
        /// <summary>
        /// Delayed nodes
        /// </summary>
        private abstract class GraphConventionNode
        {
            public abstract GraphConventionNode Accept(GraphConventionVisitor visitor);
        }

        /// <summary>
        /// Delayed scope
        /// </summary>
        private class GraphConventionScope: GraphConventionNode {
            private readonly List<GraphConventionNode> _children;

            private bool _readonly;

            public GraphConventionScope(GraphConventionScope parent, List<GraphConventionNode> children) {
                Parent = parent;
                _children = children ?? new List<GraphConventionNode>();
            }

            /// <summary>
            /// Parent scope
            /// </summary>
            /// <returns></returns>
            public GraphConventionScope Parent { [DebuggerStepThrough] get; }

            /// <summary>
            /// Delayed child nodes
            /// </summary>
            /// <returns></returns>
            public IReadOnlyList<GraphConventionNode> Children
            {
                [DebuggerStepThrough] get { return _children; }
            }

            /// <summary>
            /// Leaf count
            /// </summary>
            /// <returns></returns>
            public int GetLeafCount()
            {
                var scopesToVisit = new Queue<GraphConventionScope>();
                scopesToVisit.Enqueue(this);
                var leafCount = 0;
                while (scopesToVisit.Count > 0)
                {
                    var scope = scopesToVisit.Dequeue();
                    foreach (var conventionNode in scope.Children)
                    {
                        if (conventionNode is GraphConventionScope nextScope)
                        {
                            scopesToVisit.Enqueue(nextScope);
                        }
                        else
                        {
                            leafCount++;
                        }
                    }
                }

                return leafCount;
            }

            /// <summary>
            /// Force as readonly
            /// </summary>
            public void MakeReadonly() => _readonly = true;

            /// <summary>
            /// Add node
            /// </summary>
            /// <param name="node"></param>
            public void Add(GraphConventionNode node)
            {
                if (_readonly)
                {
                    throw new InvalidOperationException();
                }
                _children.Add(node);
            }

            /// <summary>
            /// Accept visitor
            /// </summary>
            /// <param name="visitor"></param>
            /// <returns></returns>
            public override GraphConventionNode Accept(GraphConventionVisitor visitor) => visitor.VisitGraphConventionScope(this);

            /// <summary>
            /// Entity added
            /// </summary>
            /// <param name="entityBuilder"></param>
            /// <returns></returns>
            public virtual InternalEntityBuilder OnEntityAdded([NotNull] InternalEntityBuilder builder) {
                Add(new OnEntityAddedNode(builder));
                return builder;
            }

            /// <summary>
            /// Entity ignored
            /// </summary>
            /// <param name="builder"></param>
            /// <param name="name"></param>
            /// <param name="type"></param>
            /// <returns></returns>
            public virtual bool OnEntityIgnored([NotNull] InternalGraphBuilder builder, [NotNull] string name, [CanBeNull] Type type)
            {
                Add(new OnEntityIgnoredNode(builder, name, type));
                return true;
            }

            /// <summary>
            /// Base entity changed
            /// </summary>
            /// <param name="builder"></param>
            /// <param name="previous"></param>
            /// <returns></returns>
            public virtual InternalEntityBuilder OnBaseEntityChanged([NotNull] InternalEntityBuilder builder, [CanBeNull] Entity previous) {
                Add(new OnBaseEntityChangedNode(builder, previous));
                return builder;
            }
        }

        /// <summary>
        /// Immediate scope
        /// </summary>
        private class GraphImmediateConventionScope : GraphConventionScope
        {
            private readonly GraphConventionSet _graphConventionSet;

            public GraphImmediateConventionScope([NotNull] GraphConventionSet graphConventionSet)
                : base(parent: null, children: null)
            {
                _graphConventionSet = graphConventionSet;
                MakeReadonly();
            }

            public InternalGraphBuilder OnGraphInitialized([NotNull] InternalGraphBuilder builder) {
                foreach (var convention in _graphConventionSet.GraphInitializedConventions) {
                    builder = convention.Apply(builder);

                    if (builder == null) {
                        break;
                    }
                }

                return builder;
            }

            /// <summary>
            /// When entity type added
            /// </summary>
            /// <param name="builder"></param>
            /// <returns></returns>
            public override InternalEntityBuilder OnEntityAdded(InternalEntityBuilder builder) {
                if (builder.Metadata.Builder == null)
                {
                    return null;
                }

                foreach (var convention in _graphConventionSet.EntityAddedConventions) {
                    builder = convention.Apply(builder);

                    if (builder?.Metadata.Builder == null) {
                        return null;
                    }
                }

                return builder;
            }
        }

        /// <summary>
        /// Delayed when entity type added
        /// </summary>
        private class OnEntityAddedNode : GraphConventionNode
        {
            public OnEntityAddedNode(InternalEntityBuilder entityBuilder)
            {
                EntityBuilder = entityBuilder;
            }

            public InternalEntityBuilder EntityBuilder { get; }

            public override GraphConventionNode Accept(GraphConventionVisitor visitor) => visitor.VisitOnEntityAdded(this);
        }

        private class OnEntityIgnoredNode : GraphConventionNode
        {
            public OnEntityIgnoredNode(InternalGraphBuilder builder, string name, Type type)
            {
                GraphBuilder = builder;
                Name = name;
                Type = type;
            }

            public InternalGraphBuilder GraphBuilder { get; }
            public string Name { get; }
            public Type Type { get; }

            public override GraphConventionNode Accept(GraphConventionVisitor visitor) => visitor.VisitOnEntityIgnored(this);
        }

        private class OnBaseEntityChangedNode : GraphConventionNode
        {
            public OnBaseEntityChangedNode(InternalEntityBuilder builder, Entity previous)
            {
                EntityBuilder = builder;
                Previous = previous;
            }

            public InternalEntityBuilder EntityBuilder { get; }
            public Entity Previous { get; }

            public override GraphConventionNode Accept(GraphConventionVisitor visitor) => visitor.VisitOnBaseEntityChanged(this);
        }
    }
}