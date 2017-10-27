// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    /// Graph builder similar to Entity Framework's model builder
    /// </summary>
    public class GraphBuilder: IInfrastructure<CypherInternalGraphBuilder> {
        private readonly CypherInternalGraphBuilder _builder;

        /// <summary>
        /// With graph conventions
        /// </summary>
        /// <param name="conventions"></param>
        public GraphBuilder([NotNull] CypherConventionSet conventions) {
            Check.NotNull(conventions, nameof(conventions));

            _builder = new CypherInternalGraphBuilder(new CypherGraph(conventions));
        }

        /// <summary>
        /// Graph being configured
        /// </summary>
        public virtual IMutableModel Graph => Builder.Metadata;

        /// <summary>
        /// Internal builder being used to configure the graph
        /// </summary>
        CypherInternalGraphBuilder IInfrastructure<CypherInternalGraphBuilder>.Instance => _builder;

        /// <summary>
        /// Returns a builder used to configure the given entity in the graph
        /// </summary>
        /// <returns></returns>
        public virtual CypherEntityBuilder<TEntity> Entity<TEntity>() 
            where TEntity: class =>
            new CypherEntityBuilder<TEntity>(Builder.Entity(typeof(TEntity), ConfigurationSource.Explicit));

        /// <summary>
        /// Returns a builder used to configure the given entity in the graph
        /// </summary>
        /// <param name="clrType"></param>
        /// <returns></returns>
        public virtual CypherEntityBuilder Entity([NotNull] Type clrType) {
            Check.NotNull(clrType, nameof(clrType));

            return new CypherEntityBuilder(Builder.Entity(clrType, ConfigurationSource.Explicit));
        }

        /// <summary>
        /// Ignore entity by type
        /// </summary>
        /// <returns></returns>
        public virtual GraphBuilder Ignore<TEntity>() where TEntity : class
            => Ignore(typeof(TEntity));

        /// <summary>
        /// Ignore entity by type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual GraphBuilder Ignore([NotNull] Type clrType)
        {
            Check.NotNull(clrType, nameof(clrType));

            Builder.Ignore(clrType, ConfigurationSource.Explicit);
            return this;
        }

        /// <summary>
        /// Builder
        /// </summary>
        /// <returns></returns>
        private CypherInternalGraphBuilder Builder => this.GetInfrastructure();
    }
}