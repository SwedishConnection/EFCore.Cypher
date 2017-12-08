// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.


using System.Linq;
using System.Linq.Expressions;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;

namespace Microsoft.EntityFrameworkCore.Query.ExpressionVisitors.Internal
{
    /// <summary>
    /// Finds query source in selectors
    /// </summary>
    public class CypherKeySelectorVisitor: ExpressionVisitorBase {

        private string _propertyName;

        public IQuerySource QuerySource { get; private set; }

        protected override Expression VisitQuerySourceReference(QuerySourceReferenceExpression expression)
        {
            QuerySource = expression.ReferencedQuerySource;

            return expression;
        }

        protected override Expression VisitMember(MemberExpression expression)
        {
            _propertyName = expression.Member.Name;
            return base.VisitMember(expression);
        }

        protected override Expression VisitNew(NewExpression expression) {
            var candidate = expression
                .Arguments
                .OfType<QuerySourceReferenceExpression>()
                .FirstOrDefault(a => a.ReferencedQuerySource.ItemName == _propertyName);

            if (!(candidate is null)) {
                base.Visit(candidate);
                return expression;
            }

            return base.VisitNew(expression);
        }
    }
}