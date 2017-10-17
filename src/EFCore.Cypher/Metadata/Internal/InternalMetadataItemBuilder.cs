using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    public abstract class InternalMetadataItemBuilder<TMetadata>: InternalMetadataBuilder<TMetadata>
        where TMetadata: ConventionalAnnotatable {
            protected InternalMetadataItemBuilder(
                [NotNull] TMetadata metadata, 
                [NotNull] InternalGraphBuilder graphBuilder
            ): base(metadata) {
                GraphBuilder = graphBuilder;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public override InternalGraphBuilder GraphBuilder { get; }
        }
}