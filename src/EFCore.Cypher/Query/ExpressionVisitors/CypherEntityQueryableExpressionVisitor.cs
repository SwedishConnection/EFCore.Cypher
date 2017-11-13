// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Utilities;
using Remotion.Linq.Clauses;

namespace Microsoft.EntityFrameworkCore.Query.ExpressionVisitors
{
    public class CypherEntityQueryableExpressionVisitor: EntityQueryableExpressionVisitor {

        private readonly IModel _model;

        private readonly IQuerySource _querySource;

        public CypherEntityQueryableExpressionVisitor(
            [NotNull] CypherEntityQueryableExpressionVisitorDependencies dependencies,
            [NotNull] CypherQueryModelVisitor queryModelVisitor,
            [CanBeNull] IQuerySource querySource
        ) : base(Check.NotNull(queryModelVisitor, nameof(queryModelVisitor))) {
            Check.NotNull(dependencies, nameof(dependencies));

            _model = dependencies.Model;
            _querySource = querySource;
        }

        protected override Expression VisitEntityQueryable([NotNull] Type elementType)
        {
            Check.NotNull(elementType, nameof(elementType));

            // TODO: Grab the entity from Clr, make a select expression, add the query and return an expression

            throw new NotImplementedException();
        }
    }
}