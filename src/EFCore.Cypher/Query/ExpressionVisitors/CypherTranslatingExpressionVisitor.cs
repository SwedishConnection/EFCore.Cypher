using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;
using Remotion.Linq.Parsing;

namespace Microsoft.EntityFrameworkCore.Query.ExpressionVisitors
{
    public class CypherTranslatingExpressionVisitor : ThrowingExpressionVisitor
    {
        private readonly CypherQueryModelVisitor _queryModelVisitor;

        private readonly IRelationalTypeMapper _relationalTypeMapper;
        
        public CypherTranslatingExpressionVisitor(
            [NotNull] CypherTranslatingExpressionVisitorDependencies dependencies,
            [NotNull] CypherQueryModelVisitor queryModelVisitor
        )
        {
            _queryModelVisitor = queryModelVisitor;
            _relationalTypeMapper = dependencies.RelationalTypeMapper;            
        }

        protected override Exception CreateUnhandledItemException<T>(T unhandledItem, string visitMethod)
        {
            throw new NotImplementedException();
        }
    }
}