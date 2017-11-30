// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace Microsoft.EntityFrameworkCore {

    public class CypherQueryableExtensionsTest {

        [Fact]
        public void Join_relationship_name() {
            var outer = new int[] { 1, 2, 3 }.AsQueryable();
            var inner = new int[] { 4, 5, 6 }.AsQueryable();

            var queryable = outer.Join(
                inner,
                "NEXT",
                (o) => o,
                (i) => i,
                (o, i, r) => new { o = o, i = i }
            );

            Assert.Equal(ExpressionType.Call, queryable.Expression.NodeType);
            MethodCallExpression method = (MethodCallExpression)queryable.Expression;

            Assert.Equal("Join", method.Method.Name);

            // TODO: Investigate why the key selector returns an anonymous type rather than string
        }

        [Fact]
        public void Join_relationship() {
            var outer = new int[] { 1, 2, 3 }.AsQueryable();
            var inner = new int[] { 4, 5, 6 }.AsQueryable();
            var relationship = new string[] { "NEXT" }.AsQueryable();

            var queryable = outer.Join(
                inner,
                relationship,
                (o) => o,
                (i) => i,
                (o, i, r) => new { o = o, i = i, r = r }
            );

            Assert.Equal(ExpressionType.Call, queryable.Expression.NodeType);
            MethodCallExpression method = (MethodCallExpression)queryable.Expression;

            Assert.Equal("Join", method.Method.Name);
        }
    }
}