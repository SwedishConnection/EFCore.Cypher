using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query.Expressions;

namespace Microsoft.EntityFrameworkCore.Query.Cypher
{
    public interface ICypherExpressionVisitor {
        
        Expression VisitReadOnly([NotNull] ReadOnlyExpression readOnlyExpression);
    }
}