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
        private ConfigurationSource _configurationSource;

        private ConfigurationSource? _typeConfigurationSource;

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
        /// Declaring entity (mutable)
        /// </summary>
        IMutableEntityType IMutableProperty.DeclaringEntityType => DeclaringEntityType;

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
            get => NonCapturingLazyInitializer.EnsureInitialized(ref _indexes, this,
                property => property.DeclaringType.CalculateIndexes(property));

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

        public bool IsNullable { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public ValueGenerated ValueGenerated { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public PropertySaveBehavior BeforeSaveBehavior { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public PropertySaveBehavior AfterSaveBehavior { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public bool IsReadOnlyBeforeSave { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public bool IsReadOnlyAfterSave { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public bool IsStoreGeneratedAlways { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public bool IsConcurrencyToken { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        IEntityType IProperty.DeclaringEntityType => throw new System.NotImplementedException();

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