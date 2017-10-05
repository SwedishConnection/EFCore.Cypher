using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    public abstract class Node : ConventionalAnnotatable, IMutableNode
    {
        private readonly object _labelsOrType;

        private ConfigurationSource _configurationSource;

        private Node _baseNode;

        private ConfigurationSource? _baseNodeConfigurationSource;

        private readonly SortedSet<Node> _directlyDerivedTypes = new SortedSet<Node>(EntityTypePathComparer.Instance);

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

            var localBaseNode = _baseNode;

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
    }
}