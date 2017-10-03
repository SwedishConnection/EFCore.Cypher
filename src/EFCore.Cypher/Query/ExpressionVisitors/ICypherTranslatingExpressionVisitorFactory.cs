using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Query.ExpressionVisitors
{
    public interface ICypherTranslatingExpressionVisitorFactory {

        CypherTranslatingExpressionVisitor Create(
            [NotNull] CypherQueryModelVisitor queryModelVisitor
            // TODO: Pass MatchExpression
        );
    }
}