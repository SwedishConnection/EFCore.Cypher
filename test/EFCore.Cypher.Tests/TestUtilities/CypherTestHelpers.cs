// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.


using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.TestUtilities.FakeProvider;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.EntityFrameworkCore.TestUtilities
{
    public class CypherTestHelpers: TestHelpers {
        protected CypherTestHelpers()
        {
        }

        /// <summary>
        /// Singleton
        /// </summary>
        /// <returns></returns>
        public static CypherTestHelpers Instance { get; } = new CypherTestHelpers();

        /// <summary>
        /// Add provider services
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public override IServiceCollection AddProviderServices(IServiceCollection services)
            => FakeCypherOptionsExtension.AddEntityFrameworkCypherDatabase(services);

        /// <summary>
        /// Provider options (e.g. database connection)
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void UseProviderOptions(DbContextOptionsBuilder optionsBuilder)
        {
            var extension = optionsBuilder
                .Options
                .FindExtension<FakeCypherOptionsExtension>()
                ?? new FakeCypherOptionsExtension();

            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder)
                .AddOrUpdateExtension(
                    extension.WithConnection(new FakeCypherDbConnection("Database=Fake")
                )
            );
        }
    }
}