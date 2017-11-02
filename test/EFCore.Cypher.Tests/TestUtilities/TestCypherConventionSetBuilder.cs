// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.EntityFrameworkCore.Storage;

namespace Microsoft.EntityFrameworkCore.TestUtilities
{
    public class TestCypherConventionSetBuilder: CypherConventionSetBuilder {
        public TestCypherConventionSetBuilder(
            RelationalConventionSetBuilderDependencies dependencies
        ) : base(dependencies)
        {
        }

        public static ConventionSet Build()
            => new TestCypherConventionSetBuilder(
                new RelationalConventionSetBuilderDependencies(
                    new TestRelationalTypeMapper(
                        new RelationalTypeMapperDependencies()
                    ),
                    null,
                    null
                )
            )
            .AddConventions(
                new CoreConventionSetBuilder(
                    new CoreConventionSetBuilderDependencies(
                        new CoreTypeMapper(
                            new CoreTypeMapperDependencies()
                        )
                    )
                ).CreateConventionSet()
            );
    }
}