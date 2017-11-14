// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.EntityFrameworkCore.Update.Internal;
using Microsoft.Extensions.DependencyInjection;
using Remotion.Linq.Parsing.ExpressionVisitors.TreeEvaluation;

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
                { typeof(IRelationalTypeMapper), new ServiceCharacteristics(ServiceLifetime.Singleton) },

                { typeof(ISqlGenerationHelper), new ServiceCharacteristics(ServiceLifetime.Singleton) },
                { typeof(IRawSqlCommandBuilder), new ServiceCharacteristics(ServiceLifetime.Singleton) },
                { typeof(IRelationalCommandBuilderFactory), new ServiceCharacteristics(ServiceLifetime.Singleton) },
                { typeof(IParameterNameGeneratorFactory), new ServiceCharacteristics(ServiceLifetime.Singleton) },
                { typeof(IRelationalConnection), new ServiceCharacteristics(ServiceLifetime.Scoped) },
                { typeof(IRelationalDatabaseCreator), new ServiceCharacteristics(ServiceLifetime.Scoped) },

                { typeof(ICypherResultOperatorHandler), new ServiceCharacteristics(ServiceLifetime.Scoped) },

                { typeof(IUpdateSqlGenerator), new ServiceCharacteristics(ServiceLifetime.Singleton) },
                { typeof(ICommandBatchPreparer), new ServiceCharacteristics(ServiceLifetime.Scoped) },
                { typeof(IModificationCommandBatchFactory), new ServiceCharacteristics(ServiceLifetime.Scoped) },
                { typeof(IComparer<ModificationCommand>), new ServiceCharacteristics(ServiceLifetime.Singleton) },
                { typeof(IKeyValueIndexFactorySource), new ServiceCharacteristics(ServiceLifetime.Singleton) },
                { typeof(IBatchExecutor), new ServiceCharacteristics(ServiceLifetime.Scoped) },
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override EntityFrameworkServicesBuilder TryAddCoreServices()
        {
            // TODO: Register services

            // Model
            TryAdd<IModelSource, RelationalModelSource>();  // RelationalModelSource just extends ModelSource
            TryAdd<IModelValidator, CypherModelValidator>();
            TryAdd<IModelCustomizer, RelationalModelCustomizer>();  // Relational specific logic in the FindSets method can be ignored (i.e. nothing to override)

            ServiceCollectionMap.GetInfrastructure()
                .AddDependencySingleton<RelationalTypeMapperDependencies>()
                .AddDependencySingleton<RelationalModelValidatorDependencies>()
                .AddDependencyScoped<RelationalConventionSetBuilderDependencies>();


            // Migrations


            // Despite the semantic discomfort with SQL prefixes most of the relational command/database services can be reused
            TryAdd<IDatabase, RelationalDatabase>();
            TryAdd<IRawSqlCommandBuilder, RawSqlCommandBuilder>();
            TryAdd<IRelationalCommandBuilderFactory, RelationalCommandBuilderFactory>();
            TryAdd<IParameterNameGeneratorFactory, ParameterNameGeneratorFactory>();

            ServiceCollectionMap.GetInfrastructure()
                .AddDependencyScoped<RelationalConnectionDependencies>()
                .AddDependencyScoped<RelationalDatabaseDependencies>()
                .AddDependencySingleton<RelationalSqlGenerationHelperDependencies>()
                .AddDependencySingleton<ParameterNameGeneratorDependencies>();


            // Query
            TryAdd<IQueryContextFactory, RelationalQueryContextFactory>();
            TryAdd<IQueryCompilationContextFactory, CypherQueryCompilationContextFactory>();
            TryAdd<IEntityQueryableExpressionVisitorFactory, CypherEntityQueryableExpressionVisitorFactory>();
            TryAdd<IEntityQueryModelVisitorFactory, CypherQueryModelVisitorFactory>();
            TryAdd<IEvaluatableExpressionFilter, RelationalEvaluatableExpressionFilter>();
            TryAdd<ICypherResultOperatorHandler, CypherResultOperatorHandler>();

            ServiceCollectionMap.GetInfrastructure()
                .AddDependencyScoped<RelationalQueryCompilationContextDependencies>()
                .AddDependencyScoped<CypherEntityQueryableExpressionVisitorDependencies>()
                .AddDependencyScoped<CypherQueryModelVisitorDependencies>()
                .AddDependencyScoped<RelationalQueryCompilationContextDependencies>();


            // Update
            TryAdd<ICommandBatchPreparer, CommandBatchPreparer>();
            TryAdd<IComparer<ModificationCommand>, ModificationCommandComparer>();
            TryAdd<IKeyValueIndexFactorySource, KeyValueIndexFactorySource>();
            TryAdd<IBatchExecutor, BatchExecutor>();


            // Relational transaction factory (post 2.0)

            return base.TryAddCoreServices();
        }
    }
}