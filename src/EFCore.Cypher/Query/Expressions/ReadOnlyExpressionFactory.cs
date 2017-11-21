// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Query.Expressions
{
    public class ReadOnlyExpressionFactory : IReadOnlyExpressionFactory
    {
        public ReadOnlyExpressionFactory(
            [NotNull] ReadOnlyExpressionDependencies dependencies
        ) {
            Check.NotNull(dependencies, nameof(dependencies));

            Dependencies = dependencies;
        }

        protected virtual ReadOnlyExpressionDependencies Dependencies { get; }

        public ReadOnlyExpression Create(
            [NotNull] CypherQueryCompilationContext queryCompilationContext
        ) => new ReadOnlyExpression(Dependencies, queryCompilationContext);

        public ReadOnlyExpression Create(
            [NotNull] CypherQueryCompilationContext queryCompilationContext, 
            [NotNull] string alias
        ) => new ReadOnlyExpression(
            Dependencies, 
            queryCompilationContext, 
            Check.NotEmpty(alias, nameof(alias))
        );
    }
}