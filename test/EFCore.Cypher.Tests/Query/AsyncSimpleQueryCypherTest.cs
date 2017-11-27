// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.EntityFrameworkCore.TestUtilities.FakeProvider;
using Xunit;

namespace Microsoft.EntityFrameworkCore.Query
{
    /// <summary>
    /// Queryable to Cypher
    /// </summary>
    /// <remarks>No checks for Cypher to CLR</remarks>
    public class AsyncSimpleQueryCypherTest {

        public DbContextOptions DbContextOptions { get; } = CypherTestHelpers.Options("Flash=BarryAllen");


        /// <summary>
        /// No linq just the database set
        /// </summary>
        [Fact]
        public void Select_warehouse_without_linq() {
            using (var ctx = new CypherFaceDbContext(DbContextOptions)) {
                var cypher = ctx.Warehouses
                    .AsCypher();

                Assert.Equal(
                    "MATCH (w:Warehouse) RETURN \"w\".\"Location\", \"w\".\"Size\"",
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
                    "MATCH (w:Warehouse) RETURN \"w\".\"Location\", \"w\".\"Size\"",
                    cypher
                );
            }
        }

        /// <summary>
        /// Only select with flat object assignment
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

        /// <summary>
        /// Only select with nested object assignment
        /// </summary>
        [Fact]
        public void Select_warehouse_with_nested() {
            using (var ctx = new CypherFaceDbContext(DbContextOptions)) {
                var cypher = ctx.Warehouses
                    .Select(w => new { w.Location, Size = new { w.Size }})
                    .AsCypher();

                Assert.Equal(
                    "MATCH (w:Warehouse) RETURN \"w\".\"Location\", \"w\".\"Size\"",
                    cypher
                );
            }
        }

        /// <summary>
        /// Only select with empty object
        /// </summary>
        [Fact]
        public void Select_warehouse_with_empty() {
            using (var ctx = new CypherFaceDbContext(DbContextOptions)) {
                var cypher = ctx.Warehouses
                    .Select(w => new { })
                    .AsCypher();

                Assert.Equal(
                    "MATCH (w:Warehouse) RETURN 1",
                    cypher
                );
            }
        }

        /// <summary>
        /// Only select with object having a single literal
        /// </summary>
        [Fact]
        public void Select_warehouse_with_literal() {
            using (var ctx = new CypherFaceDbContext(DbContextOptions)) {
                var cypher = ctx.Warehouses
                    .Select(w => new { Size = 1 })
                    .AsCypher();

                Assert.Equal(
                    "MATCH (w:Warehouse) RETURN 1",
                    cypher
                );
            }
        }

        /// <summary>
        /// Only select with object having a conditional
        /// </summary>
        [Fact]
        public void Select_warehouse_with_conditional() {
            using (var ctx = new CypherFaceDbContext(DbContextOptions)) {
                var cypher = ctx.Warehouses
                    .Select(w => new { IsBig = w.Size > 100 })
                    .AsCypher();

                Assert.Equal(
                    "MATCH (w:Warehouse) RETURN \"w\".\"Size\" > 100 AS \"IsBig\"",
                    cypher
                );
            }
        }

        /// <summary>
        /// Only select with object having a conditional case
        /// </summary>
        [Fact]
        public void Select_warehouse_with_conditional_case() {
            using (var ctx = new CypherFaceDbContext(DbContextOptions)) {
                var cypher = ctx.Warehouses
                    .Select(w => new { IsBig = w.Size > 100 ? "Giant" : "Ant" })
                    .AsCypher();

                Assert.Equal(
                    "MATCH (w:Warehouse) RETURN CASE\r\n    WHEN \"w\".\"Size\" > 100\r\n    THEN 'Giant' ELSE 'Ant'\r\nEND AS \"IsBig\"",
                    cypher
                );
            }
        }

        /// <summary>
        /// Where one property equals constant with default return
        /// </summary>
        [Fact]
        public void Where_equal() {
            using (var ctx = new CypherFaceDbContext(DbContextOptions)) {
                var cypher = ctx.Warehouses
                    .Where(w => w.Size == 100)
                    .AsCypher();

                Assert.Equal(
                    "MATCH (w:Warehouse)\r\nWHERE \"w\".\"Size\" = 100 RETURN \"w\".\"Location\", \"w\".\"Size\"",
                    cypher
                );
            }
        }
    }
}