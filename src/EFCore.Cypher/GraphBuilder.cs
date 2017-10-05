using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Microsoft.EntityFrameworkCore
{
    public class GraphBuilder: IInfrastructure<InternalGraphBuilder> {
        private readonly InternalGraphBuilder _builder;

        public GraphBuilder([NotNull] GraphConventionSet conventions) {
            _builder = new InternalGraphBuilder(new Graph(conventions));
        }

        public virtual IMutableGraph Graph => Builder.Base;

        public virtual EntityBuilder<TEntity> Entity<TEntity>() where TEntity: class =>
            new EntityBuilder<TEntity>(Builder.Entity(typeof(TEntity), ConfigurationSource.Explicit));

        InternalGraphBuilder IInfrastructure<InternalGraphBuilder>.Instance => _builder;

        private InternalGraphBuilder Builder => this.GetInfrastructure();
    }
}