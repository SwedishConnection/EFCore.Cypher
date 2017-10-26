// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;

namespace Microsoft.EntityFrameworkCore.Metadata.Conventions
{
    /// <summary>
    /// Conventions for a graph
    /// </summary>
    public class GraphConventionSet {
        
        public virtual IList<IEntityAddedConvention> EntityAddedConventions { get; } = new List<IEntityAddedConvention>();
        
        public virtual IList<IGraphInitializedConvention> GraphInitializedConventions { get; } = new List<IGraphInitializedConvention>();

    }
}