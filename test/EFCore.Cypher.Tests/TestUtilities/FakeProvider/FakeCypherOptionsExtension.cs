// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Query.Cypher;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;
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
                .TryAdd<IDatabaseProvider, DatabaseProvider<FakeCypherOptionsExtension>>()
                .TryAdd<ISqlGenerationHelper, RelationalSqlGenerationHelper>()
                .TryAdd<IRelationalTypeMapper, TestCypherTypeMapper>()
                // Migration SQL generator
                .TryAdd<IConventionSetBuilder, TestCypherConventionSetBuilder>()
                .TryAdd<IMemberTranslator, TestCypherCompositeMemberTranslator>()
                .TryAdd<ICompositeMethodCallTranslator, TestCypherCompositeMethodCallTranslator>()
                .TryAdd<IQueryCypherGeneratorFactory, TestQueryCypherGeneratorFactory>()
                .TryAdd<IRelationalConnection, FakeCypherConnection>()
                // History 
                // Update SQL Generator
                .TryAdd<IModificationCommandBatchFactory, TestCypherModificationCommandBatchFactory>()
                .TryAdd<IRelationalDatabaseCreator, FakeCypherDatabaseCreator>();

            builder.TryAddCoreServices();

            return services;
        }

        protected override RelationalOptionsExtension Clone()
            => new FakeCypherOptionsExtension(this);
    }
}