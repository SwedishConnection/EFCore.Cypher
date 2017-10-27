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

    }

    /// <summary>
    /// Entity builder (<see cref="EntityTypeBuilder" />)
    /// </summary>
    public class CypherEntityBuilder<TEntity>: CypherEntityBuilder where TEntity: class {
        public CypherEntityBuilder([NotNull] CypherInternalEntityBuilder builder): base(builder) {
        }

    }
}