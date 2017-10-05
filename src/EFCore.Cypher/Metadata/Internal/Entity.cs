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
        }

        public Entity(
            [NotNull] Type clrType, 
            [NotNull] Graph graph, 
            ConfigurationSource configurationSource
        ) : base(clrType, graph, configurationSource)
        {
        }

        public IEnumerable<IMutableRelationship> GetRelationships()
        {
            throw new System.NotImplementedException();
        }

        IEnumerable<IRelationship> IEntity.GetRelationships()
        {
            throw new System.NotImplementedException();
        }
    }
}