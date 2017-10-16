using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Internal;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    public abstract class Node : ConventionalAnnotatable, IMutableNode
    {
        private readonly object _labelsOrType;

        private ConfigurationSource _configurationSource;

        private Node _baseNode;

        private ConfigurationSource? _baseNodeConfigurationSource;

        private readonly SortedSet<Node> _directlyDerivedTypes = new SortedSet<Node>(NodePathComparer.Instance);

        private readonly SortedDictionary<string, NodeProperty> _properties;

        protected Node(
            [NotNull] string[] labels, 
            [NotNull] Graph graph, 
            ConfigurationSource configurationSource
        ): this(graph, configurationSource) {
            _labelsOrType = labels;
        }

        protected Node(
            [NotNull] Type clrType, 
            [NotNull] Graph graph, 
            ConfigurationSource configurationSource
        ): this(graph, configurationSource) {
            _labelsOrType = clrType;
        }

        private Node(
            [NotNull] Graph graph, 
            ConfigurationSource configurationSource
        ) {
            _configurationSource = configurationSource;
        }

        /// <summary>
        /// Graph
        /// </summary>
        /// <returns></returns>
        public virtual Graph Graph { get; }

        /// <summary>
        /// Mutable Graph
        /// </summary>
        /// <returns></returns>
        IMutableGraph IMutableNode.Graph { get => Graph; }

        /// <summary>
        /// Graph
        /// </summary>
        /// <returns></returns>
        IGraph INode.Graph { get => Graph; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract InternalNodeBuilder Builder { get; }

        /// <summary>
        /// CLR Type
        /// </summary>
        public virtual Type ClrType => _labelsOrType as Type;

        /// <summary>
        /// CLR Type
        /// </summary>
        /// <returns></returns>
        Type INode.ClrType { get => ClrType; }

        /// <summary>
        /// Base node
        /// </summary>
        public virtual Node BaseNode => _baseNode;

        /// <summary>
        /// Mutable base node
        /// </summary>
        /// <returns></returns>
        IMutableNode IMutableNode.BaseNode {
            get => _baseNode;
            set => HasBaseNode((Node)value);
        }

        /// <summary>
        /// 
        /// </summary>
        INode INode.BaseNode => _baseNode;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="configurationSource"></param>
        public virtual void HasBaseNode(
            [CanBeNull] Node node, 
            ConfigurationSource configurationSource = ConfigurationSource.Explicit
        ) {
            if (_baseNode == node) {
                // TODO: Investigate what is going on here ;-)
                UpdateBaseNodeConfigurationSource(configurationSource);
                node?.UpdateConfigurationSource(configurationSource);
                return;
            }

            // TODO: If has defining navigation

            var originalBaseNode = _baseNode;
            _baseNode?._directlyDerivedTypes.Remove(this);
            _baseNode = null;

            if (!(node is null)) {
                if (this.HasClrType()) {
                    // when the base node is a shadow node and this node is not
                    if (!node.HasClrType()) {
                        throw new InvalidOperationException(
                            CoreCypherStrings.NonClrBaseNode(
                                this.DisplayLabels(), 
                                node.DisplayLabels()
                            )
                        );
                    }

                    // non-assignable CLR types
                    if (!node.ClrType.GetTypeInfo().IsAssignableFrom(ClrType.GetTypeInfo())) {
                        throw new InvalidOperationException(
                            CoreCypherStrings.NotAssignableClrBaseNode(
                                this.DisplayLabels(), 
                                node.DisplayLabels(), 
                                ClrType.ShortDisplayName(), 
                                node.ClrType.ShortDisplayName()
                            )
                        );
                    }

                    // TODO: if defining navigation
                }

                // when this node is a shadow node and the base is not
                if (!this.HasClrType() && node.HasClrType()) {
                    throw new InvalidOperationException(
                        CoreCypherStrings.NonShadowBaseNode(
                            this.DisplayLabels(),
                            node.DisplayLabels()
                        )
                    );
                }

                // when circular inheritance
                if (node.InheritsFrom(this)) {
                    throw new InvalidOperationException(
                        CoreCypherStrings.CircularInheritance(
                            this.DisplayLabels(),
                            node.DisplayLabels()
                        )
                    );
                }
            }
        }

        /// <summary>
        /// Labels
        /// </summary>
        /// <returns></returns>
        public string[] Labels => ClrType != null
            ? ClrType.DisplayLabels()
            : (string[])_labelsOrType;

        public INode DefiningNode => throw new NotImplementedException();

        public IMutableConstraint AddExistConstraint([NotNull] IMutableNodeProperty property)
        {
            throw new NotImplementedException();
        }

        public IMutableConstraint AddKeysConstraint([NotNull] IReadOnlyList<IMutableNodeProperty> properties)
        {
            throw new NotImplementedException();
        }

        public IMutableProperty AddProperty([NotNull] string name, [CanBeNull] Type propertyType)
        {
            throw new NotImplementedException();
        }

        public IMutableConstraint AddUniqueConstraint([NotNull] IMutableNodeProperty property)
        {
            throw new NotImplementedException();
        }

        public IMutableNodeProperty FindProperty([NotNull] string name)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IMutableConstraint> GetConstraints()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IMutableNodeProperty> GetProperties()
        {
            throw new NotImplementedException();
        }

        public IMutableConstraint RemoveExistConstraint([NotNull] INodeProperty property)
        {
            throw new NotImplementedException();
        }

        public IMutableConstraint RemoveKeysConstraint([NotNull] IReadOnlyList<INodeProperty> properties)
        {
            throw new NotImplementedException();
        }

        public IMutableProperty RemoveProperty([NotNull] string name)
        {
            throw new NotImplementedException();
        }

        public IMutableConstraint RemoveUniqueConstraint([NotNull] INodeProperty property)
        {
            throw new NotImplementedException();
        }

        INodeProperty INode.FindProperty(string name)
        {
            throw new NotImplementedException();
        }

        IEnumerable<IConstraint> INode.GetConstraints()
        {
            throw new NotImplementedException();
        }

        IEnumerable<INodeProperty> INode.GetProperties()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual ConfigurationSource? GetBaseNodeConfigurationSource() => _baseNodeConfigurationSource;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configurationSource"></param>
        public virtual void UpdateConfigurationSource(ConfigurationSource configurationSource)
            => _configurationSource = _configurationSource.Max(configurationSource);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configurationSource"></param>
        private void UpdateBaseNodeConfigurationSource(ConfigurationSource configurationSource)
            => _baseNodeConfigurationSource = configurationSource.Max(_baseNodeConfigurationSource);

        /// <summary>
        /// Walk the base nodes until null looking for a match on the passed node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private bool InheritsFrom(Node node) {
            var n = this;

            do {
                if (node == n) {
                    return true;
                }
            }
            while (!((n = n._baseNode) is null));

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        public void PropertyInfoChanged() {
            foreach (var property in GetProperties()) {
                
            }

            // TODO: navigation
        }
    }
}