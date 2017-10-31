// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    /// <summary>
    /// Entity (<see cref="EntityType" />)
    /// </summary>
    public class CypherEntity : CypherNode, IMutableEntityType
    {
        private CypherEntity _baseType;

        private CypherKey _primaryKey;

        private ConfigurationSource? _baseTypeConfigurationSource;

         private ConfigurationSource? _primaryKeyConfigurationSource;

        private readonly SortedDictionary<string, CypherProperty> _properties;

        private readonly SortedSet<CypherForeignKey> _foreignKeys
            = new SortedSet<CypherForeignKey>(ForeignKeyComparer.Instance);

        private readonly SortedDictionary<string, CypherNavigation> _navigations
            = new SortedDictionary<string, CypherNavigation>(StringComparer.Ordinal);

        private PropertyCounts _propertyCounts;

        /// <summary>
        /// Construct by name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="graph"></param>
        /// <param name="configurationSource"></param>
        public CypherEntity(
            [NotNull] string name, 
            [NotNull] CypherGraph graph, 
            ConfigurationSource configurationSource
        ) : base(name, graph, configurationSource) 
        {
            _properties = new SortedDictionary<string, CypherProperty>(new CypherPropertyComparer(this));
            Builder = new CypherInternalEntityBuilder(this, graph.Builder);
        }

        /// <summary>
        /// Construct by Clr type
        /// </summary>
        /// <param name="clrType"></param>
        /// <param name="graph"></param>
        /// <param name="configurationSource"></param>
        public CypherEntity(
            [NotNull] Type clrType, 
            [NotNull] CypherGraph graph, 
            ConfigurationSource configurationSource
        ) : base(clrType, graph, configurationSource)
        {
            _properties = new SortedDictionary<string, CypherProperty>(new CypherPropertyComparer(this));
            Builder = new CypherInternalEntityBuilder(this, graph.Builder);
        }

        /// <summary>
        /// Construct with defining navigation name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="graph"></param>
        /// <param name="definingNavigationName"></param>
        /// <param name="definingEntity"></param>
        /// <param name="configurationSource"></param>
        /// <returns></returns>
        public CypherEntity(
            [NotNull] string name,
            [NotNull] CypherGraph graph,
            [NotNull] string definingNavigationName,
            [NotNull] CypherEntity definingEntity,
            ConfigurationSource configurationSource
        ) : this(name, graph, configurationSource)
        {
            DefiningNavigationName = definingNavigationName;
            DefiningEntityType = definingEntity;
        }

        /// <summary>
        /// Construct with defining navigation entity
        /// </summary>
        /// <param name="clrType"></param>
        /// <param name="graph"></param>
        /// <param name="definingNavigationName"></param>
        /// <param name="definingEntity"></param>
        /// <param name="configurationSource"></param>
        /// <returns></returns>
        public CypherEntity(
            [NotNull] Type clrType,
            [NotNull] CypherGraph graph,
            [NotNull] string definingNavigationName,
            [NotNull] CypherEntity definingEntity,
            ConfigurationSource configurationSource
        ) : this(clrType, graph, configurationSource)
        {
            DefiningNavigationName = definingNavigationName;
            DefiningEntityType = definingEntity;
        }

        /// <summary>
        /// Internal builder
        /// </summary>
        /// <returns></returns>
        public virtual CypherInternalEntityBuilder Builder { 
            [DebuggerStepThrough] get; 
            [DebuggerStepThrough] [param: CanBeNull] set; 
        }

        /// <summary>
        /// Base type
        /// </summary>
        public virtual CypherEntity BaseType => _baseType;

        /// <summary>
        /// Read-only base type 
        /// </summary>
        IEntityType IEntityType.BaseType => _baseType;

        /// <summary>
        /// Mutable base type
        /// </summary>
        /// <returns></returns>
        IMutableEntityType IMutableEntityType.BaseType {
            get => _baseType;
            set => HasBaseType((CypherEntity)value);
        }
        
        /// <summary>
        /// Query filter
        /// 
        /// TODO: Finish off
        /// </summary>
        /// <returns></returns>
        public virtual LambdaExpression QueryFilter { 
            get => throw new NotImplementedException(); 
            set => throw new NotImplementedException(); 
        }

        /// <summary>
        /// Defining navigation name
        /// </summary>
        /// <returns></returns>
        public virtual string DefiningNavigationName { get; }

        /// <summary>
        /// Defining entity (read)
        /// </summary>
        /// <returns></returns>
        public IEntityType DefiningEntityType { get; }

        /// <summary>
        /// Graph as model
        /// </summary>
        IMutableModel IMutableEntityType.Model => Graph;

        /// <summary>
        /// Query Filter
        /// </summary>
        /// <returns></returns>
        LambdaExpression IMutableEntityType.QueryFilter {
            get => QueryFilter;
            set => QueryFilter = value;
        }

        /// <summary>
        /// Set base type
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="configurationSource"></param>
        public virtual void HasBaseType(
            [CanBeNull] CypherEntity entity,
            ConfigurationSource configurationSource = ConfigurationSource.Explicit
        ) {
            if (_baseType == entity) {
                UpdateBaseTypeConfigurationSource(configurationSource);
                entity?.UpdateConfigurationSource(configurationSource);
                return;
            }

            if (this.HasDefiningNavigation())
            {
                throw new InvalidOperationException(CoreStrings.DependentDerivedType(this.DisplayName()));
            }
            
            var originalBaseType = _baseType;
            _baseType?._directlyDerivedTypes.Remove(this);
            _baseType = null;

            if (!(entity is null)) {
                // with CLR
                if (this.HasClrType()) {
                    if (!entity.HasClrType()) {
                        throw new InvalidOperationException(CoreStrings.NonClrBaseType(this.DisplayName(), entity.DisplayName()));
                    }

                    if (!entity.ClrType.GetTypeInfo().IsAssignableFrom(ClrType.GetTypeInfo()))
                    {
                        throw new InvalidOperationException(CoreStrings.NotAssignableClrBaseType(this.DisplayName(), entity.DisplayName(), ClrType.ShortDisplayName(), entity.ClrType.ShortDisplayName()));
                    }

                    if (entity.HasDefiningNavigation())
                    {
                        throw new InvalidOperationException(CoreStrings.DependentBaseType(this.DisplayName(), entity.DisplayName()));
                    }
                }

                // without Clr but base does
                if (!this.HasClrType() && entity.HasClrType()) {
                    throw new InvalidOperationException(CoreStrings.NonShadowBaseType(this.DisplayName(), entity.DisplayName()));
                }

                // circular inheritance
                if (entity.InheritsFrom(this))
                {
                    throw new InvalidOperationException(CoreStrings.CircularInheritance(this.DisplayName(), entity.DisplayName()));
                }

                // TODO: derived entities can have node keys

                var propertyCollisions = entity.GetProperties()
                    .Select(p => p.Name)
                    .SelectMany(FindDerivedPropertiesInclusive)
                    .ToList();

                if (propertyCollisions.Any())
                {
                    var derivedProperty = propertyCollisions.First();
                    var baseProperty = entity.FindProperty(derivedProperty.Name);
                    throw new InvalidOperationException(
                        CoreStrings.DuplicatePropertiesOnBase(
                            this.DisplayName(),
                            entity.DisplayName(),
                            derivedProperty.DeclaringEntityType.DisplayName(),
                            derivedProperty.Name,
                            baseProperty.DeclaringEntityType.DisplayName(),
                            baseProperty.Name
                        )
                    );
                }


                var navigationCollisions = entity.GetNavigations()
                    .Select(p => p.Name)
                    .SelectMany(FindNavigationsInHierarchy)
                    .ToList();

                if (navigationCollisions.Any())
                {
                    throw new InvalidOperationException(
                        CoreStrings.DuplicateNavigationsOnBase(
                            this.DisplayName(),
                            entity.DisplayName(),
                            string.Join(", ", navigationCollisions.Select(p => p.Name))
                        )
                    );
                }

                _baseType = entity;
                _baseType._directlyDerivedTypes.Add(this);
            }

            PropertyMetadataChanged();
            UpdateBaseTypeConfigurationSource(configurationSource);
            entity?.UpdateBaseTypeConfigurationSource(configurationSource);

            Graph.CypherConventionDispatcher.OnBaseEntityChanged(Builder, originalBaseType);
        }

        /// <summary>
        /// Base type configuration source
        /// </summary>
        /// <returns></returns>
        public virtual ConfigurationSource? GetBaseTypeConfigurationSource() => _baseTypeConfigurationSource;

        /// <summary>
        /// Update base type configuration
        /// </summary>
        /// <param name="configurationSource"></param>
        private void UpdateBaseTypeConfigurationSource(ConfigurationSource configurationSource)
            => _baseTypeConfigurationSource = configurationSource.Max(_baseTypeConfigurationSource);

        /// <summary>
        /// Derived types (shadows any extension method)
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<CypherEntity> GetDerivedTypes()
        {
            var derivedTypes = new List<CypherEntity>();
            var type = this;
            var currentTypeIndex = 0;

            while (type != null)
            {
                derivedTypes.AddRange(type.GetDirectlyDerivedTypes());

                type = derivedTypes.Count > currentTypeIndex
                    ? derivedTypes[currentTypeIndex]
                    : null;

                currentTypeIndex++;
            }

            return derivedTypes;
        }

        /// <summary>
        /// Derived types (inclusive)
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<CypherEntity> GetDerivedTypesInclusive()
            => new[] { this }.Concat(GetDerivedTypes());

        /// <summary>
        /// Directly derived types
        /// </summary>
        /// <returns></returns>
        private readonly SortedSet<CypherEntity> _directlyDerivedTypes = new SortedSet<CypherEntity>(EntityTypePathComparer.Instance);

        public virtual CypherForeignKey AddForeignKey(
            [NotNull] IReadOnlyList<CypherProperty> properties,
            [NotNull] CypherEntity principalEntity,
            ConfigurationSource? configurationSource = ConfigurationSource.Explicit
        ) {
            // TODO: 
            throw new NotImplementedException();
        }

        /// <summary>
        /// Add foreign key (ignores the principle key)
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="principalKey"></param>
        /// <param name="principalEntityType"></param>
        /// <returns></returns>
        public IMutableForeignKey AddForeignKey(
            IReadOnlyList<IMutableProperty> properties, 
            IMutableKey principalKey, 
            IMutableEntityType principalEntityType
        ) => AddForeignKey(properties.Cast<CypherProperty>().ToList(), (CypherEntity)principalEntityType);

        /// <summary>
        /// Declared foreign keys
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<CypherForeignKey> GetDeclaredForeignKeys() => _foreignKeys;

        /// <summary>
        /// Foreign keys matching properties
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        public virtual IEnumerable<CypherForeignKey> FindDeclaredForeignKeys([NotNull] IReadOnlyList<IProperty> properties) {
            Check.NotEmpty(properties, nameof(properties));

            return _foreignKeys.Where(
                fk => PropertyListComparer.Instance.Equals(fk.Properties, properties)
            );
        }

        /// <summary>
        /// Find declared foreign key for a particular principal
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="principalEntity"></param>
        /// <returns></returns>
        public virtual CypherForeignKey FindDeclaredForeignKey(
            [NotNull] IReadOnlyList<IProperty> properties,
            [NotNull] IEntityType principalEntity
        ) {
            Check.NotEmpty(properties, nameof(properties));
            Check.NotNull(principalEntity, nameof(principalEntity));

            return FindDeclaredForeignKeys(properties)
                .SingleOrDefault(fk =>
                    StringComparer.Ordinal.Equals(fk.PrincipalEntityType.Name, principalEntity.Name)
                );
        }

        /// <summary>
        /// Foreign keys of derived types by properties
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        public virtual IEnumerable<CypherForeignKey> FindDerivedForeignKeys(
            [NotNull] IReadOnlyList<IProperty> properties
        ) => GetDerivedTypes().SelectMany(e => e.FindDeclaredForeignKeys(properties));

        /// <summary>
        /// Foreign keys (not null) from derived types
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="principalEntity"></param>
        /// <returns></returns>
        public virtual IEnumerable<CypherForeignKey> FindDerivedForeignKeys(
            [NotNull] IReadOnlyList<IProperty> properties,
            [NotNull] IEntityType principalEntity
        ) => GetDerivedTypes()
            .Select(e => e.FindDeclaredForeignKey(properties, principalEntity))
            .Where(fk => !(fk is null));

        public virtual IEnumerable<CypherForeignKey> FindForeignKeysInHierachy(
            [NotNull] IReadOnlyList<IProperty> properties
        ) => FindForeignKeysInHierachy(properties)
            .Concat(FindDerivedForeignKeys(properties));

        public virtual IEnumerable<CypherForeignKey> FindForeignKeysInHierachy(
            [NotNull] IReadOnlyList<IProperty> properties,
            [NotNull] IEntityType principalEntity
        ) => ToEnumerable(FindForeignKey(properties, principalEntity))
            .Concat(FindDerivedForeignKeys(properties, principalEntity));

        /// <summary>
        /// Find foreign keys by properties
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        public virtual IEnumerable<CypherForeignKey> FindForeignKeys([NotNull] IReadOnlyList<IProperty> properties) {
            Check.HasNoNulls(properties, nameof(properties));
            Check.NotEmpty(properties, nameof(properties));

            return _baseType?
                .FindForeignKeys(properties)?
                .Concat(FindDeclaredForeignKeys(properties)) ?? FindDeclaredForeignKeys(properties);
        }

        /// <summary>
        /// Find foreign key by property and principal
        /// </summary>
        /// <param name="property"></param>
        /// <param name="principalEntity"></param>
        /// <returns></returns>
        public virtual CypherForeignKey FindForeignKey(
            [NotNull] IProperty property,
            [NotNull] IEntityType principalEntity
        ) => FindForeignKey(new[] { property }, principalEntity);

        /// <summary>
        /// Find foreign key by properties and prinicipal
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="principalEntity"></param>
        /// <returns></returns>
        public virtual CypherForeignKey FindForeignKey(
            [NotNull] IReadOnlyList<IProperty> properties,
            [NotNull] IEntityType principalEntity
        ) {
            Check.HasNoNulls(properties, nameof(properties));
            Check.NotEmpty(properties, nameof(properties));
            Check.NotNull(principalEntity, nameof(principalEntity));

            return FindDeclaredForeignKey(properties,  principalEntity)
                   ?? _baseType?.FindForeignKey(properties, principalEntity);
        }

        /// <summary>
        /// Foreign keys from derived types
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<CypherForeignKey> GetDerivedForeignKeys()
            => GetDerivedTypes().SelectMany(e => e.GetDeclaredForeignKeys());

        /// <summary>
        /// Referencing foreign keys
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<CypherForeignKey> GetReferencingForeignKeys()
            => _baseType?
                .GetReferencingForeignKeys()
                .Concat(GetDeclaredReferencingForeignKeys()) ?? GetDeclaredReferencingForeignKeys();

        /// <summary>
        /// Get decleared refencing foreign keys
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<CypherForeignKey> GetDeclaredReferencingForeignKeys()
            => DeclaredReferencingForeignKeys ?? Enumerable.Empty<CypherForeignKey>();

        /// <summary>
        /// Declared referencing foroeign keys
        /// </summary>
        /// <returns></returns>
        private SortedSet<CypherForeignKey> DeclaredReferencingForeignKeys { get; set; }

        public IMutableIndex AddIndex(IReadOnlyList<IMutableProperty> properties)
        {
            throw new NotImplementedException();
        }

        public IMutableKey AddKey(IReadOnlyList<IMutableProperty> properties)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Add property
        /// </summary>
        /// <param name="name"></param>
        /// <param name="clrType"></param>
        /// <param name="configurationSource"></param>
        /// <param name="typeConfigurationSource"></param>
        /// <returns></returns>
        public virtual CypherProperty AddProperty(
            [NotNull] string name,
            [CanBeNull] Type clrType = null,
            ConfigurationSource configurationSource = ConfigurationSource.Explicit,
            ConfigurationSource? typeConfigurationSource = ConfigurationSource.Explicit
        ) {
            Check.NotNull(name, nameof(name));

            ValidateCanAddProperty(name);

            return AddProperty(
                name, 
                clrType, 
                ClrType?.GetMembersInHierarchy(name).FirstOrDefault(), 
                configurationSource, 
                typeConfigurationSource
            );
        }

        /// <summary>
        /// Add property
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <param name="configurationSource"></param>
        /// <returns></returns>
        public virtual CypherProperty AddProperty(
            [NotNull] MemberInfo memberInfo,
            ConfigurationSource configurationSource = ConfigurationSource.Explicit
        ) {
            Check.NotNull(memberInfo, nameof(memberInfo));

            ValidateCanAddProperty(memberInfo.Name);

            if (ClrType is null)
            {
                throw new InvalidOperationException(CoreStrings.ClrPropertyOnShadowEntity(memberInfo.Name, this.DisplayName()));
            }

            if (memberInfo.DeclaringType is null
                || !memberInfo.DeclaringType.GetTypeInfo().IsAssignableFrom(ClrType.GetTypeInfo()))
            {
                throw new ArgumentException(
                    CoreStrings.PropertyWrongEntityClrType(
                        memberInfo.Name, this.DisplayName(), memberInfo.DeclaringType?.ShortDisplayName()));
            }

            return AddProperty(
                memberInfo.Name, 
                memberInfo.GetMemberType(), 
                memberInfo, 
                configurationSource, 
                configurationSource
            );
        }

        /// <summary>
        /// Define property
        /// </summary>
        /// <remarks>Fire convention event</remarks>
        /// <param name="name"></param>
        /// <param name="clrType"></param>
        /// <param name="memberInfo"></param>
        /// <param name="configurationSource"></param>
        /// <param name="typeConfigurationSource"></param>
        /// <returns></returns>
        private CypherProperty AddProperty(
            string name,
            Type clrType,
            MemberInfo memberInfo,
            ConfigurationSource configurationSource,
            ConfigurationSource? typeConfigurationSource
        ) {
            Check.NotNull(name, nameof(name));

            if (clrType == null)
            {
                if (memberInfo == null)
                {
                    throw new InvalidOperationException(CoreStrings.NoPropertyType(name, this.DisplayName()));
                }

                clrType = memberInfo.GetMemberType();
            }
            else
            {
                if (!(memberInfo is null) && clrType != memberInfo.GetMemberType())
                {
                    throw new InvalidOperationException(
                        CoreStrings.PropertyWrongClrType(
                            name,
                            this.DisplayName(),
                            memberInfo.GetMemberType().ShortDisplayName(),
                            clrType.ShortDisplayName()
                        )
                    );
                }
            }

            var property = new CypherProperty(
                name, 
                clrType, 
                memberInfo as PropertyInfo, 
                memberInfo as FieldInfo, 
                this, 
                configurationSource, 
                typeConfigurationSource
            );

            _properties.Add(property.Name, property);
            PropertyMetadataChanged();

            return Graph.CypherConventionDispatcher.OnPropertyAdded(property.Builder)?.Metadata;
        }

        /// <summary>
        /// Add property (mutable)
        /// </summary>
        /// <param name="name"></param>
        /// <param name="propertyType"></param>
        /// <returns></returns>
        IMutableProperty IMutableEntityType.AddProperty(string name, Type propertyType) => AddProperty(name, propertyType);

        /// <summary>
        /// Invalid operation
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="principalKey"></param>
        /// <param name="principalEntityType"></param>
        /// <returns></returns>
        public IMutableForeignKey FindForeignKey(IReadOnlyList<IProperty> properties, IKey principalKey, IEntityType principalEntityType)
            => throw new InvalidOperationException("Graph entities do not have foreign keys");

        public IMutableIndex FindIndex(IReadOnlyList<IProperty> properties)
        {
            throw new NotImplementedException();
        }

        public IMutableKey FindKey(IReadOnlyList<IProperty> properties)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Declared primary key
        /// </summary>
        /// <returns></returns>
        public virtual CypherKey FindDeclaredPrimaryKey() => _primaryKey;

        /// <summary>
        /// Find primary key
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        public virtual CypherKey FindPrimaryKey([CanBeNull] IReadOnlyList<CypherProperty> properties) {
            Check.HasNoNulls(properties, nameof(properties));
            Check.NotEmpty(properties, nameof(properties));

            if (_baseType != null)
            {
                return _baseType.FindPrimaryKey(properties);
            }

            if (_primaryKey != null
                && PropertyListComparer.Instance.Compare(_primaryKey.Properties, properties) == 0)
            {
                return _primaryKey;
            }

            return null;
        }

        /// <summary>
        /// Find primary key (either from base or the declaring)
        /// </summary>
        /// <returns></returns>
        public virtual CypherKey FindPrimaryKey()
            => _baseType?.FindPrimaryKey() ?? FindDeclaredPrimaryKey();

        /// <summary>
        /// Mutable primary key
        /// </summary>
        /// <returns></returns>
        IMutableKey IMutableEntityType.FindPrimaryKey() => FindPrimaryKey();

        /// <summary>
        /// Read-only primary key
        /// </summary>
        /// <returns></returns>
        IKey IEntityType.FindPrimaryKey() => FindPrimaryKey();

        /// <summary>
        /// Find property by info
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        public virtual CypherProperty FindProperty([NotNull] PropertyInfo propertyInfo) 
            => FindProperty(propertyInfo.Name);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual CypherProperty FindProperty([NotNull] string name)
            => FindDeclaredProperty(Check.NotEmpty(name, nameof(name))) ?? _baseType?.FindProperty(name);

        /// <summary>
        /// Find (mutable) property
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IMutableProperty IMutableEntityType.FindProperty(string name) => FindProperty(name);

        IProperty IEntityType.FindProperty(string name) => FindProperty(name);

        /// <summary>
        /// Invalid operations
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IMutableForeignKey> GetForeignKeys()
            => throw new NotImplementedException();

        public IEnumerable<IMutableIndex> GetIndexes()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IMutableKey> GetKeys()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Base properties with self properties else just self properties
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<CypherProperty> GetProperties()
            => _baseType?.GetProperties().Concat(_properties.Values) ?? _properties.Values;

        /// <summary>
        /// Mutable properties
        /// </summary>
        /// <returns></returns>
        IEnumerable<IMutableProperty> IMutableEntityType.GetProperties() => GetProperties();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<IProperty> IEntityType.GetProperties() => GetProperties();

        /// <summary>
        /// Invalid operation
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="principalKey"></param>
        /// <param name="principalEntityType"></param>
        /// <returns></returns>
        public IMutableForeignKey RemoveForeignKey(IReadOnlyList<IProperty> properties, IKey principalKey, IEntityType principalEntityType)
            => throw new InvalidOperationException("Graph entities do not have foreign keys");

        public IMutableIndex RemoveIndex(IReadOnlyList<IProperty> properties)
        {
            throw new NotImplementedException();
        }

        public IMutableKey RemoveKey(IReadOnlyList<IProperty> properties)
        {
            throw new NotImplementedException();
        }

        public IMutableProperty RemoveProperty(string name)
        {
            throw new NotImplementedException();
        }

        public IMutableKey SetPrimaryKey(IReadOnlyList<IMutableProperty> properties)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Invalid operation
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="principalKey"></param>
        /// <param name="principalEntityType"></param>
        /// <returns></returns>
        IForeignKey IEntityType.FindForeignKey(IReadOnlyList<IProperty> properties, IKey principalKey, IEntityType principalEntityType)
            => FindForeignKey(properties, principalKey, principalEntityType);

        IIndex IEntityType.FindIndex(IReadOnlyList<IProperty> properties)
        {
            throw new NotImplementedException();
        }

        IKey IEntityType.FindKey(IReadOnlyList<IProperty> properties)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Invalid operation
        /// </summary>
        /// <returns></returns>
        IEnumerable<IForeignKey> IEntityType.GetForeignKeys() => GetForeignKeys();

        IEnumerable<IIndex> IEntityType.GetIndexes()
        {
            throw new NotImplementedException();
        }

        IEnumerable<IKey> IEntityType.GetKeys()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public override void OnTypeMemberIgnored(string name)
            => throw new NotImplementedException();

        /// <summary>
        /// Wipe out property indexes and property counts
        /// </summary>
        public override void PropertyMetadataChanged()
        {
            foreach (var property in GetProperties())
            {
                property.PropertyIndexes = null;
            }

            foreach (var navigation in GetNavigations())
            {
                navigation.PropertyIndexes = null;
            }

            _propertyCounts = null;
        }
        
        /// <summary>
        /// Declared properties
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<CypherProperty> GetDeclaredProperties() => _properties.Values;

        /// <summary>
        /// Property Counts
        /// </summary>
        /// <returns></returns>
        public virtual PropertyCounts Counts
            => NonCapturingLazyInitializer.EnsureInitialized(
                ref _propertyCounts, 
                this, 
                entity => entity.CalculateCounts()
            );

        /// <summary>
        /// Directly derived types
        /// </summary>
        /// <returns></returns>
        public virtual ISet<CypherEntity> GetDirectlyDerivedTypes() => _directlyDerivedTypes;

        /// <summary>
        /// Navigations
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<CypherNavigation> GetNavigations()
            => _baseType?.GetNavigations().Concat(_navigations.Values) ?? _navigations.Values;

        /// <summary>
        /// Find declared property by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual CypherProperty FindDeclaredProperty([NotNull] string name)
            => _properties.TryGetValue(Check.NotEmpty(name, nameof(name)), out var property)
                ? property
                : null;

        /// <summary>
        /// Find derived properties by name
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public virtual IEnumerable<CypherProperty> FindDerivedProperties([NotNull] string name)
        {
            Check.NotNull(name, nameof(name));

            return this.GetDerivedTypes()
                .Select(et => et.FindDeclaredProperty(name))
                .Where(p => p != null);
        }

        /// <summary>
        /// Find properties in hierachy
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public virtual IEnumerable<CypherProperty> FindPropertiesInHierarchy([NotNull] string propertyName)
            => ToEnumerable(FindProperty(propertyName)).Concat(FindDerivedProperties(propertyName));

        /// <summary>
        /// Find devired properties by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual IEnumerable<CypherProperty> FindDerivedPropertiesInclusive([NotNull] string name)
            => ToEnumerable(FindDeclaredProperty(name)).Concat(FindDerivedProperties(name));

        /// <summary>
        /// Find declared navigation by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual CypherNavigation FindDeclaredNavigation([NotNull] string name)
            => _navigations.TryGetValue(Check.NotEmpty(name, nameof(name)), out var navigation)
                ? navigation
                : null;

        /// <summary>
        /// Find navigation
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual CypherNavigation FindNavigation([NotNull] string name)
        {
            Check.NotEmpty(name, nameof(name));

            return FindDeclaredNavigation(name) ?? _baseType?.FindNavigation(name);
        }

        /// <summary>
        /// Find derived navigations by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual IEnumerable<CypherNavigation> FindDerivedNavigations([NotNull] string name)
        {
            Check.NotNull(name, nameof(name));

            return this.GetDerivedTypes()
                .Select(et => et.FindDeclaredNavigation(name))
                .Where(n => n != null);
        }

        /// <summary>
        /// Find navigations in type hierachy
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual IEnumerable<CypherNavigation> FindNavigationsInHierarchy([NotNull] string name)
            => ToEnumerable(FindNavigation(name)).Concat(FindDerivedNavigations(name));

        /// <summary>
        /// Helper
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private static IEnumerable<T> ToEnumerable<T>(T element)
            where T : class
            => element == null ? Enumerable.Empty<T>() : new[] { element };

        /// <summary>
        /// Can add property
        /// </summary>
        /// <param name="name"></param>
        private void ValidateCanAddProperty(string name)
        {
            var duplicateProperty = FindPropertiesInHierarchy(name).FirstOrDefault();
            if (duplicateProperty != null)
            {
                throw new InvalidOperationException(
                    CoreStrings.DuplicateProperty(
                        name, this.DisplayName(), duplicateProperty.DeclaringEntityType.DisplayName()));
            }

            var duplicateNavigation = FindNavigationsInHierarchy(name).FirstOrDefault();
            if (duplicateNavigation != null)
            {
                throw new InvalidOperationException(
                    CoreStrings.ConflictingNavigation(
                        name, this.DisplayName(),
                        duplicateNavigation.DeclaringEntityType.DisplayName()));
            }
        }

        /// <summary>
        /// Remove navigation
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual CypherNavigation RemoveNavigation([NotNull] string name) {
            Check.NotEmpty(name, nameof(name));

            var navigation = FindDeclaredNavigation(name);
            if (navigation is null) {
                return null;
            }

            _navigations.Remove(name);
            PropertyMetadataChanged();

            return navigation;
        }

        /// <summary>
        /// Add navigation
        /// </summary>
        /// <param name="name"></param>
        /// <param name="foreignKey"></param>
        /// <param name="pointsToPrincipal"></param>
        /// <returns></returns>
        public virtual CypherNavigation AddNavigation(
            [NotNull] string name,
            [NotNull] CypherForeignKey foreignKey,
            bool pointsToPrincipal
        ) {
            Check.NotEmpty(name, nameof(name));
            Check.NotNull(foreignKey, nameof(foreignKey));

            return AddNavigation(new PropertyIdentity(name), foreignKey, pointsToPrincipal);
        }

        /// <summary>
        /// Add navigation
        /// </summary>
        /// <param name="navigationProperty"></param>
        /// <param name="foreignKey"></param>
        /// <param name="pointsToPrincipal"></param>
        /// <returns></returns>
        public virtual CypherNavigation AddNavigation(
            [NotNull] PropertyInfo navigationProperty,
            [NotNull] CypherForeignKey foreignKey,
            bool pointsToPrincipal
        ) {
            Check.NotNull(navigationProperty, nameof(navigationProperty));
            Check.NotNull(foreignKey, nameof(foreignKey));

            return AddNavigation(new PropertyIdentity(navigationProperty), foreignKey, pointsToPrincipal);
        }

        /// <summary>
        /// Add navigation
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="foreignKey"></param>
        /// <param name="pointsToPrincipal"></param>
        /// <returns></returns>
        private CypherNavigation AddNavigation(
            PropertyIdentity identity,
            CypherForeignKey foreignKey,
            bool pointsToPrincipal
        ) {
            var name = identity.Name;
            var duplicateNavigation = FindNavigationsInHierarchy(name).FirstOrDefault();

            if (!(duplicateNavigation is null)) {
                if (duplicateNavigation.ForeignKey != foreignKey) {
                    throw new InvalidOperationException(
                        CoreStrings.NavigationForWrongForeignKey(
                            duplicateNavigation.Name,
                            duplicateNavigation.DeclaringEntityType.DisplayName(),
                            Property.Format(foreignKey.Properties),
                            Property.Format(duplicateNavigation.ForeignKey.Properties)));
                }

                throw new InvalidOperationException(
                    CoreStrings.DuplicateNavigation(
                        name, 
                        this.DisplayName(), 
                        duplicateNavigation.DeclaringEntityType.DisplayName()
                    )
                );
            }

            var duplicateProperty = FindPropertiesInHierarchy(name).FirstOrDefault();
            if (!(duplicateProperty is null)) {
                throw new InvalidOperationException(
                    CoreStrings.ConflictingProperty(
                        name, this.DisplayName(),
                        duplicateProperty.DeclaringEntityType.DisplayName()
                    )
                );
            }

            var navigationProperty = identity.Property;
            if (!(ClrType is null)) {

            }

            var navigation = new CypherNavigation(name, identity.Property, null, foreignKey);

            _navigations.Add(name, navigation);
            PropertyMetadataChanged();

            return navigation;
        }
    }
}