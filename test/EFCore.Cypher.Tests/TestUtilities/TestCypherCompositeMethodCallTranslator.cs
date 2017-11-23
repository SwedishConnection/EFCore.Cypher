// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;

namespace Microsoft.EntityFrameworkCore.TestUtilities
{
    public class TestCypherCompositeMethodCallTranslator: RelationalCompositeMethodCallTranslator {
        public TestCypherCompositeMethodCallTranslator(RelationalCompositeMethodCallTranslatorDependencies dependencies)
            : base(dependencies)
        {
        }
    }
}