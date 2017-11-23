// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Query.ExpressionVisitors
{
    public class CypherTranslatingExpressionVisitorFactory: ICypherTranslatingExpressionVisitorFactory {

        public CypherTranslatingExpressionVisitorFactory(
            [NotNull] SqlTranslatingExpressionVisitorDependencies dependencies
        ) {
            Check.NotNull(dependencies, nameof(dependencies));

            Dependencies = dependencies;
        }

        /// <summary>
        /// Dependencies
        /// </summary>
        /// <returns></returns>
        protected virtual SqlTranslatingExpressionVisitorDependencies Dependencies { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="queryModelVisitor"></param>
        /// <param name="targetReadOnlyExpression"></param>
        /// <param name="topLevelWhere"></param>
        /// <param name="inReturn"></param>
        /// <returns></returns>
        public CypherTranslatingExpressionVisitor Create(
            [NotNull] CypherQueryModelVisitor queryModelVisitor, 
            [CanBeNull] ReadOnlyExpression targetReadOnlyExpression = null, 
            [CanBeNull] Expression topLevelWhere = null, 
            bool inReturn = false
        ) => new CypherTranslatingExpressionVisitor(
            Dependencies,
            Check.NotNull(queryModelVisitor, nameof(queryModelVisitor)),
            targetReadOnlyExpression,
            topLevelWhere,
            inReturn
        );
    }
}