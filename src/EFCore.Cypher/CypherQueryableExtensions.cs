// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Utilities;

namespace System.Linq {
    public static class CypherQueryableExtensions {

        /// <summary>
        /// Match by name
        /// </summary>
        /// <param name="outer"></param>
        /// <param name="inner"></param>
        /// <param name="relationship"></param>
        /// <param name="Expression<Func<TOuter"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static IQueryable<TResult> Match<TOuter, TInner, TResult>(
            [NotNull] this IQueryable<TOuter> outer,
            [NotNull] IEnumerable<TInner> inner,
            string relationship,
            Expression<Func<TOuter, TInner, TResult>> selector
        ) {
            Check.NotNull(outer, nameof(outer));
            Check.NotNull(inner, nameof(inner));
            Check.NotEmpty(relationship, nameof(relationship));
            Check.NotNull(selector, nameof(selector));

            return outer
                .Provider
                .CreateQuery<TResult>(
                    Expression.Call(
                        null,
                        CypherCachedReflectionInfo.Match_TOuter_TInner_TResult(
                            typeof(TOuter), 
                            typeof(TInner), 
                            typeof(TResult)
                        )
                    )
                );
        }

        /// <summary>
        /// Match
        /// </summary>
        /// <param name="outer"></param>
        /// <param name="inner"></param>
        /// <param name="relationship"></param>
        /// <param name="Expression<Func<TOuter"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static IQueryable<TResult> Match<TOuter, TInner, TRelationship, TResult>(
            [NotNull] this IQueryable<TOuter> outer,
            [NotNull] IEnumerable<TInner> inner,
            [NotNull] IQueryable<TRelationship> relationship,
            Expression<Func<TOuter, TInner, TResult>> selector
        ) {
            Check.NotNull(outer, nameof(outer));
            Check.NotNull(inner, nameof(inner));
            Check.NotNull(relationship, nameof(relationship));
            Check.NotNull(selector, nameof(selector));

            return outer
                .Provider
                .CreateQuery<TResult>(
                    Expression.Call(
                        null,
                        CypherCachedReflectionInfo.Match_TOuter_TInner_TResult(
                            typeof(TOuter), 
                            typeof(TInner), 
                            typeof(TResult)
                        )
                    )
                );
        }
    }
}