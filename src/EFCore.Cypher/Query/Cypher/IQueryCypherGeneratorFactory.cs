using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query.Expressions;

namespace Microsoft.EntityFrameworkCore.Query.Cypher
{
    public interface IQueryCypherGeneratorFactory {

        IQueryCypherGenerator CreateDefault([NotNull] ReadOnlyExpression readOnlyExpression);
        
    }
}