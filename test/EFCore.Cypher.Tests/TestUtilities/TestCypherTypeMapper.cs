// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;

namespace Microsoft.EntityFrameworkCore.TestUtilities
{
    public class TestCypherTypeMapper: RelationalTypeMapper {

        private readonly IReadOnlyDictionary<Type, RelationalTypeMapping> _simpleMappings
            = new Dictionary<Type, RelationalTypeMapping>
            {
            };

        private readonly IReadOnlyDictionary<string, RelationalTypeMapping> _simpleNameMappings
            = new Dictionary<string, RelationalTypeMapping>
            {
            };

        public TestCypherTypeMapper(RelationalTypeMapperDependencies dependencies)
            : base(dependencies)
        {
        }

        protected override IReadOnlyDictionary<Type, RelationalTypeMapping> GetClrTypeMappings()
            => _simpleMappings;
        
        protected override IReadOnlyDictionary<string, RelationalTypeMapping> GetStoreTypeMappings()
            => _simpleNameMappings;

        public override IStringRelationalTypeMapper StringMapper { get { throw new NotImplementedException(); } }

        protected override string GetColumnType(IProperty property) => property.TestProvider().ColumnType;
    }
}