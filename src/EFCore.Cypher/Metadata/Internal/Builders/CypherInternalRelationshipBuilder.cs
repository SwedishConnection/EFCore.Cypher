// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    public class CypherInternalRelationshipBuilder: CypherInternalMetadataItemBuilder<CypherForeignKey> {
        
        public CypherInternalRelationshipBuilder(
            [NotNull] CypherForeignKey foreignKey,
            [NotNull] CypherInternalGraphBuilder graphBuilder
        ) : base (foreignKey, graphBuilder) {
            
        }
    }
}