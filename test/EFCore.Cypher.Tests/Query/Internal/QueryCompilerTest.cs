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

        /// <summary>
        /// Database context options
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public static DbContextOptions Options(String connectionString) {
            var optionsBuilder = new DbContextOptionsBuilder();
            var options = new FakeCypherOptionsExtension()
                .WithConnectionString(connectionString);

            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder)
                .AddOrUpdateExtension(options);

            return optionsBuilder.Options;
        }

        /// <summary>
        /// Node type proivder
        /// </summary>
        /// <returns></returns>
        public static INodeTypeProvider NodeTypeProvider =>
            new DefaultMethodInfoBasedNodeTypeRegistryFactory().Create();

        /// <summary>
        /// Query Parser
        /// </summary>
        /// <returns></returns>
        public static QueryParser QueryParser =>
            new QueryParser(
                new ExpressionTreeParser(
                    NodeTypeProvider,
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

        /// <summary>
        /// Compile relationship join makes 2 join bodies in the query model
        /// </summary>
        [Fact]
        public void Get_Parsed_Query() {
            DbContextOptions options = Options("Flash=BarryAllen");

            using (var ctx = new CypherFaceDbContext(options)) {
                var query = ctx
                    .Warehouses
                    .Join(
                        ctx.Persons,
                        ctx.Supervising,
                        (o, i, r) => new { Warehouse = o, Person = i, Supervise = r }
                    )
                    .Where(x => x.Warehouse.Location == "Central City");

                var qm = QueryParser.GetParsedQuery(query.Expression);

                Assert.Equal(3, qm.BodyClauses.Count());
            }

        }
    }
}