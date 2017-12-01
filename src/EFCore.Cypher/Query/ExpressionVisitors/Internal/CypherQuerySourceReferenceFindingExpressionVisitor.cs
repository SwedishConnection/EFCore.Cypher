// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq.Expressions;
using Remotion.Linq.Clauses.Expressions;

namespace Microsoft.EntityFrameworkCore.Query.ExpressionVisitors.Internal
{
    public class CypherQuerySourceReferenceFindingExpressionVisitor: ExpressionVisitorBase {
        public bool FoundAny { get; private set; }

        protected override Expression VisitQuerySourceReference(QuerySourceReferenceExpression expression)
        {
            FoundAny = true;

            return base.VisitQuerySourceReference(expression);
        }
    }
}