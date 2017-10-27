// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal
{
    public partial class CypherConventionDispatcher
    {
        /// <summary>
        /// Delayed nodes
        /// </summary>
        private abstract class CypherConventionNode
        {
            public abstract CypherConventionNode Accept(CypherConventionVisitor visitor);
        }

        /// <summary>
        /// Delayed scope
        /// </summary>
        private class CypherConventionScope: CypherConventionNode {
            private readonly List<CypherConventionNode> _children;

            private bool _readonly;

            public CypherConventionScope(CypherConventionScope parent, List<CypherConventionNode> children) {
                Parent = parent;
                _children = children ?? new List<CypherConventionNode>();
            }

            /// <summary>
            /// Parent scope
            /// </summary>
            /// <returns></returns>
            public CypherConventionScope Parent { [DebuggerStepThrough] get; }

            /// <summary>
            /// Delayed child nodes
            /// </summary>
            /// <returns></returns>
            public IReadOnlyList<CypherConventionNode> Children
            {
                [DebuggerStepThrough] get { return _children; }
            }

            /// <summary>
            /// Leaf count
            /// </summary>
            /// <returns></returns>
            public int GetLeafCount()
            {
                var scopesToVisit = new Queue<CypherConventionScope>();
                scopesToVisit.Enqueue(this);
                var leafCount = 0;
                while (scopesToVisit.Count > 0)
                {
                    var scope = scopesToVisit.Dequeue();
                    foreach (var conventionNode in scope.Children)
                    {
                        if (conventionNode is CypherConventionScope nextScope)
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
            public void Add(CypherConventionNode node)
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
            public override CypherConventionNode Accept(CypherConventionVisitor visitor) => visitor.VisitCypherConventionScope(this);

            /// <summary>
            /// Entity added
            /// </summary>
            /// <param name="entityBuilder"></param>
            /// <returns></returns>
            public virtual CypherInternalEntityBuilder OnEntityAdded([NotNull] CypherInternalEntityBuilder builder) {
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
            public virtual bool OnEntityIgnored([NotNull] CypherInternalGraphBuilder builder, [NotNull] string name, [CanBeNull] Type type)
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
            public virtual CypherInternalEntityBuilder OnBaseEntityChanged([NotNull] CypherInternalEntityBuilder builder, [CanBeNull] CypherEntity previous) {
                Add(new OnBaseEntityChangedNode(builder, previous));
                return builder;
            }
        }

        /// <summary>
        /// Immediate scope
        /// </summary>
        private class CypherImmediateConventionScope : CypherConventionScope
        {
            private readonly CypherConventionSet _cypherConventionSet;

            public CypherImmediateConventionScope([NotNull] CypherConventionSet cypherConventionSet)
                : base(parent: null, children: null)
            {
                _cypherConventionSet = cypherConventionSet;
                MakeReadonly();
            }

            public CypherInternalGraphBuilder OnGraphInitialized([NotNull] CypherInternalGraphBuilder builder) {
                foreach (var convention in _cypherConventionSet.GraphInitializedConventions) {
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
            public override CypherInternalEntityBuilder OnEntityAdded(CypherInternalEntityBuilder builder) {
                if (builder.Metadata.Builder == null)
                {
                    return null;
                }

                foreach (var convention in _cypherConventionSet.EntityAddedConventions) {
                    builder = convention.Apply(builder);

                    if (builder?.Metadata.Builder == null) {
                        return null;
                    }
                }

                return builder;
            }

            /// <summary>
            /// When base entity changed
            /// </summary>
            /// <param name="builder"></param>
            /// <param name="previous"></param>
            /// <returns></returns>
            public override CypherInternalEntityBuilder OnBaseEntityChanged(CypherInternalEntityBuilder builder, CypherEntity previous) {
                if (builder.Metadata.Builder is null) {
                    return null;
                }

                foreach (var convention in _cypherConventionSet.BaseEntityChangedConventions) {
                    if (!convention.Apply(builder, previous)) {
                        return null;
                    }
                }

                return builder;
            }
        }

        /// <summary>
        /// Delayed when entity added
        /// </summary>
        private class OnEntityAddedNode : CypherConventionNode
        {
            public OnEntityAddedNode(CypherInternalEntityBuilder entityBuilder)
            {
                EntityBuilder = entityBuilder;
            }

            public CypherInternalEntityBuilder EntityBuilder { get; }

            public override CypherConventionNode Accept(CypherConventionVisitor visitor) => visitor.VisitOnEntityAdded(this);
        }

        /// <summary>
        /// Delayed when entity ignored
        /// </summary>
        private class OnEntityIgnoredNode : CypherConventionNode
        {
            public OnEntityIgnoredNode(CypherInternalGraphBuilder builder, string name, Type type)
            {
                GraphBuilder = builder;
                Name = name;
                Type = type;
            }

            public CypherInternalGraphBuilder GraphBuilder { get; }
            public string Name { get; }
            public Type Type { get; }

            public override CypherConventionNode Accept(CypherConventionVisitor visitor) => visitor.VisitOnEntityIgnored(this);
        }

        /// <summary>
        /// Delayed when base entity changed
        /// </summary>
        private class OnBaseEntityChangedNode : CypherConventionNode
        {
            public OnBaseEntityChangedNode(CypherInternalEntityBuilder builder, CypherEntity previous)
            {
                EntityBuilder = builder;
                Previous = previous;
            }

            public CypherInternalEntityBuilder EntityBuilder { get; }
            public CypherEntity Previous { get; }

            public override CypherConventionNode Accept(CypherConventionVisitor visitor) => visitor.VisitOnBaseEntityChanged(this);
        }
    }
}