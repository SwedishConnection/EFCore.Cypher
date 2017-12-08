// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query.Expressions;

namespace Microsoft.EntityFrameworkCore.Query.ExpressionVisitors.Internal
{
    /// <summary>
    /// Coorelation expression visitor
    /// </summary>
    public class CypherCorrelationExpressionVisitor: ExpressionVisitor {

        private ReadOnlyExpression _readOnlyExpression;

        private bool _hasCorrelation;

        public bool HasCorrelation(ReadOnlyExpression readOnlyExpression) {
            _readOnlyExpression = readOnlyExpression;

            Visit(_readOnlyExpression);
            return _hasCorrelation;
        }
        
        public override Expression Visit(Expression expression)
        {
            if (!_hasCorrelation) {
                var storageExpression = expression as StorageExpression;

                if (storageExpression?.Node.QuerySource != null
                    && !_readOnlyExpression.HandlesQuerySource(storageExpression.Node.QuerySource)) {
                        _hasCorrelation = true;
                } else {
                    return base.Visit(expression);
                }
            }

            return expression;
        }
    }
}