// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Microsoft.EntityFrameworkCore.TestUtilities.FakeProvider;
using Remotion.Linq.Parsing.ExpressionVisitors.Transformation;
using Remotion.Linq.Parsing.Structure;
using Remotion.Linq.Parsing.Structure.ExpressionTreeProcessors;
using Xunit;

namespace Microsoft.EntityFrameworkCore.Query.Internal
{
    public class QueryCompilerTest {

        [Fact]
        public void Get_Parsed_Query() {
            var optionsBuilder = new DbContextOptionsBuilder();
            var options = new FakeCypherOptionsExtension().WithConnectionString("Flash=BarryAllen");

            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder)
                .AddOrUpdateExtension(options);  

            var nodeTypeFactory = new DefaultMethodInfoBasedNodeTypeRegistryFactory().Create();

            var parser = new QueryParser(
                new ExpressionTreeParser(
                    nodeTypeFactory,
                    new CompoundExpressionTreeProcessor(
                        new IExpressionTreeProcessor[] {
                            new PartialEvaluatingExpressionTreeProcessor(
                                new EvaluatableExpressionFilter()  // Relational wrapper exists
                            ),
                            new TransformingExpressionTreeProcessor(
                                ExpressionTransformerRegistry.CreateDefault()
                            )
                        }
                    )
                )
            );

            using (var ctx = new CypherFaceDbContext(optionsBuilder.Options)) {
                var query = ctx
                    .Warehouses
                    .Join(
                        ctx.Things,
                        (x) => "OWNS",
                        (x) => String.Empty,
                        (o, i) => new { Warehouse = o, Thing = i }
                    )
                    .Where(x => x.Warehouse.Name == "Ebaz");

                var qm = parser.GetParsedQuery(query.Expression);
            }

            Assert.NotNull(parser);
        }
    }
}