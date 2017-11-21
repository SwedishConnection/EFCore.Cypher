// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Query.Expressions {
    public interface IReadOnlyExpressionFactory {

        ReadOnlyExpression Create(
            [NotNull] CypherQueryCompilationContext queryCompilationContext
        );

        ReadOnlyExpression Create(
            [NotNull] CypherQueryCompilationContext queryCompilationContext,
            [NotNull] string alias
        );
    }
}