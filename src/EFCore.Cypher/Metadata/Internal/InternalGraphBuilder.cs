using JetBrains.Annotations;
using System;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    public class InternalGraphBuilder: InternalBaseBuilder<Graph> {
        
        public InternalGraphBuilder([NotNull] Graph baze): base(baze) {

        }

        public override InternalGraphBuilder GraphBuilder => this;

        public virtual InternalEntityBuilder Entity([NotNull] string[] labels) => throw new NotImplementedException();

    }
}