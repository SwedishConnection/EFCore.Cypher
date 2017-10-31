// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    public class CypherForeignKey : CypherNode, IMutableForeignKey
    {
        private bool? _isUnique;

        private bool? _isOwnership;

        private ConfigurationSource _configurationSource;

        private ConfigurationSource? _isUniqueConfigurationSource;

        private ConfigurationSource? _isRequiredConfigurationSource;

        private ConfigurationSource? _isOwnershipConfigurationSource;

        private ConfigurationSource? _dependentToPrincipalConfigurationSource;

        private ConfigurationSource? _principalToDependentConfigurationSource;

        public CypherForeignKey(
            [NotNull] string name,
            [NotNull] CypherProperty endProperty,
            [NotNull] CypherEntity end,
            [NotNull] CypherEntity start,
            ConfigurationSource configurationSource
        ): base(name, end.Graph, configurationSource) {
            PrincipalEntityType = start;
            DeclaringEntityType = end;
            Properties = new[] { endProperty };
            _configurationSource = configurationSource;
        }

        public CypherForeignKey(
            [NotNull] Type clrType,
            [NotNull] CypherProperty endProperty,
            [NotNull] CypherEntity end,
            [NotNull] CypherEntity start,
            ConfigurationSource configurationSource
        ): base(clrType, end.Graph, configurationSource) {
            PrincipalEntityType = start;
            DeclaringEntityType = end;
            Properties = new[] { endProperty };
            _configurationSource = configurationSource;
        }

        /// <summary>
        /// Builder
        /// </summary>
        /// <returns></returns>
        public virtual CypherInternalRelationshipBuilder Builder { 
            [DebuggerStepThrough] get; 
            [DebuggerStepThrough] [param: CanBeNull] set; 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public override void OnTypeMemberIgnored(string name)
            => throw new NotImplementedException();

        /// <summary>
        /// Todo:
        /// </summary>
        public override void PropertyMetadataChanged()
            => throw new NotImplementedException();

        /// <summary>
        /// Update configuration source (effects start and end configurations)
        /// </summary>
        /// <param name="configurationSource"></param>
        public new void UpdateConfigurationSource(ConfigurationSource configurationSource)
        {
            _configurationSource = _configurationSource.Max(configurationSource);

            DeclaringEntityType.UpdateConfigurationSource(configurationSource);
            PrincipalEntityType.UpdateConfigurationSource(configurationSource);
        }

        /// <summary>
        /// End (dependent) entity
        /// </summary>
        /// <returns></returns>
        public virtual CypherEntity DeclaringEntityType { [DebuggerStepThrough] get; }

        /// <summary>
        /// Mutable end entity
        /// </summary>
        IMutableEntityType IMutableForeignKey.DeclaringEntityType => DeclaringEntityType;

        /// <summary>
        /// Read-only end entity
        /// </summary>
        IEntityType IForeignKey.DeclaringEntityType => DeclaringEntityType;

        /// <summary>
        /// End (dependent) property (singular)
        /// </summary>
        /// <returns></returns>
        public virtual IReadOnlyList<CypherProperty> Properties { [DebuggerStepThrough] get; }

        /// <summary>
        /// Mutable end property
        /// </summary>
        IReadOnlyList<IMutableProperty> IMutableForeignKey.Properties => Properties;

        /// <summary>
        /// Read-only end property
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<IProperty> IForeignKey.Properties => Properties;

        /// <summary>
        /// Start (principal) entity
        /// </summary>
        /// <returns></returns>
        public virtual CypherEntity PrincipalEntityType { [DebuggerStepThrough] get; }

        /// <summary>
        /// Mutable start entity
        /// </summary>
        IMutableEntityType IMutableForeignKey.PrincipalEntityType => PrincipalEntityType;

        /// <summary>
        /// Read-only start entity
        /// </summary>
        /// <returns></returns>
        IEntityType IForeignKey.PrincipalEntityType => PrincipalEntityType;

        // Invalid operation
        public IMutableKey PrincipalKey => throw new InvalidOperationException("Cypher relationships do not use keys");

        // Invalid operation
        IKey IForeignKey.PrincipalKey => throw new InvalidOperationException("Cypher relationships do not use keys");

        /// <summary>
        /// End (dependent) navigation
        /// </summary>
        /// <returns></returns>
        public virtual CypherNavigation DependentToPrincipal { 
            get; 
            private set; 
        }        

        /// <summary>
        /// Mutable end navigation
        /// </summary>
        IMutableNavigation IMutableForeignKey.DependentToPrincipal => DependentToPrincipal;

        /// <summary>
        /// Read-only end navigation
        /// </summary>
        INavigation IForeignKey.DependentToPrincipal => DependentToPrincipal;


        /// <summary>
        /// Start (principal) navigation
        /// </summary>
        /// <returns></returns>
        public virtual CypherNavigation PrincipalToDependent { 
            get; 
            private set; 
        }

        /// <summary>
        /// Mutable start navigation
        /// </summary>
        IMutableNavigation IMutableForeignKey.PrincipalToDependent => PrincipalToDependent;

        /// <summary>
        /// Read-only start navigation
        /// </summary>
        /// <returns></returns>
        INavigation IForeignKey.PrincipalToDependent => PrincipalToDependent;

        /// <summary>
        /// Is unqiue
        /// </summary>
        /// <returns></returns>
        public virtual bool IsUnique
        {
            get => _isUnique ?? false;
            set => SetIsUnique(value, ConfigurationSource.Explicit);
        }

        /// <summary>
        /// Set unique
        /// </summary>
        /// <param name="unique"></param>
        /// <param name="configurationSource"></param>
        /// <returns></returns>
        public virtual CypherForeignKey SetIsUnique(bool unique, ConfigurationSource configurationSource) {
            var isChanging = IsUnique != unique;

            _isUnique = unique;
            UpdateIsUniqueConfigurationSource(configurationSource);

            return isChanging ?
                DeclaringEntityType.Graph.CypherConventionDispatcher.OnForeignKeyUniqueChanged(Builder)?.Metadata :
                this;
        }

        /// <summary>
        /// Get is unique configuration source
        /// </summary>
        /// <returns></returns>
        public virtual ConfigurationSource? GetIsUniqueConfigurationSource() => _isUniqueConfigurationSource;

        /// <summary>
        /// Update is unique configuration source
        /// </summary>
        /// <param name="configurationSource"></param>
        public virtual void UpdateIsUniqueConfigurationSource(ConfigurationSource configurationSource)
            => _isUniqueConfigurationSource = configurationSource.Max(_isUniqueConfigurationSource);

        /// <summary>
        /// Is required
        /// </summary>
        /// <returns></returns>
        public virtual bool IsRequired
        {
            get { return !Properties.Any(p => p.IsNullable); }
            set => SetIsRequired(value, ConfigurationSource.Explicit);
        }

        /// <summary>
        /// Set is required
        /// </summary>
        /// <param name="required"></param>
        /// <param name="configurationSource"></param>
        public virtual void SetIsRequired(bool required, ConfigurationSource configurationSource)
        {
            if (required == IsRequired) {
                UpdateIsRequiredConfigurationSource(configurationSource);
                return;
            }

            var properties = Properties;
            if (!required) {
                var nullableTypeProperties = Properties.Where(p => p.ClrType.IsNullableType()).ToList();
                if (nullableTypeProperties.Any())
                {
                    properties = nullableTypeProperties;
                }
                // If no properties can be made nullable, let it fail
            }

            foreach (var property in properties)
            {
                property.SetIsNullable(!required, configurationSource);
            }

            UpdateIsRequiredConfigurationSource(configurationSource);
        }

        /// <summary>
        /// Set is required configuration source
        /// </summary>
        /// <param name="configurationSource"></param>
        public virtual void SetIsRequiredConfigurationSource(ConfigurationSource? configurationSource)
            => _isRequiredConfigurationSource = configurationSource;

        /// <summary>
        /// Update is required configuration source
        /// </summary>
        /// <param name="configurationSource"></param>
        public virtual void UpdateIsRequiredConfigurationSource(ConfigurationSource configurationSource)
            => _isRequiredConfigurationSource = configurationSource.Max(_isRequiredConfigurationSource);
        
        /// <summary>
        /// Is owership
        /// </summary>
        /// <returns></returns>
        public virtual bool IsOwnership
        {
            get => _isOwnership ?? false;
            set => SetIsOwnership(value, ConfigurationSource.Explicit);
        }

        /// <summary>
        /// Set ownership
        /// </summary>
        /// <param name="ownership"></param>
        /// <param name="configurationSource"></param>
        /// <returns></returns>
        public virtual CypherForeignKey SetIsOwnership(bool ownership, ConfigurationSource configurationSource) {
            var isChanging = IsOwnership != ownership;

            _isOwnership = ownership;
            UpdateIsOwnershipConfigurationSource(configurationSource);

            return isChanging ?
                DeclaringEntityType.Graph.CypherConventionDispatcher.OnForeignKeyOwnershipChanged(Builder)?.Metadata : 
                this;
        }

        /// <summary>
        /// Get is ownershipo configuration source
        /// </summary>
        /// <returns></returns>
        public virtual ConfigurationSource? GetIsOwnershipConfigurationSource() => _isOwnershipConfigurationSource;

        /// <summary>
        /// Update is ownership configuration source
        /// </summary>
        /// <param name="configurationSource"></param>
        public virtual void UpdateIsOwnershipConfigurationSource(ConfigurationSource configurationSource)
            => _isOwnershipConfigurationSource = configurationSource.Max(_isOwnershipConfigurationSource);

        public DeleteBehavior DeleteBehavior { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        DeleteBehavior IForeignKey.DeleteBehavior => throw new System.NotImplementedException();

        /// <summary>
        /// Set end to start navigation
        /// </summary>
        /// <param name="name"></param>
        /// <param name="configurationSource"></param>
        /// <returns></returns>
        public virtual CypherNavigation HasDependentToPrincipal(
            [CanBeNull] string name,
            ConfigurationSource configurationSource = ConfigurationSource.Explicit)
            => Navigation(PropertyIdentity.Create(name), configurationSource, pointsToPrincipal: true);

        /// <summary>
        /// Mutable dependent navigation
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IMutableNavigation IMutableForeignKey.HasDependentToPrincipal(string name) 
            => HasDependentToPrincipal(name);

        /// <summary>
        /// Set end to start navigation
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="configurationSource"></param>
        /// <returns></returns>
        public virtual CypherNavigation HasDependentToPrincipal(
            [CanBeNull] PropertyInfo propertyInfo,
            ConfigurationSource configurationSource = ConfigurationSource.Explicit)
            => Navigation(PropertyIdentity.Create(propertyInfo), configurationSource, pointsToPrincipal: true);


        /// <summary>
        /// Mutable dependent navigation
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        IMutableNavigation IMutableForeignKey.HasDependentToPrincipal(PropertyInfo property)
            => HasDependentToPrincipal(property);

        /// <summary>
        /// Set start to end navigation
        /// </summary>
        /// <param name="name"></param>
        /// <param name="configurationSource"></param>
        /// <returns></returns>
        public virtual CypherNavigation HasPrincipalToDependent(
            [CanBeNull] string name,
            ConfigurationSource configurationSource = ConfigurationSource.Explicit)
            => Navigation(PropertyIdentity.Create(name), configurationSource, pointsToPrincipal: false);

        /// <summary>
        /// Mutable principal navigation
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IMutableNavigation IMutableForeignKey.HasPrincipalToDependent(string name)
            => HasDependentToPrincipal(name);

        /// <summary>
        /// Start to end navigation
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="configurationSource"></param>
        /// <returns></returns>
        public virtual CypherNavigation HasPrincipalToDependent(
            [CanBeNull] PropertyInfo propertyInfo,
            ConfigurationSource configurationSource = ConfigurationSource.Explicit)
            => Navigation(PropertyIdentity.Create(propertyInfo), configurationSource, pointsToPrincipal: false);

        /// <summary>
        /// Mutable principal navigation
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        IMutableNavigation IMutableForeignKey.HasPrincipalToDependent(PropertyInfo property)
            => HasPrincipalToDependent(property);

        /// <summary>
        /// Set navigation
        /// </summary>
        /// <param name="propertyIdentity"></param>
        /// <param name="configurationSource"></param>
        /// <param name="pointsToPrincipal"></param>
        /// <returns></returns>
        private CypherNavigation Navigation(
            PropertyIdentity? propertyIdentity,
            ConfigurationSource configurationSource,
            bool pointsToPrincipal
        ) {
            var name = propertyIdentity?.Name;
            var oldNavigation = pointsToPrincipal ?
                DependentToPrincipal : 
                PrincipalToDependent;

            if (name == oldNavigation.Name) {
                if (pointsToPrincipal) {
                    UpdateDependentToPrincipalConfigurationSource(configurationSource);
                } else {
                    UpdatePrincipalToDependentConfigurationSource(configurationSource);
                }

                return oldNavigation;
            }

            if (!(oldNavigation is null)) {
                if (pointsToPrincipal) {
                    DeclaringEntityType.RemoveNavigation(oldNavigation.Name);
                    DeclaringEntityType.Graph.CypherConventionDispatcher.OnNavigationRemoved(
                        DeclaringEntityType.Builder,
                        PrincipalEntityType.Builder,
                        oldNavigation.Name,
                        oldNavigation.PropertyInfo
                    );
                } else {
                    PrincipalEntityType.RemoveNavigation(oldNavigation.Name);
                    DeclaringEntityType.Graph.CypherConventionDispatcher.OnNavigationRemoved(
                        PrincipalEntityType.Builder,
                        DeclaringEntityType.Builder,
                        oldNavigation.Name,
                        oldNavigation.PropertyInfo
                    );
                }
            }

            CypherNavigation navigation = null;
            var property = propertyIdentity?.Property;

            if (!(property is null)) {
                navigation = pointsToPrincipal 
                    ? DeclaringEntityType.AddNavigation(property, this, pointsToPrincipal: true)
                    : PrincipalEntityType.AddNavigation(property, this, pointsToPrincipal: false);
            } else if (!(name is null)) {
                navigation = pointsToPrincipal
                    ? DeclaringEntityType.AddNavigation(name, this, pointsToPrincipal: true)
                    : PrincipalEntityType.AddNavigation(name, this, pointsToPrincipal: false);
            }

            if (pointsToPrincipal) {
                DependentToPrincipal = navigation;
                UpdateDependentToPrincipalConfigurationSource(configurationSource);
            } else {
                PrincipalToDependent = navigation;
                UpdatePrincipalToDependentConfigurationSource(configurationSource);
            }

            if (!(navigation is null)) {
                var builder = DeclaringEntityType.Graph.CypherConventionDispatcher.OnNavigationAdded(Builder, navigation);
                navigation = pointsToPrincipal 
                    ? builder?.Metadata.DependentToPrincipal
                    : builder?.Metadata.PrincipalToDependent;
            }

            return navigation ?? oldNavigation;
        }

        /// <summary>
        /// Update dependent to principal configuration source
        /// </summary>
        /// <param name="configurationSource"></param>
        public virtual void UpdateDependentToPrincipalConfigurationSource(ConfigurationSource? configurationSource)
            => _dependentToPrincipalConfigurationSource = configurationSource.Max(_dependentToPrincipalConfigurationSource);

        /// <summary>
        /// Update principal to dependent configuration source
        /// </summary>
        /// <param name="configurationSource"></param>
        public virtual void UpdatePrincipalToDependentConfigurationSource(ConfigurationSource? configurationSource)
            => _principalToDependentConfigurationSource = configurationSource.Max(_principalToDependentConfigurationSource);
    }
}