// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Microsoft.EntityFrameworkCore.Query.ExpressionVisitors.Internal
{
    public class CypherRequiresMaterializationExpressionVisitorFactory : IRequiresMaterializationExpressionVisitorFactory
    {
        private readonly IModel _model;

        public CypherRequiresMaterializationExpressionVisitorFactory(
            [NotNull] IModel model
        ) {
            _model = model;
        }

        public virtual RequiresMaterializationExpressionVisitor Create(EntityQueryModelVisitor queryModelVisitor)
            => new CypherRequiresMaterializationExpressionVisitor(_model, queryModelVisitor);
    }
}