// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    public class CypherPropertyBuilderAnnotations: CypherPropertyAnnotations {

        public CypherPropertyBuilderAnnotations(
            [NotNull] InternalPropertyBuilder propertyBuilder,
            ConfigurationSource configurationSource
        ) : base(new CypherAnnotationsBuilder(propertyBuilder, configurationSource)) {

        }

        public virtual bool HasStorageName([CanBeNull] string value) => SetStorageName(value);
    }
}