// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    public class CypherForeignKeyBuilderAnnotations: CypherForeignKeyAnnotations {

        public CypherForeignKeyBuilderAnnotations(
            [NotNull] InternalRelationshipBuilder internalBuilder,
            ConfigurationSource configurationSource
        ) : base(new CypherAnnotationsBuilder(internalBuilder, configurationSource)) {
        }

        /// <summary>
        /// Annotations builder
        /// </summary>
        /// <returns></returns>
        protected new virtual CypherAnnotationsBuilder Annotations => (CypherAnnotationsBuilder)base.Annotations;

        public virtual bool HasRelationship([CanBeNull] string value) 
            => SetRelationship(
                Annotations
                    .MetadataBuilder
                    .ModelBuilder
                    .Metadata
                    .GetOrAddEntityType(value)
            );

        public virtual bool HasRelationship([CanBeNull] Type clrType)
            => SetRelationship(
                Annotations
                    .MetadataBuilder
                    .ModelBuilder
                    .Metadata
                    .GetOrAddEntityType(clrType)
            );
    }
}