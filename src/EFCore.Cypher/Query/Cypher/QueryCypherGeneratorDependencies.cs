using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;

namespace Microsoft.EntityFrameworkCore.Query.Cypher
{
    public sealed class QueryCypherGeneratorDependencies
    {
        public QueryCypherGeneratorDependencies(
            [NotNull] ICypherCommandBuilderFactory commandBuilderFactory,
            [NotNull] ICypherTypeMapper cypherTypeMapper,
            [NotNull] ICypherGenerationHelper cypherGenerationHelper
        ) {
            CommandBuilderFactory = commandBuilderFactory;
            CypherTypeMapper = cypherTypeMapper;
            CypherGenerationHelper = cypherGenerationHelper;
        }

        public ICypherTypeMapper CypherTypeMapper { get; }

        public ICypherGenerationHelper CypherGenerationHelper { get; }

        public ICypherCommandBuilderFactory CommandBuilderFactory { get; }
    }
}