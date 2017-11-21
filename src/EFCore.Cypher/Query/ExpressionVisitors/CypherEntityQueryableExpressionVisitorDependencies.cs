// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors.Internal;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace Microsoft.EntityFrameworkCore.Query.ExpressionVisitors
{
    public sealed class CypherEntityQueryableExpressionVisitorDependencies
    {
        public CypherEntityQueryableExpressionVisitorDependencies (
            [NotNull] IModel model,
            [NotNull] IReadOnlyExpressionFactory readOnlyExpressionFactory,
            [NotNull] ICypherMaterializerFactory materializerFactory,
            [NotNull] IShaperCommandContextFactory shaperCommandContextFactory
        ) {
            Model = model;
            ReadOnlyExpressionFactory = readOnlyExpressionFactory;
            MaterializerFactory = materializerFactory;
            ShaperCommandContextFactory = shaperCommandContextFactory;
        }

        /// <summary>
        /// Model
        /// </summary>
        /// <returns></returns>
        public IModel Model { get; }

        /// <summary>
        /// Read only expression factory
        /// </summary>
        /// <returns></returns>
        public IReadOnlyExpressionFactory ReadOnlyExpressionFactory { get; }

        /// <summary>
        /// Extracts return items as a type mapping from read only expression
        /// </summary>
        /// <returns></returns>
        public ICypherMaterializerFactory MaterializerFactory { get; }

        /// <summary>
        /// Shaper command context factory
        /// </summary>
        /// <returns></returns>
        public IShaperCommandContextFactory ShaperCommandContextFactory { get; }
    }
}