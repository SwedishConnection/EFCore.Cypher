// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.Query.Cypher;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.Sql;

namespace Microsoft.EntityFrameworkCore.TestUtilities
{
    public class TestQueryCypherGenerator: DefaultQueryCypherGenerator {
        public TestQueryCypherGenerator(
            QuerySqlGeneratorDependencies dependencies,
            ReadOnlyExpression readOnlyExpression)
            : base(dependencies, readOnlyExpression)
        {
        }
    }
}