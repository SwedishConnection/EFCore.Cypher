// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query.Expressions;

namespace Microsoft.EntityFrameworkCore.Query.Cypher
{
    public interface ICypherExpressionVisitor {
        
        Expression VisitReadOnly([NotNull] ReadOnlyExpression expression);

        Expression VisitMatch([NotNull] MatchExpression expression);

        Expression VisitStorage([NotNull] StorageExpression expression);

        Expression VisitAlias([NotNull] CypherAliasExpression expression);

        Expression VisitRelationshipDetail([NotNull] RelationshipDetailExpression expression);

        Expression VisitRelationshipPattern([NotNull] RelationshipPatternExpression expression);

        Expression VisitNodePattern([NotNull] NodePatternExpression expression);

        Expression VisitPattern([NotNull] PatternExpression expression);
    }
}