using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Microsoft.EntityFrameworkCore.Metadata.Builders
{
    public class EntityBuilder : IInfrastructure<IMutableGraph>, IInfrastructure<InternalEntityBuilder>
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
        public virtual IMutableEntity Base => Builder.Base;
        
        /// <summary>
        /// Graph that the entity belongs to
        /// </summary>
        IMutableGraph IInfrastructure<IMutableGraph>.Instance => Builder.GraphBuilder.Base;

        /// <summary>
        /// Set the base node of this entity (inheritance hierarchy)
        /// </summary>
        /// <param name="labels"></param>
        /// <returns></returns>
        public virtual EntityBuilder HasBaseNode([CanBeNull] string[] labels)
            => new EntityBuilder(Builder.HasBaseNode(labels, ConfigurationSource.Explicit));
    }

    public class EntityBuilder<TEntity>: EntityBuilder where TEntity: class {
        public EntityBuilder([NotNull] InternalEntityBuilder builder): base(builder) {
        }


    }
}