// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators;

namespace Microsoft.EntityFrameworkCore.TestUtilities
{
    public class TestCypherCompositeMemberTranslator : RelationalCompositeMemberTranslator
    {
        public TestCypherCompositeMemberTranslator(RelationalCompositeMemberTranslatorDependencies dependencies)
            : base(dependencies)
        {
        }
    }
}