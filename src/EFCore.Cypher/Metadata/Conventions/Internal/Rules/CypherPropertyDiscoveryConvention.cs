// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal
{
    public class CypherPropertyDiscoveryConvention : ICypherEntityAddedConvention, ICypherBaseEntityChangedConvention
    {
        private readonly ITypeMapper _typeMapper;

        public CypherPropertyDiscoveryConvention([NotNull] ITypeMapper typeMapper) {
            Check.NotNull(typeMapper, nameof(typeMapper));

            _typeMapper = typeMapper;
        }
        
        /// <summary>
        /// When base type changed
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="previous"></param>
        /// <returns></returns>
        public bool Apply([NotNull] CypherInternalEntityBuilder builder, [CanBeNull] CypherEntity previous)
            => Apply(builder) != null;

        /// <summary>
        /// When entity added
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public virtual CypherInternalEntityBuilder Apply([NotNull] CypherInternalEntityBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));
            var entity = builder.Metadata;

            if (entity.HasClrType()) {
                var primatives = entity
                    .ClrType
                    .GetRuntimeProperties()
                    .Where(IsCandidatePrimitiveProperty);

                foreach (var primative in primatives) {
                    builder.Property(primative, ConfigurationSource.Convention);
                }
            }

            return builder;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        protected virtual bool IsCandidatePrimitiveProperty([NotNull] PropertyInfo propertyInfo)
        {
            Check.NotNull(propertyInfo, nameof(propertyInfo));

            return propertyInfo.IsCandidateProperty()
                   && _typeMapper.IsTypeMapped(propertyInfo.PropertyType);
        }
    }
}