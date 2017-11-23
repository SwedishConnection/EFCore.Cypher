// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.EntityFrameworkCore.TestUtilities.FakeProvider;
using Xunit;

namespace Microsoft.EntityFrameworkCore.Query
{
    public class AsyncSimpleQueryCypherTest {

        public AsyncSimpleQueryCypherTest() {

        }

        public DbContextOptions DbContextOptions { get; } = Options("Flash=BarryAllen");

        /// <summary>
        /// Database context options
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static DbContextOptions Options(string connectionString) {
            var optionsBuilder = new DbContextOptionsBuilder();
            var options = new FakeCypherOptionsExtension()
                .WithConnectionString(connectionString);

            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder)
                .AddOrUpdateExtension(options);

            return optionsBuilder.Options;
        }

        /// <summary>
        /// No linq just the database set
        /// </summary>
        [Fact]
        public void Select_warehouse_without_linq() {
            using (var ctx = new CypherFaceDbContext(DbContextOptions)) {
                var cypher = ctx.Warehouses.AsCypher();

                Assert.Equal(
                    "MATCH (w:Warehouse) RETURN \"w\".\"Location\"",
                    cypher
                );
            }
        }

        /// <summary>
        /// Only select by identity
        /// </summary>
        [Fact]
        public void Select_warehouse_identity() {
            using (var ctx = new CypherFaceDbContext(DbContextOptions)) {
                var cypher = ctx.Warehouses
                    .Select(w => w)
                    .AsCypher();

                Assert.Equal(
                    "MATCH (w:Warehouse) RETURN \"w\".\"Location\"",
                    cypher
                );
            }
        }

        /// <summary>
        /// Only select with object assignment
        /// </summary>
        [Fact]
        public void Select_warehouse_with_object() {
            using (var ctx = new CypherFaceDbContext(DbContextOptions)) {
                var cypher = ctx.Warehouses
                    .Select(w => new { Place = w.Location, Status = 1})
                    .AsCypher();

                Assert.Equal(
                    "MATCH (w:Warehouse) RETURN \"w\".\"Location\" AS \"Place\"",
                    cypher
                );
            }
        }
    }
}