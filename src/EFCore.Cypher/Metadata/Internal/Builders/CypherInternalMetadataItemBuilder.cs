// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    public abstract class CypherInternalMetadataItemBuilder<TMetadata>: CypherInternalMetadataBuilder<TMetadata>
        where TMetadata: ConventionalAnnotatable {
            protected CypherInternalMetadataItemBuilder(
                [NotNull] TMetadata metadata, 
                [NotNull] CypherInternalGraphBuilder graphBuilder
            ): base(metadata) {
                GraphBuilder = graphBuilder;
            }

            /// <summary>
            /// Graph builder
            /// </summary>
            /// <returns></returns>
            public override CypherInternalGraphBuilder GraphBuilder { get; }
        }
}