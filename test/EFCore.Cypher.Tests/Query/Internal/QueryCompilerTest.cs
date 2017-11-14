// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq.Expressions;
using Remotion.Linq.Parsing.ExpressionVisitors.Transformation;
using Remotion.Linq.Parsing.Structure;
using Remotion.Linq.Parsing.Structure.ExpressionTreeProcessors;
using Xunit;

namespace Microsoft.EntityFrameworkCore.Query.Internal
{
    public class QueryCompilerTest {

        [Fact]
        public void Get_Parsed_Query() {
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

            Assert.NotNull(parser);
        }
    }
}