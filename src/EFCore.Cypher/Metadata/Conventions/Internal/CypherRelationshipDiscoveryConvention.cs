// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal
{
    public class CypherRelationshipDiscoveryConvention:
        IForeignKeyAddedConvention 
    {
        public virtual InternalRelationshipBuilder Apply(InternalRelationshipBuilder relationshipBuilder)
        {
            var fk = relationshipBuilder.Metadata;

            if (ConfigurationSource.Convention.Overrides(fk.GetForeignKeyPropertiesConfigurationSource()))
            {
            }

            return relationshipBuilder;
        }
    }
}
