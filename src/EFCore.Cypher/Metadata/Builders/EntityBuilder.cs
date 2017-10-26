using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Microsoft.EntityFrameworkCore.Metadata.Builders
{
    public class EntityBuilder : IInfrastructure<IMutableModel>, IInfrastructure<InternalEntityBuilder>
    {
        private InternalEntityBuilder Builder { get; }

        public EntityBuilder(InternalEntityBuilder builder) {
            // TODO: Check builder not null

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
    }

    public class EntityBuilder<TEntity>: EntityBuilder where TEntity: class {
        public EntityBuilder([NotNull] InternalEntityBuilder builder): base(builder) {
        }


    }
}