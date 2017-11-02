using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;

namespace Microsoft.EntityFrameworkCore.Query.Cypher
{
    public sealed class QueryCypherGeneratorDependencies
    {
        public QueryCypherGeneratorDependencies(
            [NotNull] ICypherCommandBuilderFactory commandBuilderFactory,
            [NotNull] IRelationalTypeMapper relationalTypeMapper,
            [NotNull] ISqlGenerationHelper sqlGenerationHelper
        ) {
            CommandBuilderFactory = commandBuilderFactory;
            RelationalTypeMapper = relationalTypeMapper;
            CypherGenerationHelper = sqlGenerationHelper;
        }

        public IRelationalTypeMapper RelationalTypeMapper { get; }

        public ISqlGenerationHelper CypherGenerationHelper { get; }

        public ICypherCommandBuilderFactory CommandBuilderFactory { get; }
    }
}