using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query.Cypher;

namespace Microsoft.EntityFrameworkCore.Query.Expressions
{
    public sealed class ReadOnlyExpressionDependencies {

        public ReadOnlyExpressionDependencies(
            [NotNull] IQueryCypherGeneratorFactory queryCypherGeneratorFactory
        ) {
            QueryCypherGeneratorFactory = queryCypherGeneratorFactory;
        }

        public IQueryCypherGeneratorFactory QueryCypherGeneratorFactory { get; }
    }
}