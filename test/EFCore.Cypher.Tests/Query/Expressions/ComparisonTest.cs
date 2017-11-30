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
        public void RelationshipDetail() {
            var me = new RelationshipDetailExpression(
                new string[] { "OWNS" },
                null,
                "r"
            );

            var other = new RelationshipDetailExpression(
                new string[] { "OWNS" },
                null,
                "r"
            );

            Assert.True(comparer.Equals(me, other));

            var bad = new RelationshipDetailExpression(
                null,
                null,
                null
            );

            Assert.False(comparer.Equals(me, bad));
        }

        [Fact]
        public void RelationshipPattern() {
            var me = new RelationshipPatternExpression(
                RelationshipDirection.Left,
                new RelationshipDetailExpression(
                    new string[] { "OWNS" },
                    null,
                    "r"
                )
            );

            var other = new RelationshipPatternExpression(
                RelationshipDirection.Left,
                new RelationshipDetailExpression(
                    new string[] { "OWNS" },
                    null,
                    "r"
                )
            );

            Assert.True(comparer.Equals(me, other));
        }

        [Fact]
        public void NodePattern() {
            var me = new NodePatternExpression(
                new string[] { "Warehouse" },
                null,
                "w"
            );

            var other = new NodePatternExpression(
                new string[] { "Warehouse" },
                null,
                "w"
            );

            Assert.True(comparer.Equals(me, other));
        }

        [Fact]
        public void Pattern() {
            var me = new PatternExpression(
                new NodePatternExpression(
                    new string[] { "Warehouse" },
                    null,
                    "w"
                )
            );

            var other = new PatternExpression(
                new NodePatternExpression(
                    new string[] { "Warehouse" },
                    null,
                    "w"
                )
            );

            Assert.True(comparer.Equals(me, other));
        }

        [Fact]
        public void Match() {
            var me = new MatchExpression(
                new PatternExpression(
                    new NodePatternExpression(
                        new string[] { "Warehouse" },
                        null,
                        "w"
                    )
                )
            );

            var other = new MatchExpression(
                new PatternExpression(
                    new NodePatternExpression(
                        new string[] { "Warehouse" },
                        null,
                        "w"
                    )
                )
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
                    new NodePatternExpression(
                        new string[] { "Warehouse" },
                        null,
                        "w"
                    )
                );

                var other = new StorageExpression(
                    "Place",
                    Location,
                    new NodePatternExpression(
                        new string[] { "Warehouse" },
                        null,
                        "w"
                    )
                );

                Assert.True(comparer.Equals(me, other));
            }
        }
    }
}