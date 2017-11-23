// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Query.ExpressionVisitors
{
    public class CypherProjectionExpressionVisitorDependencies {

        public CypherProjectionExpressionVisitorDependencies(
            [NotNull] ICypherTranslatingExpressionVisitorFactory cypherTranslatingExpressionVisitorFactory,
            [NotNull] IEntityMaterializerSource entityMaterializerSource
        ) {
            Check.NotNull(cypherTranslatingExpressionVisitorFactory, nameof(cypherTranslatingExpressionVisitorFactory));
            Check.NotNull(entityMaterializerSource, nameof(entityMaterializerSource));

            CypherTranslatingExpressionVisitorFactory = cypherTranslatingExpressionVisitorFactory;
            EntityMaterializerSource = entityMaterializerSource;
        }

        /// <summary>
        /// Translating expression visitor factory
        /// </summary>
        /// <returns></returns>
        public ICypherTranslatingExpressionVisitorFactory CypherTranslatingExpressionVisitorFactory { get; }

        /// <summary>
        /// Entity materializer
        /// </summary>
        /// <returns></returns>
        public IEntityMaterializerSource EntityMaterializerSource { get; }
    }
}