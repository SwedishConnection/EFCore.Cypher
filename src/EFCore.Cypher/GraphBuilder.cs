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
    public class GraphBuilder: IInfrastructure<InternalGraphBuilder> {
        private readonly InternalGraphBuilder _builder;

        /// <summary>
        /// With graph conventions
        /// </summary>
        /// <param name="conventions"></param>
        public GraphBuilder([NotNull] GraphConventionSet conventions) {
            Check.NotNull(conventions, nameof(conventions));

            _builder = new InternalGraphBuilder(new Graph(conventions));
        }

        /// <summary>
        /// Graph being configured
        /// </summary>
        public virtual IMutableModel Graph => Builder.Metadata;

        /// <summary>
        /// Internal builder being used to configure the graph
        /// </summary>
        InternalGraphBuilder IInfrastructure<InternalGraphBuilder>.Instance => _builder;

        /// <summary>
        /// Returns a builder used to configure the given entity in the graph
        /// </summary>
        /// <returns></returns>
        public virtual EntityBuilder<TEntity> Entity<TEntity>() 
            where TEntity: class =>
            new EntityBuilder<TEntity>(Builder.Entity(typeof(TEntity), ConfigurationSource.Explicit));

        /// <summary>
        /// Returns a builder used to configure the given entity in the graph
        /// </summary>
        /// <param name="clrType"></param>
        /// <returns></returns>
        public virtual EntityBuilder Entity([NotNull] Type clrType) {
            Check.NotNull(clrType, nameof(clrType));

            return new EntityBuilder(Builder.Entity(clrType, ConfigurationSource.Explicit));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual GraphBuilder Ignore<TEntity>() where TEntity : class
            => Ignore(typeof(TEntity));

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual GraphBuilder Ignore([NotNull] Type type)
        {
            Check.NotNull(type, nameof(type));

            Builder.Ignore(type, ConfigurationSource.Explicit);
            return this;
        }

        /// <summary>
        /// Builder
        /// </summary>
        /// <returns></returns>
        private InternalGraphBuilder Builder => this.GetInfrastructure();
    }
}