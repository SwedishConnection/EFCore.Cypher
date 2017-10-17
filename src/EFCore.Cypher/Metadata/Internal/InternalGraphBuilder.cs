using JetBrains.Annotations;
using System;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    public class InternalGraphBuilder: InternalMetadataBuilder<Graph> {
        
        public InternalGraphBuilder([NotNull] Graph metadata): base(metadata) {

        }

        /// <summary>
        /// Graph Builder
        /// </summary>
        public override InternalGraphBuilder GraphBuilder => this;

        /// <summary>
        /// Entity builder from labels
        /// </summary>
        /// <param name="labels"></param>
        /// <param name="configurationSource"></param>
        /// <returns></returns>
        public virtual InternalEntityBuilder Entity(
            [NotNull] string[] labels, 
            ConfigurationSource configurationSource
        ) => Entity(new NodeIdentity(labels), configurationSource);

        /// <summary>
        /// Entity builder from CLR type
        /// </summary>
        /// <param name="clrType"></param>
        /// <param name="configurationSource"></param>
        /// <returns></returns>
        public virtual InternalEntityBuilder Entity(
            [NotNull] Type clrType, 
            ConfigurationSource configurationSource
        ) => Entity(new NodeIdentity(clrType), configurationSource);

        /// <summary>
        /// Create entity returning an entity builder
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="configurationSource"></param>
        /// <returns></returns>
        private InternalEntityBuilder Entity(
            NodeIdentity identity, 
            ConfigurationSource configurationSource
        ) {
            // TODO: Ignore check

            Type clrType = identity.Type;
            Entity entity = clrType == null
                ? Metadata.FindEntity(identity.Labels)
                : Metadata.FindEntity(clrType);

            if (entity is null) {
                if (clrType is null) {
                    // TODO: Unignore by labels

                    entity = Metadata.AddEntity(identity.Labels, configurationSource);
                } else {
                    // TODO: Unignore the CLR type

                    entity = Metadata.AddEntity(clrType, configurationSource);
                }
            } else {
                entity.UpdateConfigurationSource(configurationSource);
            }

            return entity?.Builder;
        }

        /// <summary>
        /// Entity builder from a defining property
        /// </summary>
        /// <param name="type"></param>
        /// <param name="definingNavigationName"></param>
        /// <param name="definingEntityType"></param>
        /// <param name="configurationSource"></param>
        /// <returns></returns>
        public InternalEntityBuilder Entity(
            [NotNull] Type type,
            [NotNull] string definingNavigationName,
            [NotNull] Entity definingEntityType,
            ConfigurationSource configurationSource
        ) => Entity(new NodeIdentity(type), definingNavigationName, definingEntityType, configurationSource);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="definingNavigationName"></param>
        /// <param name="definingEntityType"></param>
        /// <param name="configurationSource"></param>
        /// <returns></returns>
        private InternalEntityBuilder Entity(
            NodeIdentity identity,
            string definingNavigationName,
            Entity definingEntityType,
            ConfigurationSource configurationSource
        ) {
            // TODO: Ignore check

            Type clrType = identity.Type;
            Entity entity = clrType == null
                ? Metadata.FindEntity(identity.Labels) 
                : Metadata.FindEntity(clrType);

            if (!(entity is null)) {
                if (!configurationSource.Overrides(entity.GetConfigurationSource())) {
                    return null;
                }

                if (entity.GetConfigurationSource() != ConfigurationSource.Explicit) {
                    // TODO: Ignore
                }
            }

            if (clrType is null) {
                // TODO: Unignore by labels

                entity = Metadata.AddEntity(identity.Labels, definingNavigationName, definingEntityType, configurationSource);
            } else {
                // TODO: Unignore by type

                entity = Metadata.AddEntity(clrType, definingNavigationName, definingEntityType, configurationSource);
            }
            
            return entity?.Builder;
        }
    }
}