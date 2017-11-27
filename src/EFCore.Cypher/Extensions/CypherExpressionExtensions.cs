// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Internal
{
    public static class CypherExpressionExtensions {
        public static bool IsSimple(
            [NotNull] this Expression expression
        ) {
            Check.NotNull(expression, nameof(expression));

            var unwrappedExpression = expression.RemoveConvert();

            return unwrappedExpression is ConstantExpression
                   || unwrappedExpression is StorageExpression
                   || unwrappedExpression is ParameterExpression
                   || unwrappedExpression is CypherAliasExpression;
        }
    }
}