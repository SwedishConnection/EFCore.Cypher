using JetBrains.Annotations;
using System;
using System.Linq;

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
            if (IsIgnoring(identity, configurationSource)) {
                return null;
            }

            Type clrType = identity.Type;
            Entity entity = clrType == null
                ? Metadata.FindEntity(identity.Labels)
                : Metadata.FindEntity(clrType);

            if (entity is null) {
                if (clrType is null) {
                    Metadata.NotIgnore(identity);

                    entity = Metadata.AddEntity(identity.Labels, configurationSource);
                } else {
                    Metadata.NotIgnore(identity);

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
            if (IsIgnoring(identity, configurationSource)) {
                return null;
            }

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clrType"></param>
        /// <param name="configurationSource"></param>
        /// <returns></returns>
        public virtual bool IsIgnoring(
            [NotNull] Type clrType, 
            ConfigurationSource configurationSource
        ) => IsIgnoring(new NodeIdentity(clrType), configurationSource);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="labels"></param>
        /// <param name="configurationSource"></param>
        /// <returns></returns>
        public virtual bool IsIgnoring(
            [NotNull] string[] labels, 
            ConfigurationSource configurationSource
        ) => IsIgnoring(new NodeIdentity(labels), configurationSource);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="configurationSource"></param>
        /// <returns></returns>
        internal bool IsIgnoring(NodeIdentity identity, ConfigurationSource configurationSource) {
            if (configurationSource == ConfigurationSource.Explicit)
            {
                return false;
            }

            var ignoredConfigurationSource = Metadata.FindIgnoredTypeConfigurationSource(identity);
            return ignoredConfigurationSource.HasValue &&
                ignoredConfigurationSource.Value.Overrides(configurationSource);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="configurationSource"></param>
        /// <returns></returns>
        public virtual bool Ignore([NotNull] Type type, ConfigurationSource configurationSource)
            => Ignore(new NodeIdentity(type), configurationSource);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="labels"></param>
        /// <param name="configurationSource"></param>
        /// <returns></returns>
        public virtual bool Ignore([NotNull] string[] labels, ConfigurationSource configurationSource)
            => Ignore(new NodeIdentity(labels), configurationSource);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="configurationSource"></param>
        /// <returns></returns>
        private bool Ignore([NotNull] NodeIdentity identity, ConfigurationSource configurationSource)
        {
            var ignoredConfigurationSource = Metadata.FindIgnoredTypeConfigurationSource(identity);
            if (ignoredConfigurationSource.HasValue)
            {
                if (configurationSource.Overrides(ignoredConfigurationSource) && 
                    configurationSource != ignoredConfigurationSource)
                {
                    Metadata.Ignore(identity, configurationSource);
                }

                return true;
            }

            var entity = Metadata.FindEntity(identity.Labels);
            if (entity == null)
            {
                Metadata.Ignore(identity, configurationSource);

                return true;
            }

            return Ignore(entity, configurationSource);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="configurationSource"></param>
        /// <returns></returns>
        private bool Ignore(Entity entity, ConfigurationSource configurationSource)
        {
            var entityConfigurationSource = entity.GetConfigurationSource();
            if (!configurationSource.Overrides(entityConfigurationSource))
            {
                return false;
            }

            using (Metadata.GraphConventionDispatcher.StartBatch())
            {
                if (entity.HasClrType())
                {
                    Metadata.Ignore(entity.ClrType, configurationSource);
                }
                else
                {
                    Metadata.Ignore(entity.Labels, configurationSource);
                }

                return RemoveEntity(entity, configurationSource);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="configurationSource"></param>
        /// <returns></returns>
        public virtual bool RemoveEntity(
            [NotNull] Entity entity, 
            ConfigurationSource configurationSource
        )
        {
            // bail if configuration can not override
            var entityConfigurationSource = entity.GetConfigurationSource();
            if (!configurationSource.Overrides(entityConfigurationSource))
            {
                return false;
            }

            // wipe out the base type association
            var baseType = entity.BaseType;
            entity.Builder.HasBaseType((Entity)null, configurationSource);

            // TODO: remove from relationships

            // hoist children to the base type
            foreach (var directlyDerivedType in entity.GetDirectlyDerivedTypes().ToList())
            {
                var derivedEntityTypeBuilder = directlyDerivedType
                    .Builder
                    .HasBaseType(baseType, configurationSource);
            }

            // when a defining type
            using (Metadata.GraphConventionDispatcher.StartBatch())
            {
                foreach (var definedType in Metadata.GetEntities().Where(e => e.DefiningType == entity).ToList())
                {
                    RemoveEntity(definedType, configurationSource);
                }

                Metadata.RemoveEntity(entity);
            }

            return true;
        }
    }
}