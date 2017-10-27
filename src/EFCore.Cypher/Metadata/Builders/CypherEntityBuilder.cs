// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Metadata.Builders
{
    /// <summary>
    /// Entity builer (<see cref="EntityTypeBuilder" />)
    /// </summary>
    public class CypherEntityBuilder : IInfrastructure<IMutableModel>, IInfrastructure<CypherInternalEntityBuilder>
    {
        private CypherInternalEntityBuilder Builder { get; }

        public CypherEntityBuilder(CypherInternalEntityBuilder builder) {
            Check.NotNull(builder, nameof(builder));

            Builder = builder;
        }

        /// <summary>
        /// Internal builder being used to configure the entity
        /// </summary>
        CypherInternalEntityBuilder IInfrastructure<CypherInternalEntityBuilder>.Instance => Builder;

        /// <summary>
        /// Entity being configured
        /// </summary>
        public virtual IMutableEntityType Metadata => Builder.Metadata;
        
        /// <summary>
        /// Graph that the entity belongs to
        /// </summary>
        IMutableModel IInfrastructure<IMutableModel>.Instance => Builder.GraphBuilder.Metadata;

        /// <summary>
        /// Set the base node of this entity (inheritance hierarchy)
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual CypherEntityBuilder HasBaseType([CanBeNull] string name)
            => new CypherEntityBuilder(Builder.HasBaseType(name, ConfigurationSource.Explicit));

        /// <summary>
        /// Set the base node of this entity (inheritance hierarchy)
        /// </summary>
        /// <param name="clrType"></param>
        /// <returns></returns>
        public virtual CypherEntityBuilder HasBaseType([CanBeNull] Type clrType)
            => new CypherEntityBuilder(Builder.HasBaseType(clrType, ConfigurationSource.Explicit));

        /// <summary>
        /// Has One
        /// </summary>
        /// <param name="clrType"></param>
        /// <param name="navigationName"></param>
        /// <returns></returns>
        public virtual CypherReferenceNavigationBuilder HasOne(
            [NotNull] Type clrType, 
            [CanBeNull] string navigationName = null
        ) {
            Check.NotNull(clrType, nameof(clrType));
            Check.NullButNotEmpty(navigationName, nameof(navigationName));

            CypherEntity relatedEntity = Builder.Metadata.FindInDefinitionPath(clrType) ??
                Builder.GraphBuilder.Entity(clrType, ConfigurationSource.Explicit).Metadata;

            return new CypherReferenceNavigationBuilder(
                Builder.Metadata,
                relatedEntity,
                navigationName,
                Builder.Navigation(
                    relatedEntity.Builder, 
                    navigationName, 
                    ConfigurationSource.Explicit,
                    setTargetAsPrincipal: Builder.Metadata == relatedEntity
                )
            );
        }

    }

    /// <summary>
    /// Entity builder (<see cref="EntityTypeBuilder" />)
    /// </summary>
    public class CypherEntityBuilder<TEntity>: CypherEntityBuilder where TEntity: class {
        public CypherEntityBuilder([NotNull] CypherInternalEntityBuilder builder)
            : base(builder) {
        }

        private CypherInternalEntityBuilder Builder => this.GetInfrastructure<CypherInternalEntityBuilder>();

        /// <summary>
        /// Set base type
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public new virtual CypherEntityBuilder<TEntity> HasBaseType([CanBeNull] string name)
            => new CypherEntityBuilder<TEntity>(Builder.HasBaseType(name, ConfigurationSource.Explicit));

        /// <summary>
        /// Set base type
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        public new virtual CypherEntityBuilder<TEntity> HasBaseType([CanBeNull] Type entityType)
            => new CypherEntityBuilder<TEntity>(Builder.HasBaseType(entityType, ConfigurationSource.Explicit));

        /// <summary>
        /// Set base type
        /// </summary>
        /// <returns></returns>
        public virtual CypherEntityBuilder<TEntity> HasBaseType<TBaseType>()
            => HasBaseType(typeof(TBaseType));
    }
}