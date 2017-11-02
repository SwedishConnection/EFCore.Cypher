// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal
{
    public class CypherLabelsAttributeConvention: EntityTypeAttributeConvention<LabelsAttribute> {

        /// <summary>
        /// Apply labels if not null, empy and any item is not null or white space
        /// </summary>
        /// <param name="entityTypeBuilder"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public override InternalEntityTypeBuilder Apply(
            InternalEntityTypeBuilder entityTypeBuilder, 
            LabelsAttribute attribute
        ) {
            if (!ReferenceEquals(attribute.Names, null) 
                && attribute.Names.Count() != 0
                && !attribute.Names.Any(n => string.IsNullOrWhiteSpace(n))) {
                entityTypeBuilder
                    .Cypher(ConfigurationSource.DataAnnotation)
                    .HasLabels(attribute.Names);
            }

            return entityTypeBuilder;
        }
    }
}