// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    public class CypherPropertyBuildersSnapshot {

        private IReadOnlyList<Tuple<CypherInternalPropertyBuilder, ConfigurationSource>> Properties { get; }

        public CypherPropertyBuildersSnapshot(
            IReadOnlyList<Tuple<CypherInternalPropertyBuilder, ConfigurationSource>> properties
        ) {
            Properties = properties;
        }
        
        public void Attach(CypherInternalEntityBuilder builder) {
            foreach (var pair in Properties) {
                pair.Item1.Attach(builder, pair.Item2);
            }
        }
    }
}