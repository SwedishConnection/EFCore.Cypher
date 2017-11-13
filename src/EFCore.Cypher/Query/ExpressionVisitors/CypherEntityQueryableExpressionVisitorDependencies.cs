// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace Microsoft.EntityFrameworkCore.Query.ExpressionVisitors
{
    public sealed class CypherEntityQueryableExpressionVisitorDependencies
    {
        public CypherEntityQueryableExpressionVisitorDependencies (
            [NotNull] IModel model
            // ISelectExpressionFactory
            // IMaterializerFactory
            // IShaperCommandContextFactory
        ) {
            Model = model;
        }

        public IModel Model { get; }


    }
}