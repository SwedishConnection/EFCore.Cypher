using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    public class Graph : ConventionalAnnotatable, IMutableGraph
    {
        private readonly Dictionary<NodeIdentity, Entity> _entities =
            new Dictionary<NodeIdentity, Entity>(new NodeIdentityComparer());

        private readonly IDictionary<NodeIdentity, HashSet<Entity>> _entitesWithDefiningNavigation =
            new Dictionary<NodeIdentity, HashSet<Entity>>(new NodeIdentityComparer());

        private readonly Dictionary<NodeIdentity, Relationship> _relationships =
            new Dictionary<NodeIdentity, Relationship>(new NodeIdentityComparer());

        private readonly Dictionary<NodeIdentity, ConfigurationSource> _ignored = 
            new Dictionary<NodeIdentity, ConfigurationSource>(new NodeIdentityComparer());

        private readonly IDictionary<Type, INode> _clrTypeMap = 
            new Dictionary<Type, INode>();

        public Graph(): this(new GraphConventionSet()) {

        }

        public Graph(GraphConventionSet conventions) {
            var dispatcher = new GraphConventionDispatcher(conventions);
            var builder = new InternalGraphBuilder(this);

            GraphConventionDispatcher = dispatcher;
            Builder = builder;
        }

        /// <summary>
        /// Dispatcher (conventions)
        /// </summary>
        /// <returns></returns>
        public virtual GraphConventionDispatcher GraphConventionDispatcher { get; }

        /// <summary>
        /// Internal builder
        /// </summary>
        /// <returns></returns>
        public virtual InternalGraphBuilder Builder { get; }

        /// <summary>
        /// Add entity by labels
        /// </summary>
        /// <param name="labels"></param>
        /// <param name="configurationSource"></param>
        /// <returns></returns>
        public virtual Entity AddEntity(
            [NotNull] string[] labels,
            ConfigurationSource configurationSource = ConfigurationSource.Explicit
        ) {
            Check.NotEmpty(labels, nameof(labels));

            var entity = new Entity(labels, this, configurationSource);
            return AddEntity(entity);
        }

        /// <summary>
        /// Add entity be type
        /// </summary>
        /// <param name="clrType"></param>
        /// <param name="configurationSource"></param>
        /// <returns></returns>
        public virtual Entity AddEntity(
            [NotNull] Type clrType,
            ConfigurationSource configurationSource = ConfigurationSource.Explicit
        ) {
            Check.NotNull(clrType, nameof(clrType));

             var entity = new Entity(clrType, this, configurationSource);

             _clrTypeMap[clrType] = entity;
             return AddEntity(entity);
        }

        /// <summary>
        /// Add entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private Entity AddEntity(Entity entity) {
            NodeIdentity identity = new NodeIdentity(entity.Labels);

            if (entity.HasDefiningNavigation()) {
                if (_entities.ContainsKey(identity)) {
                    throw new InvalidOperationException(CoreCypherStrings.ClashingNonDependentEntity(entity.DisplayLabels()));
                }

                if (!_entitesWithDefiningNavigation.TryGetValue(identity, out var sameEntities)) {
                    sameEntities = new HashSet<Entity>();
                    _entitesWithDefiningNavigation[identity] = sameEntities;
                }

                sameEntities.Add(entity);
            } else {
                if (_entitesWithDefiningNavigation.ContainsKey(identity)) {
                    throw new InvalidOperationException(CoreCypherStrings.ClashingDependentEntity(entity.DisplayLabels()));
                }

                var howManyEntities = _entities.Count;
                _entities[identity] = entity;
                if (howManyEntities == _entities.Count) {
                    throw new InvalidOperationException(CoreCypherStrings.DuplicateEntity(entity.DisplayLabels()));
                }
            }

            return GraphConventionDispatcher.OnEntityAdded(entity.Builder)?.Metadata;
        }

        public Entity AddEntity(
            [NotNull] string[] labels,
            [NotNull] string definingNavigationName,
            [NotNull] IMutableEntity definingType,
            ConfigurationSource configurationSource = ConfigurationSource.Explicit
        ) {
            throw new NotImplementedException();
        }

        public Entity AddEntity(
            [NotNull] Type clrType,
            [NotNull] string definingNavigationName,
            [NotNull] IMutableEntity definingType,
            ConfigurationSource configurationSource = ConfigurationSource.Explicit
        ) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="labels"></param>
        /// <returns></returns>
        public IMutableEntity AddEntity(string[] labels) => AddEntity(labels);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clrType"></param>
        /// <returns></returns>
        public IMutableEntity AddEntity(Type clrType) => AddEntity(clrType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="labels"></param>
        /// <param name="definingNavigationName"></param>
        /// <param name="definingType"></param>
        /// <returns></returns>
        public IMutableEntity AddEntity(string[] labels, string definingNavigationName, IMutableEntity definingType) => 
            AddEntity(labels, definingNavigationName, definingType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clrType"></param>
        /// <param name="definingNavigationName"></param>
        /// <param name="definingType"></param>
        /// <returns></returns>
        public IMutableEntity AddEntity(Type clrType, string definingNavigationName, IMutableEntity definingType) => 
            AddEntity(clrType, definingNavigationName, definingType);

        public IMutableRelationship AddRelationship([NotNull] string[] labels)
        {
            throw new NotImplementedException();
        }

        public IMutableRelationship AddRelationship([NotNull] Type clrType)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="labels"></param>
        /// <returns></returns>
        public virtual Entity FindEntity([NotNull] string[] labels) 
            => _entities.TryGetValue(new NodeIdentity(labels), out var entity)
                ? entity 
                : null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clrType"></param>
        /// <returns></returns>
        public virtual Entity FindEntity([NotNull] Type clrType)
            => _clrTypeMap.TryGetValue(clrType, out var entityType) && entityType is IMutableEntity
                ? entityType as Entity
                : FindEntity(clrType.DisplayLabels());

        /// <summary>
        /// 
        /// </summary>
        /// <param name="labels"></param>
        /// <returns></returns>
        IEntity IGraph.FindEntity(string[] labels) => FindEntity(labels);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="labels"></param>
        /// <returns></returns>
        IMutableEntity IMutableGraph.FindEntity(string[] labels) => FindEntity(labels);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="labels"></param>
        /// <returns></returns>
        public IMutableRelationship FindRelationship([NotNull] string[] labels)
            => _relationships.TryGetValue(new NodeIdentity(labels), out var rel)
                ? rel 
                : null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clrType"></param>
        /// <returns></returns>
        public IMutableRelationship FindRelationship([NotNull] Type clrType)
            => _clrTypeMap.TryGetValue(clrType, out var entityType) && entityType is IMutableRelationship
                ? entityType as IMutableRelationship
                : FindRelationship(clrType.DisplayLabels());

        /// <summary>
        /// 
        /// </summary>
        /// <param name="labels"></param>
        /// <returns></returns>
        IRelationship IGraph.FindRelationship(string[] labels) => FindRelationship(labels);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="labels"></param>
        /// <returns></returns>
        IMutableRelationship IMutableGraph.FindRelationship(string[] labels) => FindRelationship(labels);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Entity> GetEntities()
            => _entities.Where(n => n.Value is IMutableEntity).Select(n => n.Value as Entity);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<IEntity> IGraph.GetEntities() => GetEntities();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<IMutableEntity> IMutableGraph.GetEntities() => GetEntities();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IMutableRelationship> GetRelationships()
            => _relationships.Where(n => n.Value is IMutableRelationship).Select(n => n.Value as Relationship);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<IRelationship> IGraph.GetRelationships() => GetRelationships();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<IMutableRelationship> IMutableGraph.GetRelationships() => GetRelationships();

        public IMutableNode RemoveEntity([NotNull] string[] labels)
        {
            throw new NotImplementedException();
        }

        public IMutableNode RemoveRelationship([NotNull] string[] labels)
        {
            throw new NotImplementedException();
        }

        public virtual ConfigurationSource? FindIgnoredTypeConfigurationSource([NotNull] Type clrType) =>
            FindIgnoredTypeConfigurationSource(new NodeIdentity(Check.NotNull(clrType, nameof(clrType))));

        public virtual ConfigurationSource? FindIgnoredTypeConfigurationSource([NotNull] string[] labels) => 
            FindIgnoredTypeConfigurationSource(Check.NotNull(labels, nameof(labels)));

        private ConfigurationSource? FindIgnoredTypeConfigurationSource(NodeIdentity identity) =>
            _ignored.TryGetValue(identity, out var ignoredConfigurationSource)
                ? (ConfigurationSource?)ignoredConfigurationSource
                : null;
        
        public virtual void Ignore(
            [NotNull] Type clrType,
            ConfigurationSource configurationSource = ConfigurationSource.Explicit
        ) => Ignore(new NodeIdentity(Check.NotNull(clrType, nameof(clrType))), configurationSource);

        public virtual void Ignore(
            [NotNull] string[] labels,
            ConfigurationSource configurationSource = ConfigurationSource.Explicit
        ) => Ignore(new NodeIdentity(Check.NotNull(labels, nameof(labels))), configurationSource);

        private void Ignore(
            NodeIdentity identity,
            ConfigurationSource configurationSource
        ) {
            if (_ignored.TryGetValue(identity, out var existing)) {
                configurationSource = configurationSource.Max(existing);
                _ignored[identity] = configurationSource;
                return;
            }

            _ignored[identity] = configurationSource;
            // TODO: dispatcher
        }

        public virtual void NotIgnore([NotNull] Type clrType) => 
            NotIgnore(new NodeIdentity(Check.NotNull(clrType, nameof(clrType))));

        public virtual void NotIgnore([NotNull] string[] labels) => 
            NotIgnore(new NodeIdentity(Check.NotNull(labels, nameof(labels))));

        private void NotIgnore(NodeIdentity identity) => _ignored.Remove(identity);
    }

    public class NodeIdentityComparer : IEqualityComparer<NodeIdentity>
    {
        public bool Equals(NodeIdentity x, NodeIdentity y)
        {
            string[] xLabels = x.Labels;
            string[] yLabels = y.Labels;

            if (xLabels.Length != yLabels.Length) {
                return false;
            }

            var xs = xLabels.OrderByDescending(i => i, StringComparer.CurrentCulture).ToArray();
            var ys = yLabels.OrderByDescending(i => i, StringComparer.CurrentCulture).ToArray();

            for (var i=0; i < xs.Length; i++) {
                if (xs[i] != ys[i]) {
                    return false;
                }
            }

            return true;
        }

        public int GetHashCode(NodeIdentity obj)
        {
            string[] labels = obj.Labels;
            int result = 17;

            for (int i=0; i < labels.Length; i++) {
                unchecked {
                    result = result * 23 + labels[i].GetHashCode();
                }
            }

            return result;
        }
    }
}