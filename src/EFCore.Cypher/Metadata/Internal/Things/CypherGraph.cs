// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    /// <summary>
    /// Graph
    /// </summary>
    public class CypherGraph : ConventionalAnnotatable, IMutableModel
    {
        private readonly SortedDictionary<string, CypherEntity> _entities
            = new SortedDictionary<string, CypherEntity>();

        private readonly IDictionary<Type, CypherEntity> _clrTypeMap
            = new Dictionary<Type, CypherEntity>();

        private readonly SortedDictionary<string, SortedSet<CypherEntity>> _entitiesWithDefiningNavigation
            = new SortedDictionary<string, SortedSet<CypherEntity>>();

        private readonly Dictionary<string, ConfigurationSource> _ignorables
            = new Dictionary<string, ConfigurationSource>();

        /// <summary>
        /// With empty graph convention set
        /// </summary>
        public CypherGraph(): this(new CypherConventionSet()) {

        }

        /// <summary>
        /// With conventions
        /// </summary>
        /// <param name="conventions"></param>
        public CypherGraph([NotNull] CypherConventionSet conventions) {
            var dispatcher = new CypherConventionDispatcher(conventions);
            var builder = new CypherInternalGraphBuilder(this);

            CypherConventionDispatcher = dispatcher;
            Builder = builder;
            dispatcher.OnGraphInitialized(builder);
        }

        /// <summary>
        /// Convention dispatcher
        /// </summary>
        /// <returns></returns>
        public virtual CypherConventionDispatcher CypherConventionDispatcher { [DebuggerStepThrough] get; }

        /// <summary>
        /// Builder
        /// </summary>
        /// <returns></returns>
        public virtual CypherInternalGraphBuilder Builder { [DebuggerStepThrough] get; }      

        /// <summary>
        /// Add entity 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="configurationSource"></param>
        /// <returns></returns>
        public virtual CypherEntity AddEntity(
            [NotNull] string name, 
            ConfigurationSource configurationSource = ConfigurationSource.Explicit
        ) {
            Check.NotEmpty(name, nameof(name));

            var entity = new CypherEntity(name, this, configurationSource);
            return AddEntity(entity);
        }

        /// <summary>
        /// Add entity type
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IMutableEntityType IMutableModel.AddEntityType(string name) => AddEntity(name);

        /// <summary>
        /// Add entity 
        /// </summary>
        /// <param name="clrType"></param>
        /// <param name="configurationSource"></param>
        /// <returns></returns>
        public virtual CypherEntity AddEntity(
            [NotNull] Type clrType,
            ConfigurationSource configurationSource = ConfigurationSource.Explicit
        ) {
            Check.NotNull(clrType, nameof(clrType));

            var entity = new CypherEntity(clrType, this, configurationSource);

            _clrTypeMap[clrType] = entity;
            return AddEntity(entity);
        }

        /// <summary>
        /// Add entity type
        /// </summary>
        /// <param name="clrType"></param>
        /// <returns></returns>
        IMutableEntityType IMutableModel.AddEntityType(Type clrType) => AddEntity(clrType);

        /// <summary>
        /// When entity has a defining navigation lookup in the navigation set
        /// otherwise go the entities set adding if not found
        /// </summary>
        /// <remarks>Fires convention event</remarks>
        /// <param name="entity"></param>
        /// <returns></returns>
        private CypherEntity AddEntity(CypherEntity entity) {
            var name = entity.Name;

            if (entity.HasDefiningNavigation()) {
                if (_entities.ContainsKey(name)) {
                    throw new InvalidOperationException(CoreStrings.ClashingNonDependentEntityType(entity.DisplayName()));
                }

                if (!_entitiesWithDefiningNavigation.TryGetValue(name, out var withSameType)) {
                    withSameType = new SortedSet<CypherEntity>(EntityTypePathComparer.Instance);
                    _entitiesWithDefiningNavigation[name] = withSameType;
                }

                withSameType.Add(entity);
            } else {
                if (_entitiesWithDefiningNavigation.ContainsKey(name)) {
                    throw new InvalidOperationException(CoreStrings.ClashingDependentEntityType(entity.DisplayName()));
                }

                int howManyEntities = _entities.Count;
                _entities[name] = entity;

                if (howManyEntities == _entities.Count) {
                    throw new InvalidOperationException(CoreStrings.DuplicateEntityType(entity.DisplayName()));
                }
            }

            return CypherConventionDispatcher.OnEntityAdded(entity.Builder)?.Metadata;
        }

        /// <summary>
        /// Add entity through another
        /// </summary>
        /// <param name="name"></param>
        /// <param name="definingNavigationName"></param>
        /// <param name="definingEntity"></param>
        /// <param name="configurationSource"></param>
        /// <returns></returns>
        public virtual CypherEntity AddEntity(
            [NotNull] string name,
            [NotNull] string definingNavigationName,
            [NotNull] CypherEntity definingEntity,
            ConfigurationSource configurationSource = ConfigurationSource.Explicit
        ) {
            Check.NotEmpty(name, nameof(name));

            var entity = new CypherEntity(name, this, definingNavigationName, definingEntity, configurationSource);
            return AddEntity(entity);
        }

        /// <summary>
        /// Add entity through another
        /// </summary>
        /// <param name="name"></param>
        /// <param name="definingNavigationName"></param>
        /// <param name="definingEntityType"></param>
        /// <returns></returns>
        IMutableEntityType IMutableModel.AddEntityType(string name, string definingNavigationName, IMutableEntityType definingEntityType) =>
            AddEntity(name, definingNavigationName, (CypherEntity)definingEntityType);

        /// <summary>
        /// Add entity through another
        /// </summary>
        /// <param name="clrType"></param>
        /// <param name="definingNavigationName"></param>
        /// <param name="definingEntity"></param>
        /// <param name="configurationSource"></param>
        /// <returns></returns>
        public virtual CypherEntity AddEntity(
            [NotNull] Type clrType,
            [NotNull] string definingNavigationName,
            [NotNull] CypherEntity definingEntity,
            ConfigurationSource configurationSource = ConfigurationSource.Explicit
        ) {
            Check.NotNull(clrType, nameof(clrType));

            var entity = new CypherEntity(clrType, this, definingNavigationName, definingEntity, configurationSource);
            return AddEntity(entity);
        }

        /// <summary>
        /// Add entity through another
        /// </summary>
        /// <param name="clrType"></param>
        /// <param name="definingNavigationName"></param>
        /// <param name="definingEntityType"></param>
        /// <returns></returns>
        IMutableEntityType IMutableModel.AddEntityType(Type clrType, string definingNavigationName, IMutableEntityType definingEntityType) =>
            AddEntity(clrType, definingNavigationName, (CypherEntity)definingEntityType);

        /// <summary>
        /// Find entity by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual CypherEntity FindEntity([NotNull] string name)
            => _entities.TryGetValue(Check.NotEmpty(name, nameof(name)), out var entity)
                ? entity
                : null;


        /// <summary>
        /// Find entity type by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IMutableEntityType IMutableModel.FindEntityType(string name) => FindEntity(name);

        /// <summary>
        /// Find entity by type
        /// </summary>
        /// <param name="clrType"></param>
        /// <returns></returns>
        public virtual CypherEntity FindEntity([NotNull] Type clrType)
            => _clrTypeMap.TryGetValue(Check.NotNull(clrType, nameof(clrType)), out var entityType)
                ? entityType
                : FindEntity(clrType.DisplayName());

        /// <summary>
        /// Find entity by Clr type
        /// </summary>
        /// <param name="clrType"></param>
        /// <param name="definingNavigationName"></param>
        /// <param name="definingEntity"></param>
        /// <returns></returns>
        public virtual CypherEntity FindEntity(
            [NotNull] Type clrType,
            [NotNull] string definingNavigationName,
            [NotNull] CypherEntity definingEntity
        ) => FindEntity(clrType.DisplayName(), definingNavigationName, definingEntity);

        /// <summary>
        /// Find entity by navigation
        /// </summary>
        /// <param name="name"></param>
        /// <param name="definingNavigationName"></param>
        /// <param name="definingEntity"></param>
        /// <returns></returns>
        public virtual CypherEntity FindEntity(
            [NotNull] string name,
            [NotNull] string definingNavigationName,
            [NotNull] CypherEntity definingEntity
        ) {
            if (!_entitiesWithDefiningNavigation.TryGetValue(name, out var withSameType)) {
                return null;
            }

            return withSameType.FirstOrDefault(
                e => e.DefiningNavigationName == definingNavigationName &&
                    e.DefiningEntityType == definingEntity
            );
        }

        /// <summary>
        /// Find (read) entity type by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IEntityType IModel.FindEntityType(string name) => FindEntity(name);

        /// <summary>
        /// Find entity type by navigation
        /// </summary>
        /// <param name="name"></param>
        /// <param name="definingNavigationName"></param>
        /// <param name="definingEntityType"></param>
        /// <returns></returns>
        IMutableEntityType IMutableModel.FindEntityType(string name, string definingNavigationName, IMutableEntityType definingEntityType) =>
            FindEntity(name, definingNavigationName, (CypherEntity)definingEntityType);

        /// <summary>
        /// Find entity type by navigation
        /// </summary>
        /// <param name="name"></param>
        /// <param name="definingNavigationName"></param>
        /// <param name="definingEntityType"></param>
        /// <returns></returns>
        IEntityType IModel.FindEntityType(string name, string definingNavigationName, IEntityType definingEntityType) =>
            FindEntity(name, definingNavigationName, (CypherEntity)definingEntityType);

        /// <summary>
        /// Get entities
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<CypherEntity> GetEntities() =>
            _entities.Values.Concat(_entitiesWithDefiningNavigation.Values.SelectMany(e => e));

        /// <summary>
        /// Get entities by Clr type
        /// </summary>
        /// <param name="clrType"></param>
        /// <returns></returns>
        public virtual IReadOnlyCollection<CypherEntity> GetEntities([NotNull] Type clrType) =>
            GetEntities(clrType.DisplayName());

        /// <summary>
        /// Get entities by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual IReadOnlyCollection<CypherEntity> GetEntities([NotNull] string name) {
            if (_entitiesWithDefiningNavigation.TryGetValue(name, out var withSameType)) {
                return withSameType;
            }

            var entity = FindEntity(name);
            return entity is null
                ? new CypherEntity[0]
                : new[] { entity };
        }

        /// <summary>
        /// Get (read) entity types
        /// </summary>
        /// <returns></returns>
        IEnumerable<IEntityType> IModel.GetEntityTypes() => GetEntities();

        /// <summary>
        /// Get (mutable) entity types
        /// </summary>
        /// <returns></returns>
        IEnumerable<IMutableEntityType> IMutableModel.GetEntityTypes() => GetEntities();

        /// <summary>
        /// Remove entity by Clr type
        /// </summary>
        /// <param name="clrType"></param>
        /// <returns></returns>
        public virtual CypherEntity RemoveEntity([NotNull] Type clrType) =>
            RemoveEntity(FindEntity(clrType));

        /// <summary>
        /// Remove entity by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual CypherEntity RemoveEntity([NotNull] string name) =>
            RemoveEntity(FindEntity(name));

        /// <summary>
        /// When has defining navigation remove from navigation set 
        /// otherwise from the entities set. Remove builder reference.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual CypherEntity RemoveEntity([CanBeNull] CypherEntity entity) {
            if (entity?.Builder is null) {
                return null;
            }

            entity.AssertCanRemove();

            var name = entity.Name;
            if (entity.HasDefiningNavigation()) {
                if (!_entitiesWithDefiningNavigation.TryGetValue(name, out var withSameType)) {
                    return null;
                }

                withSameType.Remove(entity);

                if (withSameType.Count == 0) {
                    _entitiesWithDefiningNavigation.Remove(name);
                }
            } else {
                _entities.Remove(name);

                if (!(entity.ClrType is null)) {
                    _clrTypeMap.Remove(entity.ClrType);
                }
            }

            entity.Builder = null;
            return entity;
        }

        /// <summary>
        /// Remove entity by another
        /// </summary>
        /// <param name="clrType"></param>
        /// <param name="definingNavigationName"></param>
        /// <param name="definingEntity"></param>
        /// <returns></returns>
        public virtual CypherEntity RemoveEntity(
            [NotNull] Type clrType,
            [NotNull] string definingNavigationName,
            [NotNull] CypherEntity definingEntity
        ) => RemoveEntity(FindEntity(clrType, definingNavigationName, definingEntity));

        /// <summary>
        /// Remove entity by another
        /// </summary>
        /// <param name="name"></param>
        /// <param name="definingNavigationName"></param>
        /// <param name="definingEntity"></param>
        /// <returns></returns>
        public virtual CypherEntity RemoveEntity(
            [NotNull] string name,
            [NotNull] string definingNavigationName,
            [NotNull] CypherEntity definingEntity
        ) => RemoveEntity(FindEntity(name, definingNavigationName, definingEntity));

        /// <summary>
        /// Remove entity type
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IMutableEntityType IMutableModel.RemoveEntityType(string name) =>
            RemoveEntity(name);
        

        /// <summary>
        /// Remove entity type
        /// </summary>
        /// <param name="name"></param>
        /// <param name="definingNavigationName"></param>
        /// <param name="definingEntityType"></param>
        /// <returns></returns>
        IMutableEntityType IMutableModel.RemoveEntityType(string name, string definingNavigationName, IMutableEntityType definingEntityType) =>
            RemoveEntity(name, definingNavigationName, (CypherEntity)definingEntityType);

        /// <summary>
        /// Ignore by type
        /// </summary>
        /// <param name="clrType"></param>
        /// <param name="configurationSource"></param>
        public virtual void Ignore(
            [NotNull] Type clrType,
            ConfigurationSource configurationSource = ConfigurationSource.Explicit)
            => Ignore(Check.NotNull(clrType, nameof(clrType)).DisplayName(), clrType, configurationSource);

        /// <summary>
        /// Ignore by name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="configurationSource"></param>
        public virtual void Ignore(
            [NotNull] string name,
            ConfigurationSource configurationSource = ConfigurationSource.Explicit)
            => Ignore(Check.NotNull(name, nameof(name)), null, configurationSource);

        /// <summary>
        /// Add to ignorables
        /// </summary>
        /// <remarks>Fire convention event</remarks>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="configurationSource"></param>
        private void Ignore(
            [NotNull] string name,
            [CanBeNull] Type clrType,
            ConfigurationSource configurationSource)
        {
            if (_ignorables.TryGetValue(name, out var existingIgnoredConfigurationSource))
            {
                configurationSource = configurationSource.Max(existingIgnoredConfigurationSource);
                _ignorables[name] = configurationSource;
                return;
            }

            _ignorables[name] = configurationSource;

            CypherConventionDispatcher.OnEntityIgnored(Builder, name, clrType);
        }

        /// <summary>
        /// Undo ignore by type
        /// </summary>
        /// <param name="clrType"></param>
        public virtual void NotIngore([NotNull] Type clrType) {
            Check.NotNull(clrType, nameof(clrType));
            NotIngore(clrType.DisplayName());
        }

        /// <summary>
        /// Undo ignore by name
        /// </summary>
        /// <param name="name"></param>
        public virtual void NotIngore([NotNull] string name) {
            Check.NotNull(name, nameof(name));
            _ignorables.Remove(name);
        }

        /// <summary>
        /// Find ignorables by type
        /// </summary>
        /// <param name="clrType"></param>
        /// <returns></returns>
        public virtual ConfigurationSource? FindIgnoredTypeConfigurationSource([NotNull] Type clrType)
        {
            Check.NotNull(clrType, nameof(clrType));

            return FindIgnoredTypeConfigurationSource(clrType.DisplayName());
        }

        /// <summary>
        /// Find ignorables by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual ConfigurationSource? FindIgnoredTypeConfigurationSource([NotNull] string name)
            => _ignorables.TryGetValue(Check.NotEmpty(name, nameof(name)), out var ignoredConfigurationSource)
                ? (ConfigurationSource?)ignoredConfigurationSource
                : null;
    }
}