// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
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

            /// <summary>
            /// Property added
            /// </summary>
            /// <param name="propertyBuilder"></param>
            /// <returns></returns>
            public virtual CypherInternalPropertyBuilder OnPropertyAdded([NotNull] CypherInternalPropertyBuilder propertyBuilder)
            {
                Add(new OnPropertyAddedNode(propertyBuilder));
                return propertyBuilder;
            }

            /// <summary>
            /// Foreign key unique changed
            /// </summary>
            /// <param name="builder"></param>
            /// <returns></returns>
            public virtual CypherInternalRelationshipBuilder OnForeignKeyUniqueChanged([NotNull] CypherInternalRelationshipBuilder builder) {
                Add(new OnForeignKeyUniqueChangedNode(builder));
                return builder;
            }

            /// <summary>
            /// Property nullable changed
            /// </summary>
            /// <param name="builder"></param>
            /// <returns></returns>
            public virtual bool OnPropertyNullableChanged([NotNull] CypherInternalPropertyBuilder builder) {
                Add(new OnPropertyNullableChangedNode(builder));
                return true;
            }

            /// <summary>
            /// Foreign key ownership changed
            /// </summary>
            /// <param name="builder"></param>
            /// <returns></returns>
            public virtual CypherInternalRelationshipBuilder OnForeignKeyOwnershipChanged([NotNull] CypherInternalRelationshipBuilder builder) {
                Add(new OnForeignKeyOwnershipChangedNode(builder));
                return builder;
            }

            /// <summary>
            /// Navigation removed
            /// </summary>
            /// <param name="startEntityBuilder"></param>
            /// <param name="endEntityBuilder"></param>
            /// <param name="name"></param>
            /// <param name="propertyInfo"></param>
            public virtual void OnNavigationRemoved(
                [NotNull] CypherInternalEntityBuilder startEntityBuilder,
                [NotNull] CypherInternalEntityBuilder endEntityBuilder,
                [NotNull] string name,
                [CanBeNull] PropertyInfo propertyInfo
            ) => Add(new OnNavigationRemovedNode(startEntityBuilder, endEntityBuilder, name, propertyInfo));

            /// <summary>
            /// When navigation added
            /// </summary>
            /// <param name="builder"></param>
            /// <param name="navigation"></param>
            /// <returns></returns>
            public virtual CypherInternalRelationshipBuilder OnNavigationAdded([NotNull] CypherInternalRelationshipBuilder builder, [NotNull] CypherNavigation navigation) {
                Add(new OnNavigationAddedNode(builder, navigation));
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

            /// <summary>
            /// When entity ignored
            /// </summary>
            /// <param name="builder"></param>
            /// <param name="name"></param>
            /// <param name="type"></param>
            /// <returns></returns>
            public override bool OnEntityIgnored(CypherInternalGraphBuilder builder, string name, Type type) {
                foreach (var convention in _cypherConventionSet.EntityIgnoredConventions) {
                    if (!convention.Apply(builder, name, type)) {
                        return false;
                    }
                }

                return true;
            }

            /// <summary>
            /// When property added
            /// </summary>
            /// <param name="propertyBuilder"></param>
            /// <returns></returns>
            public override CypherInternalPropertyBuilder OnPropertyAdded(CypherInternalPropertyBuilder propertyBuilder)
            {
                if (propertyBuilder.Metadata.Builder is null
                    || propertyBuilder.Metadata.DeclaringEntityType.Builder is null)
                {
                    return null;
                }

                foreach (var propertyConvention in _cypherConventionSet.PropertyAddedConventions)
                {
                    propertyBuilder = propertyConvention.Apply(propertyBuilder);
                    if (propertyBuilder?.Metadata.Builder is null
                        || propertyBuilder.Metadata.DeclaringEntityType.Builder is null)
                    {
                        return null;
                    }
                }

                return propertyBuilder;
            }

            /// <summary>
            /// When foreign key unique change
            /// </summary>
            /// <param name="builder"></param>
            /// <returns></returns>
            public override CypherInternalRelationshipBuilder OnForeignKeyUniqueChanged(CypherInternalRelationshipBuilder builder) {
                if (builder.Metadata.Builder is null) {
                    return null;
                }

                foreach (var convention in _cypherConventionSet.ForeignKeyUniqueChangedConventions) {
                    builder = convention.Apply(builder);

                    if (builder?.Metadata.Builder is null) {
                        return null;
                    }
                }

                return builder;
            }

            /// <summary>
            /// When property nullable changed
            /// </summary>
            /// <param name="builder"></param>
            /// <returns></returns>
            public override bool OnPropertyNullableChanged(CypherInternalPropertyBuilder builder) {
                if (builder.Metadata.Builder is null || builder.Metadata.DeclaringEntityType.Builder == null) {
                    return false;
                }

                foreach (var convention in _cypherConventionSet.PropertyNullabilityChangedConventions) {
                    if (!convention.Apply(builder)) {
                        return false;
                    }
                }

                return true;
            }

            /// <summary>
            /// When foreign key ownership changed
            /// </summary>
            /// <param name="builder"></param>
            /// <returns></returns>
            public override CypherInternalRelationshipBuilder OnForeignKeyOwnershipChanged(CypherInternalRelationshipBuilder builder) {
                if (builder.Metadata.Builder == null) {
                    return null;
                }

                foreach (var convention in _cypherConventionSet.ForeignKeyOwnershipChangedConventions) {
                    builder = convention.Apply(builder);

                    if (builder?.Metadata.Builder == null) {
                        return null;
                    }
                }

                return builder;
            }

            /// <summary>
            /// When navigation removed
            /// </summary>
            /// <param name="startEntityBuilder"></param>
            /// <param name="endEntityBuilder"></param>
            /// <param name="name"></param>
            /// <param name="propertyInfo"></param>
            public override void OnNavigationRemoved(CypherInternalEntityBuilder startEntityBuilder, CypherInternalEntityBuilder endEntityBuilder, string name, PropertyInfo propertyInfo) {
                if (startEntityBuilder.Metadata.Builder is null) {
                    return;
                }

                foreach (var convention in _cypherConventionSet.NavigationRemovedConventions) {
                    if (convention.Apply(startEntityBuilder, endEntityBuilder, name, propertyInfo)) {
                        break;
                    }
                }
            }

            /// <summary>
            /// When navigation added
            /// </summary>
            /// <param name="relationshipBuilder"></param>
            /// <param name="navigation"></param>
            /// <returns></returns>
            public override CypherInternalRelationshipBuilder OnNavigationAdded(CypherInternalRelationshipBuilder relationshipBuilder, CypherNavigation navigation) {
                if (relationshipBuilder.Metadata.Builder is null) {
                    return null;
                }

                foreach (var convention in _cypherConventionSet.NavigationAddedConventions) {
                    relationshipBuilder = convention.Apply(relationshipBuilder, navigation);

                    if (relationshipBuilder?.Metadata.Builder is null) {
                        return null;
                    }
                }

                return relationshipBuilder;
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

        /// <summary>
        /// Delayed when property added
        /// </summary>
        private class OnPropertyAddedNode : CypherConventionNode
        {
            public OnPropertyAddedNode(CypherInternalPropertyBuilder propertyBuilder)
            {
                PropertyBuilder = propertyBuilder;
            }

            public CypherInternalPropertyBuilder PropertyBuilder { get; }

            public override CypherConventionNode Accept(CypherConventionVisitor visitor) => visitor.VisitOnPropertyAdded(this);
        }

        /// <summary>
        /// Delayed foreign key unique changed
        /// </summary>
        private class OnForeignKeyUniqueChangedNode : CypherConventionNode {
            public OnForeignKeyUniqueChangedNode(CypherInternalRelationshipBuilder builder) {
                RelationshipBuilder = builder;
            }

            public CypherInternalRelationshipBuilder RelationshipBuilder { get; }

            public override CypherConventionNode Accept(CypherConventionVisitor visitor) => visitor.VisitOnForeignKeyUniqueChanged(this);
        }

        /// <summary>
        /// Delayed property nullable changed
        /// </summary>
        private class OnPropertyNullableChangedNode : CypherConventionNode {
            public OnPropertyNullableChangedNode(CypherInternalPropertyBuilder builder) {
                PropertyBuilder = builder;
            }

            public CypherInternalPropertyBuilder PropertyBuilder { get; }

            public override CypherConventionNode Accept(CypherConventionVisitor visitor) => visitor.VisitOnPropertyNullableChanged(this);
        }

        /// <summary>
        /// Delayed foreign key ownership changed
        /// </summary>
        private class OnForeignKeyOwnershipChangedNode: CypherConventionNode {
            public OnForeignKeyOwnershipChangedNode(CypherInternalRelationshipBuilder builder) {
                RelationshipBuilder = builder;
            }

            public CypherInternalRelationshipBuilder RelationshipBuilder { get; }

            public override CypherConventionNode Accept(CypherConventionVisitor visitor) => visitor.VisitOnForeignKeyOwnershipChanged(this);
        }

        /// <summary>
        /// Delayed navigation removed
        /// </summary>
        private class OnNavigationRemovedNode: CypherConventionNode {
            public OnNavigationRemovedNode(
                CypherInternalEntityBuilder startEntityBuilder,
                CypherInternalEntityBuilder endEntityBuilder,
                string name,
                PropertyInfo propertyInfo
            ) {
                StartEntityBuilder = startEntityBuilder;
                EndEntityBuilder = endEntityBuilder;
                Name = name;
                PropertyInfo = propertyInfo;
            } 

            public CypherInternalEntityBuilder StartEntityBuilder { get; }

            public CypherInternalEntityBuilder EndEntityBuilder { get; }

            public string Name { get; }

            public PropertyInfo PropertyInfo { get; }

            public override CypherConventionNode Accept(CypherConventionVisitor visitor) => visitor.VisitOnNavigationRemoved(this);
        }

        /// <summary>
        /// Delayed navigation added
        /// </summary>
        private class OnNavigationAddedNode: CypherConventionNode {

            public OnNavigationAddedNode(CypherInternalRelationshipBuilder builder, CypherNavigation navigation) {
                RelationshipBuilder = builder;
                Navigation = navigation;
            }

            public CypherInternalRelationshipBuilder RelationshipBuilder { get; }

            public CypherNavigation Navigation { get; }

            public override CypherConventionNode Accept(CypherConventionVisitor visitor) => visitor.VisitOnNavigationAdded(this);
        }
    }
}