using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    public class Entity : Node, IMutableEntity
    {
        public Entity(
            [NotNull] string[] labels, 
            [NotNull] Graph graph, 
            ConfigurationSource configurationSource
        ) : base(labels, graph, configurationSource)
        {
            Builder = new InternalEntityBuilder(this, graph.Builder);
        }

        public Entity(
            [NotNull] Type clrType, 
            [NotNull] Graph graph, 
            ConfigurationSource configurationSource
        ) : base(clrType, graph, configurationSource)
        {
        }

        public new InternalEntityBuilder Builder { get; }

        public string DefiningNavigationName => throw new NotImplementedException();

        public IEntity DefiningType => throw new NotImplementedException();

        IMutableEntity IMutableEntity.BaseType { 
            get => (IMutableEntity)_baseType; 
            set => HasBaseType((Entity)value);
        }

        public new Entity BaseType => (Entity)_baseType;

        IEntity IEntity.BaseType => (IEntity)_baseType;

        public virtual void HasBaseType(
            [CanBeNull] Entity entity, 
            ConfigurationSource configurationSource = ConfigurationSource.Explicit
        ) {

        }

        public IMutableConstraint AddKeysConstraint([NotNull] IReadOnlyList<IMutableNodeProperty> properties)
        {
            throw new NotImplementedException();
        }

        public IMutableConstraint AddUniqueConstraint([NotNull] IMutableNodeProperty property)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IMutableRelationship> GetRelationships()
        {
            throw new System.NotImplementedException();
        }

        public IMutableConstraint RemoveKeysConstraint([NotNull] IReadOnlyList<INodeProperty> properties)
        {
            throw new NotImplementedException();
        }

        IEnumerable<IRelationship> IEntity.GetRelationships()
        {
            throw new System.NotImplementedException();
        }
    }
}