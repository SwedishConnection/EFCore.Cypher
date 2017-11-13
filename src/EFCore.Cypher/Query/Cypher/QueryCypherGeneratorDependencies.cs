// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;

namespace Microsoft.EntityFrameworkCore.Query.Cypher
{
    public sealed class QueryCypherGeneratorDependencies
    {
        public QueryCypherGeneratorDependencies(
            [NotNull] IRelationalCommandBuilderFactory commandBuilderFactory,
            [NotNull] IRelationalTypeMapper relationalTypeMapper,
            [NotNull] ISqlGenerationHelper sqlGenerationHelper
        ) {
            CommandBuilderFactory = commandBuilderFactory;
            RelationalTypeMapper = relationalTypeMapper;
            CypherGenerationHelper = sqlGenerationHelper;
        }

        public IRelationalTypeMapper RelationalTypeMapper { get; }

        public ISqlGenerationHelper CypherGenerationHelper { get; }

        public IRelationalCommandBuilderFactory CommandBuilderFactory { get; }
    }
}