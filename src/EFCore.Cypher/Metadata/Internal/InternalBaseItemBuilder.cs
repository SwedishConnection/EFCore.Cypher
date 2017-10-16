using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    public abstract class InternalBaseItemBuilder<TBase>: InternalBaseBuilder<TBase>
        where TBase: ConventionalAnnotatable {
            protected InternalBaseItemBuilder(
                [NotNull] TBase baze, 
                [NotNull] InternalGraphBuilder graphBuilder
            ): base(baze) {
                GraphBuilder = graphBuilder;
            }

            public override InternalGraphBuilder GraphBuilder { get; }
        }
}