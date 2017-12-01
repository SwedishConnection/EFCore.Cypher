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

        public DbSet<Person> Persons { get; set; }

        public DbSet<Supervise> Supervising { get; set; }

        public DbSet<Owns> Owning { get; set; }
    }

    [Labels(new string[] {"Warehouse"})]
    public class Warehouse {
        public string Location { get; set; }

        public int Size { get; set; }

        [Relationship(typeof(Owns))]
        public virtual IEnumerable<Thing> Things { get; set; }

        [Relationship(typeof(Supervise))]
        public virtual Person PersonInCharge { get; set; }
    }

    public class Thing {
        public int Number { get; set; }
    }

    public class Person {
        public string Name { get; set; }
    }

    public class Supervise {
        public bool Certified { get; set; }
    }

    [Labels(new string[] {"OWNS"})]
    public class Owns {
        public bool Partial { get; set; }
    }
}