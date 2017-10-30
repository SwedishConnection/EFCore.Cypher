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

        public virtual IList<ICypherPropertyAddedConvention> PropertyAddedConventions { get; } = new List<ICypherPropertyAddedConvention>();

        public virtual IList<ICypherEntityIgnoredConvention> EntityIgnoredConventions { get; } = new List<ICypherEntityIgnoredConvention>();

        public virtual IList<ICypherForeignKeyUniqueChangedConvention> ForeignKeyUniqueChangedConventions { get; } = new List<ICypherForeignKeyUniqueChangedConvention>();

        public virtual IList<ICypherPropertyNullabilityChangedConvention> PropertyNullabilityChangedConventions { get; } = new List<ICypherPropertyNullabilityChangedConvention>();

        public virtual IList<ICypherForeignKeyOwnershipChangedConvention> ForeignKeyOwnershipChangedConventions { get; } = new List<ICypherForeignKeyOwnershipChangedConvention>();

        public virtual IList<ICypherNavigationRemovedConvention> NavigationRemovedConventions { get; } = new List<ICypherNavigationRemovedConvention>();

        public virtual IList<ICypherNavigationAddedConvention> NavigationAddedConventions { get; } = new List<ICypherNavigationAddedConvention>();
     }
}