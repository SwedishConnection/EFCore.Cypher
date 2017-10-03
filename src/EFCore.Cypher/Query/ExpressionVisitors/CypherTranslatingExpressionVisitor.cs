using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;
using Remotion.Linq.Parsing;

namespace Microsoft.EntityFrameworkCore.Query.ExpressionVisitors
{
    public class CypherTranslatingExpressionVisitor : ThrowingExpressionVisitor
    {
        private readonly CypherQueryModelVisitor _queryModelVisitor;

        private readonly ICypherTypeMapper _cypherTypeMapper;
        
        public CypherTranslatingExpressionVisitor(
            [NotNull] CypherTranslatingExpressionVisitorDependencies dependencies,
            [NotNull] CypherQueryModelVisitor queryModelVisitor
        )
        {
            _queryModelVisitor = queryModelVisitor;
            _cypherTypeMapper = dependencies.CypherTypeMapper;            
        }

        protected override Exception CreateUnhandledItemException<T>(T unhandledItem, string visitMethod)
        {
            throw new NotImplementedException();
        }
    }
}