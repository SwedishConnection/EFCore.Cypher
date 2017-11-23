// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query.Cypher;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Query.Expressions
{
    public class CypherAliasExpression: AliasExpression {

        public CypherAliasExpression(
            [NotNull] string alias,
            [NotNull] Expression expression
        ): base(alias, expression) {
        }

        protected override Expression Accept(ExpressionVisitor visitor)
        {
            Check.NotNull(visitor, nameof(visitor));

            var concrete = visitor as ICypherExpressionVisitor;

            return concrete is null
                ? base.Accept(visitor)
                : concrete.VisitAlias(this);
        }
    }
}