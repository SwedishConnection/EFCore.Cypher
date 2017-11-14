// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace System.Linq {

    public static class CypherCachedReflectionInfo {

        private static MethodInfo s_Match_TOuter_TInner_TResult;

        public static MethodInfo Match_TOuter_TInner_TResult(Type TOuter, Type TInner, Type TResult)
            => (s_Match_TOuter_TInner_TResult ??
                (s_Match_TOuter_TInner_TResult = new Func<IQueryable<object>, IEnumerable<object>, string, Expression<Func<object, object, object>>, IQueryable<object>>(CypherQueryableExtensions.Match).GetMethodInfo().GetGenericMethodDefinition()))
                .MakeGenericMethod(
                    TOuter, 
                    TInner, 
                    TResult
                );

        private static MethodInfo s_Match_TOuter_TInner_TRelationship_TResult;

        public static MethodInfo Match_TOuter_TInner_TRelationship_TResult(Type TOuter, Type TInner, Type TRelationship, Type TResult)
            => (s_Match_TOuter_TInner_TRelationship_TResult ??
                (s_Match_TOuter_TInner_TRelationship_TResult = new Func<IQueryable<object>, IEnumerable<object>, IQueryable<object>, Expression<Func<object, object, object>>, IQueryable<object>>(CypherQueryableExtensions.Match).GetMethodInfo().GetGenericMethodDefinition()))
                .MakeGenericMethod(
                    TOuter, 
                    TInner,
                    TRelationship, 
                    TResult
                );
    }
}