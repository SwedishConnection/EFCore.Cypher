// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.EntityFrameworkCore.Infrastructure
{
    public class EntityFrameworkCypherServicesBuilder : EntityFrameworkServicesBuilder
    {
        public EntityFrameworkCypherServicesBuilder(
            [NotNull] IServiceCollection serviceCollection
        ) : base(serviceCollection)
        {
        }

        /// <summary>
        /// Cypher service characteristics
        /// </summary>
        public static readonly IDictionary<Type, ServiceCharacteristics> CypherServices
            = new Dictionary<Type, ServiceCharacteristics>
            {
            };

        /// <summary>
        /// Grab a specific service characteristics
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        protected override ServiceCharacteristics GetServiceCharacteristics(Type serviceType)
        {
            return CypherServices.TryGetValue(serviceType, out var characteristics)
                ? characteristics
                : base.GetServiceCharacteristics(serviceType);
        }

        public override EntityFrameworkServicesBuilder TryAddCoreServices()
        {
            // TODO: Register services

            return base.TryAddCoreServices();
        }
    }
}