// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.EntityFrameworkCore.TestUtilities.FakeProvider
{
    public class FakeCypherOptionsExtension: RelationalOptionsExtension {
        public FakeCypherOptionsExtension()
        {
        }

        protected FakeCypherOptionsExtension(
            FakeCypherOptionsExtension copyFrom
        ) : base(copyFrom)
        {
        }

        public override bool ApplyServices(IServiceCollection services)
        {
            AddEntityFrameworkCypherDatabase(services);

            return true;
        }

        public static IServiceCollection AddEntityFrameworkCypherDatabase(IServiceCollection services) {
            var builder = new EntityFrameworkCypherServicesBuilder(services)
                .TryAdd<IDatabaseProvider, DatabaseProvider<FakeCypherOptionsExtension>>();

            builder.TryAddCoreServices();

            return services;
        }

        protected override RelationalOptionsExtension Clone()
            => new FakeCypherOptionsExtension(this);
    }
}