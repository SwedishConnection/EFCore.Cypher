// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query.Cypher;
using Microsoft.EntityFrameworkCore.Storage;

namespace Microsoft.EntityFrameworkCore.Query.Expressions
{
    public sealed class ReadOnlyExpressionDependencies {

        public ReadOnlyExpressionDependencies(
            [NotNull] IQueryCypherGeneratorFactory queryCypherGeneratorFactory,
            [NotNull] IRelationalTypeMapper typeMapper
        ) {
            QueryCypherGeneratorFactory = queryCypherGeneratorFactory;
            TypeMapper = typeMapper;
        }

        /// <summary>
        /// Query Cypher generator
        /// </summary>
        /// <returns></returns>
        public IQueryCypherGeneratorFactory QueryCypherGeneratorFactory { get; }

        /// <summary>
        /// Type mapper
        /// </summary>
        /// <returns></returns>
        public IRelationalTypeMapper TypeMapper { get; }
    }
}