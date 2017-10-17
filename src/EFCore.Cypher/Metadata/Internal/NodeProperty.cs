using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    public class NodeProperty : ConventionalAnnotatable, IMutableNodeProperty
    {
        private FieldInfo _fieldInfo;

        private IClrNodePropertyGetter _getter;

        private ConfigurationSource? _configurationSource;

        private ConfigurationSource? _typeConfigurationSource;

        public NodeProperty(
            [NotNull] string name,
            [NotNull] Type clrType,
            [CanBeNull] PropertyInfo propertyInfo,
            [CanBeNull] FieldInfo fieldInfo,
            [NotNull] Node declaringType,
            ConfigurationSource configurationSource,
            ConfigurationSource? typeConfigurationSource
        ) {
            // TODO: Check name not null

            Name = name;
            ClrType = clrType;
            PropertyInfo = propertyInfo;
            _fieldInfo = fieldInfo;
            DeclaringType = declaringType;

            _configurationSource = configurationSource;
            _typeConfigurationSource = typeConfigurationSource;
        }

        /// <summary>
        /// Property name
        /// </summary>
        /// <returns></returns>
        public virtual string Name { get; }

        /// <summary>
        /// Property info
        /// </summary>
        /// <returns></returns>
        public virtual PropertyInfo PropertyInfo { get; }

        /// <summary>
        /// Field info
        /// </summary>
        /// <returns></returns>
        public virtual FieldInfo FieldInfo {
            get => _fieldInfo;
            [param: CanBeNull] set { SetFieldInfo(value, ConfigurationSource.Explicit); }
        }

        /// <summary>
        /// Set field by name
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="configurationSource"></param>
        public virtual void SetField([CanBeNull] string fieldName, ConfigurationSource configurationSource)
        {
            if (fieldName is null) {
                SetFieldInfo(null, configurationSource);
                return;
            }

            if (FieldInfo?.Name == fieldName) {
                SetFieldInfo(FieldInfo, configurationSource);
                return;
            }

            var fieldInfo = GetFieldInfo(fieldName, DeclaringType.ClrType, Name, shouldThrow: true);
            if (!(fieldInfo is null)) {
                SetFieldInfo(fieldInfo, configurationSource);
            }
        }

        /// <summary>
        /// Set field info
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <param name="configurationSource"></param>
        public virtual void SetFieldInfo([CanBeNull] FieldInfo fieldInfo, ConfigurationSource configurationSource)
        {
            // when equal update the dominant configuration source then break
            if (Equals(FieldInfo, fieldInfo)) {
                UpdateFieldInfoConfigurationSource(configurationSource);
                return;
            }

            // when passed field info is not null check if compatible
            if (!(fieldInfo is null)) {
                IsCompatible(fieldInfo, ClrType, DeclaringType.ClrType, Name, shouldThrow: true);
            }

            UpdateFieldInfoConfigurationSource(configurationSource);

            // swap
            var oldFieldInfo = FieldInfo;
            _fieldInfo = fieldInfo;

            PropertyInfoChanged();
            OnFieldInfoSet(oldFieldInfo);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldName"></param>
        /// <param name="type"></param>
        /// <param name="propertyName"></param>
        /// <param name="shouldThrow"></param>
        /// <returns></returns>
        public static FieldInfo GetFieldInfo([NotNull] string fieldName, [NotNull] Type type, [CanBeNull] string propertyName, bool shouldThrow)
        {
            var fieldInfo = type.GetFieldInfo(fieldName);
            if (fieldInfo is null && 
                shouldThrow) {
                throw new InvalidOperationException(
                    CoreCypherStrings.MissingBackingField(fieldName, propertyName, type.ShortDisplayName())
                );
            }

            return fieldInfo;
        }

        /// <summary>
        /// CLR Type
        /// </summary>
        /// <returns></returns>
        public Type ClrType { get; }

        /// <summary>
        /// Internal builder for this node property
        /// </summary>
        /// <returns></returns>
        public virtual InternalNodePropertyBuilder Builder { get; [param: CanBeNull] set; }

        /// <summary>
        /// Declaring type
        /// </summary>
        /// <returns></returns>
        public virtual Node DeclaringType { get; }

        /// <summary>
        /// Declaring type
        /// </summary>
        IMutableNode IMutableNodeProperty.DeclaringType => DeclaringType;

        /// <summary>
        /// Declaring type
        /// </summary>
        INode INodeProperty.DeclaringType => DeclaringType;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual ConfigurationSource? GetConfigurationSource() => _configurationSource;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_getter"></param>
        /// <param name="ClrPropertyGetterFactory("></param>
        /// <returns></returns>
        public virtual IClrNodePropertyGetter Getter
            => NonCapturingLazyInitializer.EnsureInitialized(ref _getter, this, p => new ClrNodePropertyGetterFactory().Create(p));

        public bool IsNullable { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsShadowProperty => throw new NotImplementedException();

        bool INodeProperty.IsNullable => throw new NotImplementedException();

        /// <summary>
        /// Configuration precedence
        /// </summary>
        /// <param name="configurationSource"></param>
        private void UpdateFieldInfoConfigurationSource(ConfigurationSource configurationSource)
            => _configurationSource = configurationSource.Max(_configurationSource);

        /// <summary>
        /// 
        /// </summary>
        protected void PropertyInfoChanged() => DeclaringType.PropertyInfoChanged();

        protected void OnFieldInfoSet(FieldInfo oldFieldInfo) =>
            DeclaringType.Graph.GraphConventionDispatcher.OnPropertyFieldChanged(Builder, oldFieldInfo);

        /// <summary>
        /// False if field info isn't assignable from either the property type or passed node CLR type
        /// </summary>
        /// <returns></returns>
        public static bool IsCompatible(
            [NotNull] FieldInfo fieldInfo,
            [NotNull] Type propertyType,
            [NotNull] Type nodeClrType,
            [CanBeNull] string propertyName,
            bool shouldThrow
        ) {
            var fieldTypeInfo = fieldInfo.FieldType.GetTypeInfo();

            if (!fieldTypeInfo.IsAssignableFrom(propertyType.GetTypeInfo()) &&
                !propertyType.GetTypeInfo().IsAssignableFrom(fieldTypeInfo)) {
                if (shouldThrow) {
                    throw new InvalidOperationException(
                        CoreCypherStrings.BadBackingFieldType(
                            fieldInfo.Name,
                            fieldInfo.FieldType.ShortDisplayName(),
                            nodeClrType.ShortDisplayName(),
                            propertyName,
                            propertyType.ShortDisplayName()
                        )
                    );
                }

                return false;
            }

            if (!fieldInfo.DeclaringType.GetTypeInfo().IsAssignableFrom(nodeClrType.GetTypeInfo())) {
                if (shouldThrow) {
                    throw new InvalidOperationException(
                        CoreStrings.MissingBackingField(
                            fieldInfo.Name, 
                            propertyName, 
                            nodeClrType.ShortDisplayName()
                        )
                    );
                }

                return false;
            }

            return true;
        }
    }
}