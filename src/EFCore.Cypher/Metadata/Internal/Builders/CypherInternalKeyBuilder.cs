// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    /// <summary>
    /// Internal graph builder
    /// </summary>
    public class CypherInternalKeyBuilder: CypherInternalMetadataItemBuilder<CypherKey> {

        public CypherInternalKeyBuilder(
            [NotNull] CypherKey key,
            [NotNull] CypherInternalGraphBuilder graphBuilder
        ) : base(key, graphBuilder) {

        }
    }
}