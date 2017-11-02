
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;

namespace Microsoft.EntityFrameworkCore.Query.ExpressionVisitors
{
    public sealed class CypherTranslatingExpressionVisitorDependencies {
        
        public CypherTranslatingExpressionVisitorDependencies(
            [NotNull] IRelationalTypeMapper relationalTypeMapper
            // TODO: Add translators (i.e. database function calls)
        ) {
            RelationalTypeMapper = relationalTypeMapper;
        }

        public IRelationalTypeMapper RelationalTypeMapper { get; }
    }
}