// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Utilities;
using Remotion.Linq.Parsing.ExpressionVisitors;
using Xunit;

namespace Microsoft.EntityFrameworkCore.Relinq {

    public class JoinRewriterTest {

        [Fact]
        public void Chaining() {
            // exposed extension footprint
            Expression<Func<Warehouse, Thing, Ownership, object>> target = (o, i, r) 
                => new { Outer = o, Inner = i, Relationship = r };

            // rip out the outer and the relationship into a join function
            var headParameters = new ParameterExpression[] { 
                target.Parameters.ElementAt(0), target.Parameters.ElementAt(2) 
            };

            var headReturnsParameters = headParameters
                .Select(p => new KeyValuePair<string, Type>(p.Name, p.Type))
                .ToList();

            var headReturns = AnonymousTypeBuilder.Create(headReturnsParameters);

            LambdaExpression head = Expression.Lambda(
                Expression.New(
                    headReturns.GetConstructor(
                        headReturnsParameters.Select(p => p.Value).ToArray()
                    ),
                    headParameters
                ),
                headParameters
            );

            // use the returns from the header with the inner as a join function
            var tailLambdaParameters = new ParameterExpression[] {
                Expression.Parameter(headReturns),
                target.Parameters.ElementAt(1)
            };

            LambdaExpression tail = Expression.Lambda(
                target.Body,
                tailLambdaParameters
            );


            // replace tail's outer references in the body
            var outerPropertyAccess = Expression.PropertyOrField(
                tailLambdaParameters.ElementAt(0),
                target.Parameters.ElementAt(0).Name
            );
            var stepOne = ReplacingExpressionVisitor.Replace(
                target.Parameters.ElementAt(0),
                outerPropertyAccess,
                tail
            );

            var relationshipPropertyAccess = Expression.PropertyOrField(
                tailLambdaParameters.ElementAt(0),
                target.Parameters.ElementAt(2).Name
            );
            var stepTwo = ReplacingExpressionVisitor.Replace(
                target.Parameters.ElementAt(2),
                relationshipPropertyAccess,
                stepOne
            );

            Assert.Equal(
                outerPropertyAccess,
                ((NewExpression)((LambdaExpression)stepTwo).Body).Arguments.First()
            );

            Assert.Equal(
                relationshipPropertyAccess,
                ((NewExpression)((LambdaExpression)stepTwo).Body).Arguments.ElementAt(2)
            );
        }

        private class Warehouse {
            public string Location { get; set; }

            [Relationship(typeof(Ownership))]
            public IEnumerable<Thing> Things { get; set; }
        }

        private class Thing {
            public string Name { get; set; }
        }

        private class Ownership {
            public bool Private { get; set; }
        }
    }
}