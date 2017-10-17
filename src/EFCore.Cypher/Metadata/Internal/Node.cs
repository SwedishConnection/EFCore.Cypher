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

        protected Node _baseType;

        private ConfigurationSource _configurationSource;

        private ConfigurationSource? _baseTypeConfigurationSource;

        private readonly SortedDictionary<string, NodeProperty> _properties;

        /// <summary>
        /// Node by labels
        /// </summary>
        /// <param name="labels"></param>
        /// <param name="graph"></param>
        /// <param name="configurationSource"></param>
        /// <returns></returns>
        protected Node(
            [NotNull] string[] labels, 
            [NotNull] Graph graph, 
            ConfigurationSource configurationSource
        ): this(graph, configurationSource) {
            _labelsOrType = labels;
        }

        /// <summary>
        /// Node by CLR type
        /// </summary>
        /// <param name="clrType"></param>
        /// <param name="graph"></param>
        /// <param name="configurationSource"></param>
        /// <returns></returns>
        protected Node(
            [NotNull] Type clrType, 
            [NotNull] Graph graph, 
            ConfigurationSource configurationSource
        ): this(graph, configurationSource) {
            _labelsOrType = clrType;
        }

    
        /// <summary>
        /// Set graph and configuration source
        /// </summary>
        /// <param name="graph"></param>
        /// <param name="configurationSource"></param>
        private Node(
            [NotNull] Graph graph, 
            ConfigurationSource configurationSource
        ) {
            Graph = graph;
            _configurationSource = configurationSource;
        }

        /// <summary>
        /// Internal builder
        /// </summary>
        /// <returns></returns>
        public virtual InternalNodeBuilder Builder { get; }

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
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual Node BaseType { get; }

        /// <summary>
        /// Base type
        /// </summary>
        IMutableNode IMutableNode.BaseType => BaseType;

        /// <summary>
        /// Base type
        /// </summary>
        INode INode.BaseType => BaseType;

        /// <summary>
        /// Labels
        /// </summary>
        /// <returns></returns>
        public string[] Labels => ClrType != null
            ? ClrType.DisplayLabels()
            : (string[])_labelsOrType;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public IMutableConstraint AddExistConstraint([NotNull] IMutableNodeProperty property)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="propertyType"></param>
        /// <returns></returns>
        public IMutableProperty AddProperty([NotNull] string name, [CanBeNull] Type propertyType)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IMutableNodeProperty FindProperty([NotNull] string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IMutableConstraint> GetConstraints()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IMutableNodeProperty> GetProperties()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public IMutableConstraint RemoveExistConstraint([NotNull] INodeProperty property)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IMutableProperty RemoveProperty([NotNull] string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public IMutableConstraint RemoveUniqueConstraint([NotNull] INodeProperty property)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        INodeProperty INode.FindProperty(string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<IConstraint> INode.GetConstraints()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<INodeProperty> INode.GetProperties()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual ConfigurationSource GetConfigurationSource() => _configurationSource;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual ConfigurationSource? GetBaseTypeConfigurationSource() => _baseTypeConfigurationSource;

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
        private void UpdateBaseTypeConfigurationSource(ConfigurationSource configurationSource)
            => _baseTypeConfigurationSource = configurationSource.Max(_baseTypeConfigurationSource);

        /// <summary>
        /// 
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
            while (!((n = n.BaseType) is null));

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