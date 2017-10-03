using System;
using System.Linq.Expressions;

namespace Microsoft.EntityFrameworkCore.Query.Expressions
{
    /// <summary>
    /// See https://github.com/opencypher/openCypher/blob/master/grammar/cypher.xml
    /// 
    /// Reading (clause) like Match which maps to a Relinq QuerySource.  Unclear
    /// if other types of reading clauses like LOAD CSV should be involved.
    /// 
    /// A Cypher read part can contain multiple reading clauses.
    /// </summary>
    public class ReadingExpression: Expression {

        public override ExpressionType NodeType => ExpressionType.Extension;

        public override Type Type => typeof(object);

        protected override Expression VisitChildren(ExpressionVisitor visitor) => this;
    }
}