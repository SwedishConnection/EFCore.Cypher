// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    public class CypherProperty : PropertyBase, IMutableProperty
    {
        private bool? _isNullable;

        private ConfigurationSource _configurationSource;

        private ConfigurationSource? _typeConfigurationSource;

        private ConfigurationSource? _isNullableConfigurationSource;

        private PropertyIndexes _indexes;

        public CypherProperty(
            [NotNull] string name,
            [NotNull] Type clrType,
            [CanBeNull] PropertyInfo propertyInfo,
            [CanBeNull] FieldInfo fieldInfo,
            [NotNull] CypherEntity declaringEntity,
            ConfigurationSource configurationSource,
            ConfigurationSource? typeConfigurationSource
        ) : base(name, propertyInfo, fieldInfo) {
            Check.NotNull(clrType, nameof(clrType));
            Check.NotNull(declaringEntity, nameof(declaringEntity));

            DeclaringEntityType = declaringEntity;
            ClrType = clrType;
            _configurationSource = configurationSource;
            _typeConfigurationSource = typeConfigurationSource;

            Builder = new CypherInternalPropertyBuilder(this, declaringEntity.Graph.Builder);
        }

        /// <summary>
        /// Declaring entity
        /// </summary>
        /// <returns></returns>
        public virtual CypherEntity DeclaringEntityType { [DebuggerStepThrough] get; }

        /// <summary>
        /// Declaring entity
        /// </summary>
        /// <returns></returns>
        public new virtual CypherEntity DeclaringType
        {
            [DebuggerStepThrough] get => DeclaringEntityType;
        }

        /// <summary>
        /// Property metadata changed
        /// </summary>
        protected override void PropertyMetadataChanged() => DeclaringType.PropertyMetadataChanged();

        /// <summary>
        /// Mutable declaring entity
        /// </summary>
        IMutableEntityType IMutableProperty.DeclaringEntityType => DeclaringEntityType;

        /// <summary>
        /// Read-only declaring entity
        /// </summary>
        IEntityType IProperty.DeclaringEntityType => DeclaringEntityType;

        /// <summary>
        /// Clr type
        /// </summary>
        /// <returns></returns>
        public override Type ClrType { [DebuggerStepThrough] get; }

        /// <summary>
        /// Builder
        /// </summary>
        /// <returns></returns>
        public virtual CypherInternalPropertyBuilder Builder { 
             [DebuggerStepThrough] get; 
             [DebuggerStepThrough] [param: CanBeNull] set; 
        }

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
        /// Set configuration source
        /// </summary>
        /// <param name="configurationSource"></param>
        public virtual void SetConfigurationSource(ConfigurationSource configurationSource)
            => _configurationSource = configurationSource;

        /// <summary>
        /// Type configuraiton source
        /// </summary>
        /// <returns></returns>
        public virtual ConfigurationSource? GetTypeConfigurationSource() => _typeConfigurationSource;

        /// <summary>
        /// Update type configuration source
        /// </summary>
        /// <param name="configurationSource"></param>
        public virtual void UpdateTypeConfigurationSource(ConfigurationSource configurationSource)
            => _typeConfigurationSource = _typeConfigurationSource.Max(configurationSource);

        /// <summary>
        /// Property indexes
        /// </summary>
        /// <returns></returns>
        public virtual PropertyIndexes PropertyIndexes
        {
            get => CypherNonCapturingLazyInitializer.EnsureInitialized(
                ref _indexes, 
                this,
                property => {
                    var _ = (property.DeclaringType as CypherEntity)?.Counts;
                }
            );

            [param: CanBeNull]
            set
            {
                if (value == null)
                {
                    // This path should only kick in when the model is still mutable and therefore access does not need
                    // to be thread-safe.
                    _indexes = null;
                }
                else
                {
                    NonCapturingLazyInitializer.EnsureInitialized(ref _indexes, value);
                }
            }
        }

        /// <summary>
        /// Is nullable
        /// </summary>
        /// <returns></returns>
        public virtual bool IsNullable
        {
            get => _isNullable ?? ClrType.IsNullableType();
            set => SetIsNullable(value, ConfigurationSource.Explicit);
        }

        /// <summary>
        /// Set is nullable
        /// </summary>
        /// <param name="nullable"></param>
        /// <param name="configurationSource"></param>
        public virtual void SetIsNullable(bool nullable, ConfigurationSource configurationSource) {
            if (nullable) {
                if (!ClrType.IsNullableType()) {
                    throw new InvalidOperationException(CoreStrings.CannotBeNullable(Name, DeclaringEntityType.DisplayName(), ClrType.ShortDisplayName()));
                }
            }

            UpdateIsNullableConfigurationSource(configurationSource);

            var isChanging = IsNullable != nullable;
            _isNullable = nullable;

            if (isChanging) {
                DeclaringEntityType.Graph.CypherConventionDispatcher.OnPropertyNullableChanged(Builder);
            }
        }

        /// <summary>
        /// Get is nullable configuration source
        /// </summary>
        /// <returns></returns>
        public virtual ConfigurationSource? GetIsNullableConfigurationSource() => _isNullableConfigurationSource;

        /// <summary>
        /// Update is nullable configuration source
        /// </summary>
        /// <param name="configurationSource"></param>
        private void UpdateIsNullableConfigurationSource(ConfigurationSource configurationSource)

            => _isNullableConfigurationSource = configurationSource.Max(_isNullableConfigurationSource);

        public ValueGenerated ValueGenerated { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public PropertySaveBehavior BeforeSaveBehavior { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public PropertySaveBehavior AfterSaveBehavior { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public bool IsReadOnlyBeforeSave { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public bool IsReadOnlyAfterSave { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public bool IsStoreGeneratedAlways { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public bool IsConcurrencyToken { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        bool IProperty.IsNullable => throw new System.NotImplementedException();

        PropertySaveBehavior IProperty.BeforeSaveBehavior => throw new System.NotImplementedException();

        PropertySaveBehavior IProperty.AfterSaveBehavior => throw new System.NotImplementedException();

        bool IProperty.IsReadOnlyBeforeSave => throw new System.NotImplementedException();

        bool IProperty.IsReadOnlyAfterSave => throw new System.NotImplementedException();

        bool IProperty.IsStoreGeneratedAlways => throw new System.NotImplementedException();

        ValueGenerated IProperty.ValueGenerated => throw new System.NotImplementedException();

        bool IProperty.IsConcurrencyToken => throw new System.NotImplementedException();
    }
}