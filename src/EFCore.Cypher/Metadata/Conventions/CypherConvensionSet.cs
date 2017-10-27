// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;

namespace Microsoft.EntityFrameworkCore.Metadata.Conventions
{
    /// <summary>
    /// Conventions for a graph
    /// </summary>
    public class CypherConventionSet {
        
        public virtual IList<ICypherGraphInitializedConvention> GraphInitializedConventions { get; } = new List<ICypherGraphInitializedConvention>();

        public virtual IList<ICypherEntityAddedConvention> EntityAddedConventions { get; } = new List<ICypherEntityAddedConvention>();

        public virtual IList<ICypherBaseEntityChangedConvention> BaseEntityChangedConventions { get; } = new List<ICypherBaseEntityChangedConvention>();

    }
}