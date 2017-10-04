using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Storage;
using Remotion.Linq.Parsing;

namespace Microsoft.EntityFrameworkCore.Query.Cypher
{

    public class DefaultQueryCypherGenerator : ThrowingExpressionVisitor, ICypherExpressionVisitor, IQueryCypherGenerator
    {
        protected DefaultQueryCypherGenerator(
            [NotNull] QueryCypherGeneratorDependencies dependencies,
            [NotNull] ReadOnlyExpression readOnlyExpression
        ) {
            Dependencies = dependencies;
            ReadOnlyExpression = readOnlyExpression;
        }

        protected virtual QueryCypherGeneratorDependencies Dependencies { get; }

        protected virtual ReadOnlyExpression ReadOnlyExpression { get; }

        public virtual bool IsCachable { get; private set; }

        public ICypherCommand GenerateCypher([NotNull] IReadOnlyDictionary<string, object> parameterValues)
        {
            throw new NotImplementedException();
        }

        public Expression VisitReadOnly([NotNull] ReadOnlyExpression readOnlyExpression)
        {
            throw new NotImplementedException();
        }

        protected override Exception CreateUnhandledItemException<T>(T unhandledItem, string visitMethod)
        {
            throw new NotImplementedException();
        }
    }
}