// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
            /// Graph builder
            /// </summary>
            /// <returns></returns>
            public override InternalGraphBuilder GraphBuilder { get; }
        }
}