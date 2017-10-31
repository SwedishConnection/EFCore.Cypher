// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    public class CypherKey : ConventionalAnnotatable, IMutableKey
    {
        private ConfigurationSource _configurationSource;

        private Func<bool, IIdentityMap> _identityMapFactory;

        /// <summary>
        /// Construct from properties
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="configurationSource"></param>
        public CypherKey(
            [NotNull] IReadOnlyList<CypherProperty> properties,
            ConfigurationSource configurationSource
        ) {
            Check.NotEmpty(properties, nameof(properties));
            Check.HasNoNulls(properties, nameof(properties));

            Properties = properties;
            _configurationSource = configurationSource;

            Builder = new CypherInternalKeyBuilder(this, DeclaringEntityType.Graph.Builder);
        }

        /// <summary>
        /// Internal builder
        /// </summary>
        /// <returns></returns>
        public virtual CypherInternalKeyBuilder Builder { 
            [DebuggerStepThrough] get; 
            [DebuggerStepThrough] [param: CanBeNull] set; 
        }

        /// <summary>
        /// Get configuration source
        /// </summary>
        /// <returns></returns>
        public virtual ConfigurationSource GetConfigurationSource() => _configurationSource;

        /// <summary>
        /// Update configuration source plus each property's configuration source
        /// </summary>
        /// <param name="configurationSource"></param>
        public virtual void UpdateConfigurationSource(ConfigurationSource configurationSource)
        {
            _configurationSource = _configurationSource.Max(configurationSource);
            foreach (var property in Properties)
            {
                property.UpdateConfigurationSource(configurationSource);
            }
        }

        /// <summary>
        /// Properties making up the key
        /// </summary>
        /// <returns></returns>
        public virtual IReadOnlyList<CypherProperty> Properties { [DebuggerStepThrough] get; }
        
        /// <summary>
        /// Mutable properties
        /// </summary>
        IReadOnlyList<IMutableProperty> IMutableKey.Properties => Properties;

        /// <summary>
        /// Read-only properties
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<IProperty> IKey.Properties => Properties;


        /// <summary>
        /// Declaring entity
        /// </summary>
        /// <returns></returns>
        public virtual CypherEntity DeclaringEntityType
        {
            [DebuggerStepThrough] get => Properties[0].DeclaringEntityType;
        }

        /// <summary>
        /// Mutable declaring entity
        /// </summary>
        IMutableEntityType IMutableKey.DeclaringEntityType => DeclaringEntityType;

        /// <summary>
        /// Read-only declaring entity
        /// </summary>
        /// <returns></returns>
        IEntityType IKey.DeclaringEntityType => DeclaringEntityType;

        /// <summary>
        /// Identity map factory
        /// </summary>
        /// <param name="_identityMapFactory"></param>
        /// <param name="IdentityMapFactoryFactory("></param>
        /// <returns></returns>
        public virtual Func<bool, IIdentityMap> IdentityMapFactory
            => NonCapturingLazyInitializer.EnsureInitialized(
                ref _identityMapFactory, this, k => new IdentityMapFactoryFactory().Create(k));
    }
}