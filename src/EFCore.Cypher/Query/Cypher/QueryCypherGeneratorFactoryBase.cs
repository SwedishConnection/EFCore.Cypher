// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.Sql;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Query.Cypher
{
    public abstract class QueryCypherGeneratorFactoryBase : IQueryCypherGeneratorFactory
    {
        protected QueryCypherGeneratorFactoryBase(
            [NotNull] QuerySqlGeneratorDependencies dependencies
        ) {
            Check.NotNull(dependencies, nameof(dependencies));

            Dependencies = dependencies;
        }

        /// <summary>
        /// Dependencies
        /// </summary>
        /// <returns></returns>
        protected virtual QuerySqlGeneratorDependencies Dependencies { get; }

        /// <summary>
        /// Query Sql (cypher) generator
        /// </summary>
        /// <param name="readOnlyExpression"></param>
        /// <returns></returns>
        public abstract IQuerySqlGenerator CreateDefault(
            [NotNull] ReadOnlyExpression readOnlyExpression
        ); 
    }
}
