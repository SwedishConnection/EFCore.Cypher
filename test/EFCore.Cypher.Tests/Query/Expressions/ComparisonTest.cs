// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Xunit;

namespace Microsoft.EntityFrameworkCore.Query.Expressions
{
    public class ComparisonTest {
        public DbContextOptions DbContextOptions { get; } = CypherTestHelpers.Options("Flash=BarryAllen");

        ExpressionEqualityComparer comparer = new ExpressionEqualityComparer();

        [Fact]
        public void Match() {
            var me = new MatchExpression(
                new string[] { "Warehouse "}, 
                "w", 
                null // TODO: add|get query source
            );

            var other = new MatchExpression(
                new string[] { "Warehouse "}, 
                "w", 
                null // TODO: add|get query source
            );

            Assert.True(comparer.Equals(me, other));
        }

        [Fact]
        public void Storage() {
            using (var ctx = new CypherFaceDbContext(DbContextOptions)) {
                var Location = ctx.Model
                    .FindEntityType(typeof(Warehouse))
                    .FindProperty("Location");

                var me = new StorageExpression(
                    "Place",
                    Location,
                    new MatchExpression(
                        new string[] { "Warehouse" }, 
                        "w", 
                        null
                    )  
                );

                var other = new StorageExpression(
                    "Place",
                    Location,
                    new MatchExpression(
                        new string[] { "Warehouse" }, 
                        "w", 
                        null
                    )  
                );

                Assert.True(comparer.Equals(me, other));
            }
        }
    }
}