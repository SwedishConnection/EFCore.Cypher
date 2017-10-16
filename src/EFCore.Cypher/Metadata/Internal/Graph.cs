using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    public class Graph : ConventionalAnnotatable, IMutableGraph
    {
        private readonly Dictionary<string[], Entity> _entities =
            new Dictionary<string[], Entity>(new LabelsComparer());

        private readonly SortedDictionary<string[], Relationship> _relationships =
            new SortedDictionary<string[], Relationship>();

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
            if (_entities.ContainsKey(entity.Labels)) {
                // TODO: String pattern
                throw new InvalidOperationException("");
            }

            return entity;
        }

        public IMutableEntity AddEntity([NotNull] string[] labels)
        {
            throw new NotImplementedException();
        }

        public IMutableRelationship AddRelationship([NotNull] string[] labels)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="labels"></param>
        /// <returns></returns>
        public virtual Entity FindEntity([NotNull] string[] labels) 
            => _entities.TryGetValue(labels, out var entity)
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
            => _relationships.TryGetValue(labels, out var rel)
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
    }

    public class LabelsComparer : IEqualityComparer<string[]>
    {
        public bool Equals(string[] x, string[] y)
        {
            if (x.Length != y.Length) {
                return false;
            }

            var xs = x.OrderByDescending(i => i, StringComparer.CurrentCulture).ToArray();
            var ys = y.OrderByDescending(i => i, StringComparer.CurrentCulture).ToArray();

            for (var i=0; i < xs.Length; i++) {
                if (xs[i] != ys[i]) {
                    return false;
                }
            }

            return true;
        }

        public int GetHashCode(string[] obj)
        {
            int result = 17;
            for (int i=0; i < obj.Length; i++) {
                unchecked {
                    result = result * 23 + Convert.ToInt32(obj[i]);
                }
            }

            return result;
        }
    }
}