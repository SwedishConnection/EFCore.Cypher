// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Remotion.Linq;
using Remotion.Linq.Clauses;

namespace Microsoft.EntityFrameworkCore.Query.Internal
{
    public class CypherResultOperatorHandler : ICypherResultOperatorHandler
    {
        public CypherResultOperatorHandler(
            [NotNull] IModel model
        ) {}

        public Expression HandleResultOperator(
            EntityQueryModelVisitor entityQueryModelVisitor, 
            ResultOperatorBase resultOperator, 
            QueryModel queryModel
        )
        {
            // TODO: 
            throw new System.NotImplementedException();
        }
    }
}