// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.EntityFrameworkCore.TestUtilities.FakeProvider;
using Xunit;

namespace Microsoft.EntityFrameworkCore.Query
{
    public class AsyncSimpleQueryCypherTest {

        public AsyncSimpleQueryCypherTest() {

        }

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

        [Fact]
        public void Select_warehouse() {
            DbContextOptions options = Options("Flash=BarryAllen");

            using (var ctx = new CypherFaceDbContext(options)) {
                var warehouses = ctx.Warehouses
                    .ToArrayAsync()
                    .Result;

                Assert.Empty(warehouses);
            }
        }
    }
}