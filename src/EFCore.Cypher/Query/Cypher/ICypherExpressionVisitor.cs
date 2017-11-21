// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query.Expressions;

namespace Microsoft.EntityFrameworkCore.Query.Cypher
{
    public interface ICypherExpressionVisitor {
        
        Expression VisitReadOnly([NotNull] ReadOnlyExpression readOnlyExpression);

        Expression VisitMatch([NotNull] MatchExpression matchExpression);
    }
}