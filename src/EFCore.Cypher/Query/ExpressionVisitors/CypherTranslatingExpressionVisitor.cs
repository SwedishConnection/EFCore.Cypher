using System;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Parsing;

namespace Microsoft.EntityFrameworkCore.Query.ExpressionVisitors
{
    public class CypherTranslatingExpressionVisitor : ThrowingExpressionVisitor
    {
        private readonly IExpressionFragmentTranslator _compositeExpressionFragmentTranslator;
        
        private readonly CypherQueryModelVisitor _queryModelVisitor;

        private readonly IRelationalTypeMapper _relationalTypeMapper;

        private readonly ReadOnlyExpression _targetReadOnlyExpression;

        private bool _isTopLevelReturn;
        
        public CypherTranslatingExpressionVisitor(
            [NotNull] SqlTranslatingExpressionVisitorDependencies dependencies,
            [NotNull] CypherQueryModelVisitor queryModelVisitor,
            [CanBeNull] ReadOnlyExpression targetReadOnlyExpresion = null,
            [CanBeNull] Expression topLevelWhere = null,
            bool inReturn = false
        )
        {
            Check.NotNull(dependencies, nameof(dependencies));
            Check.NotNull(queryModelVisitor, nameof(queryModelVisitor));

            _compositeExpressionFragmentTranslator = dependencies.CompositeExpressionFragmentTranslator;
            _targetReadOnlyExpression = targetReadOnlyExpresion;
            _isTopLevelReturn = inReturn;
            
            _queryModelVisitor = queryModelVisitor;
            _relationalTypeMapper = dependencies.RelationalTypeMapper;            
        }

        public override Expression Visit(Expression expression)
        {
            var translatedExpression = _compositeExpressionFragmentTranslator.Translate(expression);

            if (translatedExpression != null
                && translatedExpression != expression)
            {
                return Visit(translatedExpression);
            }

            if (expression != null
                && (expression.NodeType == ExpressionType.Convert
                    || expression.NodeType == ExpressionType.Negate
                    || expression.NodeType == ExpressionType.New))
            {
                return base.Visit(expression);
            }

            var isTopLevelReturn = _isTopLevelReturn;
            _isTopLevelReturn = false;

            try
            {
                return base.Visit(expression);
            }
            finally
            {
                _isTopLevelReturn = isTopLevelReturn;
            }
        }

        protected override Expression VisitBinary(BinaryExpression expression)
        {
            throw new NotImplementedException();
        }

        protected override Expression VisitConditional(ConditionalExpression expression)
        {
            throw new NotImplementedException();
        }

        protected override Expression VisitMethodCall(MethodCallExpression methodCallExpression)
        {
            throw new NotImplementedException();
        }

        protected override Expression VisitMember(MemberExpression memberExpression)
        {
            Check.NotNull(memberExpression, nameof(memberExpression));

            if (!(memberExpression.Expression.RemoveConvert() is QuerySourceReferenceExpression)
                && !(memberExpression.Expression.RemoveConvert() is SubQueryExpression))
            {
            }

            return TryBindMemberOrMethodToReadOnlyExpression(
                    memberExpression, 
                    (expression, visitor, binder) => visitor.BindMemberExpression(expression, binder)
                )
                ?? TryBindQuerySourcePropertyExpression(memberExpression)
                ?? _queryModelVisitor.BindMemberToOuterQueryParameter(memberExpression);
        }

        protected override Expression VisitUnary(UnaryExpression expression)
        {
            throw new NotImplementedException();
        }

        protected override Expression VisitNew(NewExpression expression)
        {
            throw new NotImplementedException();
        }

        protected override Expression VisitSubQuery(SubQueryExpression expression)
        {
            throw new NotImplementedException();
        }

        protected override Expression VisitConstant(ConstantExpression expression)
        {
            throw new NotImplementedException();
        }

        protected override Expression VisitParameter(ParameterExpression expression)
        {
            throw new NotImplementedException();
        }

        protected override Expression VisitExtension(Expression expression)
        {
            throw new NotImplementedException();
        }

        protected override Expression VisitQuerySourceReference(QuerySourceReferenceExpression expression)
        {
            throw new NotImplementedException();
        }

        protected override Exception CreateUnhandledItemException<T>(T unhandledItem, string visitMethod)
        {
            throw new NotImplementedException();
        }

        private Expression TryBindMemberOrMethodToReadOnlyExpression<TExpression>(
            TExpression sourceExpression,
            Func<TExpression, CypherQueryModelVisitor, Func<IProperty, IQuerySource, ReadOnlyExpression, Expression>, Expression> binder 
        ) {
            Expression BindPropertyToReadOnlyExpression(
                IProperty property, 
                IQuerySource querySource, 
                ReadOnlyExpression readOnlyExpression
            ) => readOnlyExpression.BindProperty(property, querySource);

            var boundExpression = binder(
                sourceExpression,
                _queryModelVisitor,
                (property, querySource, readOnlyExpression) => {
                    var boundPropertyExpression = BindPropertyToReadOnlyExpression(
                        property,
                        querySource,
                        readOnlyExpression
                    );

                    if (!(_targetReadOnlyExpression is null)
                        && readOnlyExpression != _targetReadOnlyExpression) {
                        readOnlyExpression.AddReturnItem(boundPropertyExpression);
                    }

                    return boundPropertyExpression;
                }
            );

            if (boundExpression != null)
            {
                return boundExpression;
            }

            // TODO: Outer

            return null;
        }

        private Expression TryBindQuerySourcePropertyExpression(MemberExpression memberExpression) {
            if (memberExpression.Expression is QuerySourceReferenceExpression qsre)
            {
                var readOnlyExpression = _queryModelVisitor.TryGetQuery(qsre.ReferencedQuerySource);

                return readOnlyExpression?.GetReturnForMemberInfo(memberExpression.Member);
            }

            return null;
        }
    }
}