// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Data.Common;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Microsoft.EntityFrameworkCore.TestUtilities
{
    public class CypherFaceDbContext: DbContext {

        public CypherFaceDbContext(
            DbContextOptions options
        ): base(options) {
        }

        public DbSet<Warehouse> Warehouses { get; set; }

        public DbSet<Thing> Things { get; set; }
    }

    [Labels(new string[] {"Warehouse"})]
    public class Warehouse {
        public string Name { get; set; }

        [Relationship("OWNS")]
        public IEnumerable<Thing> Things { get; set; }
    }

    public class Thing {
        public int Number { get; set; }
    }
}