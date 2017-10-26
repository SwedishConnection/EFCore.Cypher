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
    public class EntityBuilder : IInfrastructure<IMutableModel>, IInfrastructure<InternalEntityBuilder>
    {
        private InternalEntityBuilder Builder { get; }

        public EntityBuilder(InternalEntityBuilder builder) {
            Check.NotNull(builder, nameof(builder));

            Builder = builder;
        }

        /// <summary>
        /// Internal builder being used to configure the entity
        /// </summary>
        InternalEntityBuilder IInfrastructure<InternalEntityBuilder>.Instance => Builder;

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
        public virtual EntityBuilder HasBaseType([CanBeNull] string name)
            => new EntityBuilder(Builder.HasBaseType(name, ConfigurationSource.Explicit));

        /// <summary>
        /// Set the base node of this entity (inheritance hierarchy)
        /// </summary>
        /// <param name="clrType"></param>
        /// <returns></returns>
        public virtual EntityBuilder HasBaseType([CanBeNull] Type clrType)
            => new EntityBuilder(Builder.HasBaseType(clrType, ConfigurationSource.Explicit));

    }

    /// <summary>
    /// Entity builder (<see cref="EntityTypeBuilder" />)
    /// </summary>
    public class EntityBuilder<TEntity>: EntityBuilder where TEntity: class {
        public EntityBuilder([NotNull] InternalEntityBuilder builder): base(builder) {
        }

    }
}