// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    /// <summary>
    /// Node (<see cref="TypeBase" />)
    /// </summary>
    public abstract class Node : ConventionalAnnotatable, IMutableTypeBase
    {
        private readonly object _typeOrName;

        private ConfigurationSource _configurationSource;

        private readonly Dictionary<string, ConfigurationSource> _ignoredMembers = new Dictionary<string, ConfigurationSource>();

        protected Node([NotNull] string name, [NotNull] Graph graph, ConfigurationSource configurationSource) : this(graph, configurationSource) {
            Check.NotEmpty(name, nameof(name));
            Check.NotNull(graph, nameof(graph));

            _typeOrName = name;
        }

        protected Node([NotNull] Type clrType, [NotNull] Graph graph, ConfigurationSource configurationSource) : this(graph, configurationSource) {
            Check.NotNull(clrType, nameof(clrType));
            Check.NotNull(graph, nameof(graph));

            _typeOrName = clrType;
        }

        private Node([NotNull] Graph graph, ConfigurationSource configurationSource) {
            Graph = graph;
            _configurationSource = configurationSource;
        }

        /// <summary>
        /// Clr type
        /// </summary>
        public virtual Type ClrType => _typeOrName as Type;

        /// <summary>
        /// Name
        /// </summary>
        /// <returns></returns>
        public virtual string Name
            => ClrType != null ? ClrType.DisplayName() : (string)_typeOrName;

        /// <summary>
        /// Graph
        /// </summary>
        /// <returns></returns>
        public virtual Graph Graph { get; }
        
        /// <summary>
        /// Graph as model
        /// </summary>
        /// <returns></returns>
        public IMutableModel Model { [DebuggerStepThrough] get => Graph; }

        /// <summary>
        /// Graph as model
        /// </summary>
        /// <returns></returns>
        IModel ITypeBase.Model { [DebuggerStepThrough] get => Graph; }

        /// <summary>
        /// Configuration source
        /// </summary>
        /// <returns></returns>
        public virtual ConfigurationSource GetConfigurationSource() => _configurationSource;

        /// <summary>
        /// Update configuration source
        /// </summary>
        /// <param name="configurationSource"></param>
        public virtual void UpdateConfigurationSource(ConfigurationSource configurationSource)
            => _configurationSource = _configurationSource.Max(configurationSource);

        /// <summary>
        /// Property metadata changed
        /// </summary>
        public abstract void PropertyMetadataChanged();

        /// <summary>
        /// Ignore
        /// </summary>
        /// <param name="name"></param>
        /// <param name="configurationSource"></param>
        public virtual void Ignore([NotNull] string name, ConfigurationSource configurationSource = ConfigurationSource.Explicit)
        {
            Check.NotNull(name, nameof(name));

            if (_ignoredMembers.TryGetValue(name, out var existingIgnoredConfigurationSource))
            {
                _ignoredMembers[name] = configurationSource.Max(existingIgnoredConfigurationSource);
                return;
            }

            _ignoredMembers[name] = configurationSource;

            OnTypeMemberIgnored(name);
        }

        /// <summary>
        /// When type member ignored
        /// </summary>
        /// <param name="name"></param>
        public abstract void OnTypeMemberIgnored([NotNull] string name);

        /// <summary>
        /// Ignored members
        /// </summary>
        /// <returns></returns>
        public virtual IReadOnlyList<string> GetIgnoredMembers()
            => _ignoredMembers.Keys.ToList();

        /// <summary>
        /// Find ignored member's configuration source
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual ConfigurationSource? FindDeclaredIgnoredMemberConfigurationSource([NotNull] string name)
        {
            Check.NotEmpty(name, nameof(name));

            if (_ignoredMembers.TryGetValue(name, out var ignoredConfigurationSource))
            {
                return ignoredConfigurationSource;
            }

            return null;
        }

        /// <summary>
        /// Find ignored member's configuration source
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual ConfigurationSource? FindIgnoredMemberConfigurationSource([NotNull] string name)
            => FindDeclaredIgnoredMemberConfigurationSource(name);

        /// <summary>
        /// Take away ignore
        /// </summary>
        /// <param name="name"></param>
        public virtual void Unignore([NotNull] string name)
        {
            Check.NotNull(name, nameof(name));
            _ignoredMembers.Remove(name);
        }
    }
}