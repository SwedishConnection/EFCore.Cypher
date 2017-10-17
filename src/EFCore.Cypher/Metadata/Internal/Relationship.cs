using System;
using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    public class Relationship : Node, IMutableRelationship
    {
        
        public Relationship(
            [NotNull] string[] labels, 
            [NotNull] Graph graph, 
            ConfigurationSource configurationSource
        ) : base(labels, graph, configurationSource)
        {
            Builder = new InternalRelationshipBuilder(this, graph.Builder);
        }

        public Relationship(
            [NotNull] Type clrType, 
            [NotNull] Graph graph, 
            ConfigurationSource configurationSource
        ) : base(clrType, graph, configurationSource)
        {
        }

        public new InternalRelationshipBuilder Builder { get; }

        public IMutableEntity Start { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public IMutableEntity End { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        IMutableRelationship IMutableRelationship.BaseType { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        IRelationship IRelationship.BaseType => throw new System.NotImplementedException();

        IEntity IRelationship.Start => throw new System.NotImplementedException();

        IEntity IRelationship.End => throw new System.NotImplementedException();
    }
}