// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Storage;
using Remotion.Linq.Clauses;

namespace Microsoft.EntityFrameworkCore.Query.ExpressionVisitors.Internal
{
    public interface ICypherMaterializerFactory {

        Expression<Func<ValueBuffer, object>> CreateMaterializer(
            [NotNull] IEntityType entityType,
            [NotNull] ReadOnlyExpression readOnlyExpression,
            [NotNull] Func<IProperty, ReadOnlyExpression, int> returnItemHandler,
            [CanBeNull] IQuerySource querySource,
            out Dictionary<Type, int[]> typeIndexMapping
        );
    }
}