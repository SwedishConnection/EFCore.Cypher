// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;

namespace Microsoft.EntityFrameworkCore.TestUtilities
{
    public class TestCypherTypeMapper: RelationalTypeMapper {

        private static readonly RelationalTypeMapping _defaultLongMapping
            = new LongTypeMapping("default_long_mapping", dbType: DbType.Int64);

        private static readonly RelationalTypeMapping _defaultDecimalMapping
            = new DecimalTypeMapping("default_decimal_mapping");

        private static readonly RelationalTypeMapping _defaultBoolMapping
            = new BoolTypeMapping("default_bool_mapping");

        private static readonly RelationalTypeMapping _defaultStringMapping 
            = new StringTypeMapping("default_string");

        private readonly IReadOnlyDictionary<Type, RelationalTypeMapping> _simpleMappings
            = new Dictionary<Type, RelationalTypeMapping>
            {
                { typeof(bool), _defaultBoolMapping },
                { typeof(byte), _defaultLongMapping },
                { typeof(int), _defaultLongMapping },
                { typeof(long), _defaultLongMapping },
                { typeof(float), _defaultDecimalMapping },
                { typeof(decimal), _defaultDecimalMapping },
                { typeof(double), _defaultDecimalMapping },
                { typeof(string), _defaultStringMapping }
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

        protected override string GetColumnType(IProperty property) => property.TestProvider().ColumnType;
    }
}