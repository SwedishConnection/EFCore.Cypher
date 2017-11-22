// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;
using Remotion.Linq.Clauses;

namespace Microsoft.EntityFrameworkCore.Query.ExpressionVisitors.Internal
{
    public class CypherMaterializerFactory : ICypherMaterializerFactory
    {
        private readonly IEntityMaterializerSource _entityMaterializerSource;

        public CypherMaterializerFactory(
            [NotNull] IEntityMaterializerSource entityMaterializerSource
        ) {
            Check.NotNull(entityMaterializerSource, nameof(entityMaterializerSource));
            
            _entityMaterializerSource = entityMaterializerSource;
        }

        /// <summary>
        /// Uses the read only expression to materialize properties into 
        /// value buffers
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="readOnlyExpression"></param>
        /// <param name="Func<IProperty"></param>
        /// <param name="returnItemHandler"></param>
        /// <param name="querySource"></param>
        /// <param name="Dictionary<Type"></param>
        /// <param name="typeIndexMapping"></param>
        /// <returns></returns>
        public Expression<Func<ValueBuffer, object>> CreateMaterializer(
            [NotNull] IEntityType entityType, 
            [NotNull] ReadOnlyExpression readOnlyExpression, 
            [NotNull] Func<IProperty, ReadOnlyExpression, int> returnItemHandler, 
            [CanBeNull] IQuerySource querySource, 
            out Dictionary<Type, int[]> typeIndexMapping
        ) {
            Check.NotNull(entityType, nameof(entityType));
            Check.NotNull(readOnlyExpression, nameof(readOnlyExpression));
            Check.NotNull(returnItemHandler, nameof(returnItemHandler));

            typeIndexMapping = null;

            ParameterExpression valueBufferParameter = Expression
                .Parameter(typeof(ValueBuffer), "valueBuffer" );

            var concreteEntityTypes = entityType
                .GetConcreteTypesInHierarchy()
                .ToList();

            // grab first concrete entity type filling the index mapping with return item indexes
            int[] indexMapping = new int[concreteEntityTypes.First().PropertyCount()];
            int propertyIndex = 0;

            // TODO: Review exclusion of shadow properties
            foreach (var property in concreteEntityTypes.First().GetProperties().Where(p => !p.IsShadowProperty)) {
                indexMapping[propertyIndex++] = returnItemHandler(property, readOnlyExpression);
            }

            // materializer
            Expression materializer = _entityMaterializerSource
                .CreateMaterializeExpression(
                    concreteEntityTypes.First(),
                    valueBufferParameter,
                    indexMapping
                );

            // when single concrete entity type that is the root return just the value buffer to object fn
            if (concreteEntityTypes.Count == 1 
                && concreteEntityTypes.First().RootType() == concreteEntityTypes.First()) {
                return Expression.Lambda<Func<ValueBuffer, object>>(
                    materializer,
                    valueBufferParameter
                );
            }

            // TODO: Handle discrimination?

            throw new NotImplementedException();
        }
    }
}