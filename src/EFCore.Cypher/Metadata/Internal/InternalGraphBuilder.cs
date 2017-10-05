using JetBrains.Annotations;
using System;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    public class InternalGraphBuilder: InternalBaseBuilder<Graph> {
        
        public InternalGraphBuilder([NotNull] Graph baze): base(baze) {

        }

        public override InternalGraphBuilder GraphBuilder => this;

        public virtual InternalEntityBuilder Entity([NotNull] string[] labels, ConfigurationSource configurationSource) 
            => Entity(new NodeIdentity(labels), configurationSource);

        public virtual InternalEntityBuilder Entity([NotNull] Type clrType, ConfigurationSource configurationSource) 
            => Entity(new NodeIdentity(clrType), configurationSource);

        private InternalEntityBuilder Entity(NodeIdentity identity, ConfigurationSource configurationSource) {
            var clrType = identity.Type;
            var entityType = clrType == null
                ? Base.FindEntity(identity.Labels)
                : Base.FindEntity(clrType);

            return null;
        }
    }
}