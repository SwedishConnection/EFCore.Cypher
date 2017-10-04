using System;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query.Cypher;

namespace Microsoft.EntityFrameworkCore.Query.Expressions
{
    /// <summary>
    /// See https://github.com/opencypher/openCypher/blob/master/grammar/cypher.xml
    /// 
    /// Read only expression with a Reading clause like Match or Unwind which maps 
    /// to a Relinq QuerySource then a Return.  Unfolding a Match might have an optional 
    /// match, hints and a predicate.  Returns can have a distinct aggregation.
    /// 
    /// A Cypher read only expression can contain multiple reading clauses.
    /// </summary>
    public class ReadOnlyExpression: Expression {

        public ReadOnlyExpression(
            [NotNull] ReadOnlyExpressionDependencies dependencies
        ) {
            Dependencies = dependencies;
        }

        protected virtual ReadOnlyExpressionDependencies Dependencies { get; }

        public override ExpressionType NodeType => ExpressionType.Extension;

        public override Type Type => typeof(object);

        protected override Expression VisitChildren(ExpressionVisitor visitor) => this;

        public virtual IQueryCypherGenerator CreateDefaultQueryCypherGenerator() => 
            Dependencies.QueryCypherGeneratorFactory.CreateDefault(this);
    }
}