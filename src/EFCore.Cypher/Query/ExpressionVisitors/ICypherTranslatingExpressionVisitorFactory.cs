// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query.Expressions;

namespace Microsoft.EntityFrameworkCore.Query.ExpressionVisitors
{
    public interface ICypherTranslatingExpressionVisitorFactory {

        CypherTranslatingExpressionVisitor Create(
            [NotNull] CypherQueryModelVisitor queryModelVisitor,
            [CanBeNull] ReadOnlyExpression targetReadOnlyExpression = null,
            [CanBeNull] Expression topLevelWhere = null,
            bool inReturn = false
        );
    }
}