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
    public class Entity : Node, IMutableEntityType
    {
        private Entity _baseType;

        private ConfigurationSource? _baseTypeConfigurationSource;

        private readonly SortedDictionary<string, Property> _properties;

        private readonly SortedDictionary<string, Navigation> _navigations
            = new SortedDictionary<string, Navigation>(StringComparer.Ordinal);

        private PropertyCounts _propertyCounts;


        public Entity([NotNull] string name, [NotNull] Graph graph, ConfigurationSource configurationSource)
            : base(name, graph, configurationSource) 
        {
            _properties = new SortedDictionary<string, Property>(new PropertyComparer(this));
            Builder = new InternalEntityBuilder(this, graph.Builder);
        }

        public Entity([NotNull] Type clrType, [NotNull] Graph graph, ConfigurationSource configurationSource)
            : base(clrType, graph, configurationSource)
        {
            _properties = new SortedDictionary<string, Property>(new PropertyComparer(this));
            Builder = new InternalEntityBuilder(this, graph.Builder);
        }

        public Entity(
            [NotNull] string name,
            [NotNull] Graph graph,
            [NotNull] string definingNavigationName,
            [NotNull] Entity definingEntity,
            ConfigurationSource configurationSource
        ) : this(name, graph, configurationSource)
        {

        }

        public Entity(
            [NotNull] Type clrType,
            [NotNull] Graph graph,
            [NotNull] string definingNavigationName,
            [NotNull] Entity definingEntity,
            ConfigurationSource configurationSource
        ) : this(clrType, graph, configurationSource)
        {

        }

        /// <summary>
        /// Internal builder
        /// </summary>
        /// <returns></returns>
        public virtual InternalEntityBuilder Builder { 
            [DebuggerStepThrough] get; 
            [DebuggerStepThrough] [param: CanBeNull] set; 
        }

        /// <summary>
        /// Base type
        /// </summary>
        public virtual Entity BaseType => _baseType;

        /// <summary>
        /// Base type
        /// </summary>
        IEntityType IEntityType.BaseType => _baseType;

        IMutableEntityType IMutableEntityType.BaseType {
            get => _baseType;
            set => HasBaseType((Entity)value);
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
        /// 
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

        public virtual void HasBaseType(
            [CanBeNull] Entity entity,
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

                PropertyMetadataChanged();
                UpdateBaseTypeConfigurationSource(configurationSource);
                entity?.UpdateBaseTypeConfigurationSource(configurationSource);

                Graph.GraphConventionDispatcher.OnBaseEntityChanged(Builder, originalBaseType);
            }
        }

        /// <summary>
        /// 
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
        /// Directly derived types
        /// </summary>
        /// <returns></returns>
        private readonly SortedSet<Entity> _directlyDerivedTypes = new SortedSet<Entity>(EntityTypePathComparer.Instance);

        /// <summary>
        /// Invalid operation
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="principalKey"></param>
        /// <param name="principalEntityType"></param>
        /// <returns></returns>
        public IMutableForeignKey AddForeignKey(IReadOnlyList<IMutableProperty> properties, IMutableKey principalKey, IMutableEntityType principalEntityType)
            => throw new InvalidOperationException("Graph entities do not have foreign keys");

        public IMutableIndex AddIndex(IReadOnlyList<IMutableProperty> properties)
        {
            throw new NotImplementedException();
        }

        public IMutableKey AddKey(IReadOnlyList<IMutableProperty> properties)
        {
            throw new NotImplementedException();
        }

        public IMutableProperty AddProperty(string name, Type propertyType)
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

        public IMutableKey FindPrimaryKey()
        {
            throw new NotImplementedException();
        }

        public IMutableProperty FindProperty(string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Invalid operations
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IMutableForeignKey> GetForeignKeys()
            => throw new InvalidOperationException("Graph entities do not have foreign keys");

        public IEnumerable<IMutableIndex> GetIndexes()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IMutableKey> GetKeys()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<Property> GetProperties()
            => _baseType?.GetProperties().Concat(_properties.Values) ?? _properties.Values;

        /// <summary>
        /// 
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

        IKey IEntityType.FindPrimaryKey()
        {
            throw new NotImplementedException();
        }

        IProperty IEntityType.FindProperty(string name)
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
        /// Property Counts
        /// </summary>
        /// <param name="_counts"></param>
        /// <param name="entityType.CalculateCounts("></param>
        /// <returns></returns>
        public virtual PropertyCounts Counts
            => NonCapturingLazyInitializer.EnsureInitialized(ref _propertyCounts, this, entity => entity.CalculateCounts());

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual ISet<Entity> GetDirectlyDerivedTypes() => _directlyDerivedTypes;

        /// <summary>
        /// Navigations
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<Navigation> GetNavigations()
            => _baseType?.GetNavigations().Concat(_navigations.Values) ?? _navigations.Values;

        /// <summary>
        /// Find declared property by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual Property FindDeclaredProperty([NotNull] string name)
            => _properties.TryGetValue(Check.NotEmpty(name, nameof(name)), out var property)
                ? property
                : null;

        /// <summary>
        /// Find derived properties by name
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public virtual IEnumerable<Property> FindDerivedProperties([NotNull] string name)
        {
            Check.NotNull(name, nameof(name));

            return this.GetDerivedTypes()
                .Select(et => et.FindDeclaredProperty(name))
                .Where(p => p != null);
        }

        /// <summary>
        /// Find devired properties by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual IEnumerable<Property> FindDerivedPropertiesInclusive([NotNull] string name)
            => ToEnumerable(FindDeclaredProperty(name)).Concat(FindDerivedProperties(name));

        /// <summary>
        /// Find declared navigation by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual Navigation FindDeclaredNavigation([NotNull] string name)
            => _navigations.TryGetValue(Check.NotEmpty(name, nameof(name)), out var navigation)
                ? navigation
                : null;

        /// <summary>
        /// Find navigation
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual Navigation FindNavigation([NotNull] string name)
        {
            Check.NotEmpty(name, nameof(name));

            return FindDeclaredNavigation(name) ?? _baseType?.FindNavigation(name);
        }

        /// <summary>
        /// Find derived navigations by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual IEnumerable<Navigation> FindDerivedNavigations([NotNull] string name)
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
        public virtual IEnumerable<Navigation> FindNavigationsInHierarchy([NotNull] string name)
            => ToEnumerable(FindNavigation(name)).Concat(FindDerivedNavigations(name));

        /// <summary>
        /// Helper
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private static IEnumerable<T> ToEnumerable<T>(T element)
            where T : class
            => element == null ? Enumerable.Empty<T>() : new[] { element };
    }
}