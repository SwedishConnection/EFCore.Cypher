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
    public class GraphBuilder: IInfrastructure<InternalGraphBuilder> {
        private readonly InternalGraphBuilder _builder;

        /// <summary>
        /// With set of graph conventions
        /// </summary>
        /// <param name="conventions"></param>
        public GraphBuilder([NotNull] GraphConventionSet conventions) {
            // TODO: Check conventions not null

            _builder = new InternalGraphBuilder(new Graph(conventions));
        }

        /// <summary>
        /// Graph being configured
        /// </summary>
        public virtual IMutableGraph Graph => Builder.Metadata;

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


        private InternalGraphBuilder Builder => this.GetInfrastructure();
    }
}