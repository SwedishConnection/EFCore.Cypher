// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Utilities;
using Remotion.Linq.Parsing.ExpressionVisitors;

namespace System.Linq {
    public static class CypherQueryableExtensions {

        /// <summary>
        /// Join on relationship name
        /// </summary>
        /// <param name="outer"></param>
        /// <param name="inner"></param>
        /// <param name="relationship"></param>
        /// <param name="Expression<Func<TOuter"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static IQueryable<TResult> Join<TOuter, TInner, TResult>(
            [NotNull] this IQueryable<TOuter> outer,
            [NotNull] IEnumerable<TInner> inner,
            string relationship,
            Expression<Func<TOuter, TInner, TResult>> resultSelector
        ) {
            Check.NotNull(outer, nameof(outer));
            Check.NotNull(inner, nameof(inner));
            Check.NotEmpty(relationship, nameof(relationship));
            Check.NotNull(resultSelector, nameof(resultSelector));

            Expression<Func<TOuter, string>> outerKeySelector = (x) => relationship;
            Expression<Func<TInner, string>> innerKeySelector = (x) => relationship;

            return outer.Join(
                inner,
                outerKeySelector,
                innerKeySelector,
                resultSelector
            );
        }

        /// <summary>
        /// Join on relationship
        /// </summary>
        /// <param name="outer"></param>
        /// <param name="inner"></param>
        /// <param name="relationship"></param>
        /// <param name="Expression<Func<TOuter"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static IQueryable<TResult> Join<TOuter, TInner, TRelationship, TResult>(
            [NotNull] this IQueryable<TOuter> outer,
            [NotNull] IEnumerable<TInner> inner,
            [NotNull] IEnumerable<TRelationship> relationship,
            Expression<Func<TOuter, TInner, TRelationship, TResult>> resultSelector
        ) {
            Check.NotNull(outer, nameof(outer));
            Check.NotNull(inner, nameof(inner));
            Check.NotNull(relationship, nameof(relationship));
            Check.NotNull(resultSelector, nameof(resultSelector));

            // fake key selectors
            Expression<Func<TOuter, string>> outerKeySelector = (x) => String.Empty;
            Expression<Func<TRelationship, string>> relationshipKeySelector = (x) => String.Empty;
            Expression<Func<TInner, string>> innerKeySelector = (x) => String.Empty;

            // grab outer and relationship into the first join
            var startParameters = new ParameterExpression[] {
                resultSelector.Parameters.ElementAt(0),
                resultSelector.Parameters.ElementAt(2)
            };

            var startReturnsParameters = startParameters
                .Select(p => new KeyValuePair<string, Type>(p.Name, p.Type))
                .ToList();

            var startReturns = AnonymousTypeBuilder.Create(startReturnsParameters);

            LambdaExpression start = Expression.Lambda(
                Expression.New(
                    startReturns.GetConstructor(
                        startReturnsParameters
                            .Select(p => p.Value)
                            .ToArray()
                    ),
                    startParameters
                ),
                startParameters
            );

            // grab the returns of the first join and inner into the second join
            var endLambdaParameters = new ParameterExpression[] {
                Expression.Parameter(startReturns),
                resultSelector.Parameters.ElementAt(1)
            };

            Expression end = Expression.Lambda(
                resultSelector.Body,
                endLambdaParameters
            );

            // replace using relinq outer and relationship parameter expressions
            var outerMemberExpression = Expression.PropertyOrField(
                endLambdaParameters.ElementAt(0),
                resultSelector.Parameters.ElementAt(0).Name
            );
            var relationshipMemberExpression = Expression.PropertyOrField(
                endLambdaParameters.ElementAt(0),
                resultSelector.Parameters.ElementAt(2).Name
            );

            end = ReplacingExpressionVisitor.Replace(
                resultSelector.Parameters.ElementAt(0),
                outerMemberExpression,
                end
            );
            end = ReplacingExpressionVisitor.Replace(
                resultSelector.Parameters.ElementAt(2),
                relationshipMemberExpression,
                end
            );

            // construct method calls
            var startMethodInfo = Join_TOuter_TInner_TKey_TResult_5(
                typeof(TOuter),
                typeof(TRelationship),
                typeof(string),
                startReturns
            );
            
            var startExpression = Expression.Call(
                null,
                startMethodInfo,
                outer.Expression,
                GetSourceExpression(relationship),
                Expression.Quote(outerKeySelector),
                Expression.Quote(relationshipKeySelector),
                Expression.Quote(start)
            );

            var endMethodInfo = Join_TOuter_TInner_TKey_TResult_5(
                startReturns,
                typeof(TInner),
                typeof(string),
                typeof(TResult)
            );

            var startReturnsKeySelector = Expression.Lambda(
                Expression.Constant(String.Empty),
                new ParameterExpression[] {
                    Expression.Parameter(startReturns),
                    Expression.Parameter(typeof(string))
                }
            );

            var wrapped = Expression.Call(
                null,
                endMethodInfo,
                startExpression,
                GetSourceExpression(inner),
                Expression.Quote(startReturnsKeySelector),
                Expression.Quote(innerKeySelector),
                Expression.Quote(end)
            );

            return outer.Provider.CreateQuery<TResult>(wrapped);
        }

        /// <summary>
        /// From <see cref="Queryable"/>
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private static Expression GetSourceExpression<TSource>(IEnumerable<TSource> source)
        {
            IQueryable<TSource> q = source as IQueryable<TSource>;
            return q != null ? q.Expression : Expression.Constant(source, typeof(IEnumerable<TSource>));
        }

        /// <summary>
        /// From <see cref="CachedReflection"/>
        /// </summary>
        private static MethodInfo s_Join_TOuter_TInner_TKey_TResult_5;

        /// <summary>
        /// From <see cref="CachedReflection"/>
        /// </summary>
        /// <param name="TOuter"></param>
        /// <param name="TInner"></param>
        /// <param name="TKey"></param>
        /// <param name="TResult"></param>
        /// <returns></returns>
        public static MethodInfo Join_TOuter_TInner_TKey_TResult_5(Type TOuter, Type TInner, Type TKey, Type TResult) =>
             (s_Join_TOuter_TInner_TKey_TResult_5 ??
             (s_Join_TOuter_TInner_TKey_TResult_5 = new Func<IQueryable<object>, IEnumerable<object>, Expression<Func<object, object>>, Expression<Func<object, object>>, Expression<Func<object, object, object>>, IQueryable<object>>(Queryable.Join).GetMethodInfo().GetGenericMethodDefinition()))
              .MakeGenericMethod(TOuter, TInner, TKey, TResult);
    }
}