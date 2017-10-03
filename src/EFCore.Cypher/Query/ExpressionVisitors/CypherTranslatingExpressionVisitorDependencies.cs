
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;

namespace Microsoft.EntityFrameworkCore.Query.ExpressionVisitors
{
    public sealed class CypherTranslatingExpressionVisitorDependencies {
        
        public CypherTranslatingExpressionVisitorDependencies(
            [NotNull] ICypherTypeMapper cypherTypeMapper
            // TODO: Add translators (i.e. database function calls)
        ) {
            CypherTypeMapper = cypherTypeMapper;
        }

        public ICypherTypeMapper CypherTypeMapper { get; }
    }
}