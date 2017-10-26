using JetBrains.Annotations;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore.Internal;

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
        /// Entity builder from name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="configurationSource"></param>
        /// <returns></returns>
        public virtual InternalEntityBuilder Entity(
            [NotNull] string name, 
            ConfigurationSource configurationSource
        ) => Entity(new TypeIdentity(name), configurationSource);

        /// <summary>
        /// Entity builder from CLR type
        /// </summary>
        /// <param name="clrType"></param>
        /// <param name="configurationSource"></param>
        /// <returns></returns>
        public virtual InternalEntityBuilder Entity(
            [NotNull] Type clrType, 
            ConfigurationSource configurationSource
        ) => Entity(new TypeIdentity(clrType), configurationSource);

        /// <summary>
        /// Create entity returning an entity builder
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="configurationSource"></param>
        /// <returns></returns>
        private InternalEntityBuilder Entity(
            TypeIdentity identity, 
            ConfigurationSource configurationSource
        ) {
            if (IsIgnoring(identity, configurationSource)) {
                return null;
            }

            Type clrType = identity.Type;
            var entity = clrType == null
                ? Metadata.FindEntity(identity.Name)
                : Metadata.FindEntity(clrType);

            if (entity is null) {
                if (clrType is null) {
                    Metadata.NotIngore(identity.Name);

                    entity = Metadata.AddEntity(identity.Name, configurationSource);
                } else {
                    Metadata.NotIngore(clrType);

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
            [NotNull] string name,
            [NotNull] string definingNavigationName,
            [NotNull] Entity definingEntityType,
            ConfigurationSource configurationSource
        ) => Entity(new TypeIdentity(name), definingNavigationName, definingEntityType, configurationSource);

        /// <summary>
        /// Entity builder from a defining property
        /// </summary>
        /// <param name="type"></param>
        /// <param name="definingNavigationName"></param>
        /// <param name="definingEntityType"></param>
        /// <param name="configurationSource"></param>
        /// <returns></returns>
        public InternalEntityBuilder Entity(
            [NotNull] Type clrType,
            [NotNull] string definingNavigationName,
            [NotNull] Entity definingEntityType,
            ConfigurationSource configurationSource
        ) => Entity(new TypeIdentity(clrType), definingNavigationName, definingEntityType, configurationSource);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="definingNavigationName"></param>
        /// <param name="definingEntityType"></param>
        /// <param name="configurationSource"></param>
        /// <returns></returns>
        private InternalEntityBuilder Entity(
            TypeIdentity identity,
            string definingNavigationName,
            Entity definingEntityType,
            ConfigurationSource configurationSource
        ) {
            if (IsIgnoring(identity, configurationSource)) {
                return null;
            }

            Type clrType = identity.Type;
            var entity = clrType == null
                ? Metadata.FindEntity(identity.Name) 
                : Metadata.FindEntity(clrType);

            if (!(entity is null)) {
                if (!configurationSource.Overrides(entity.GetConfigurationSource())) {
                    return null;
                }

                if (entity.GetConfigurationSource() != ConfigurationSource.Explicit) {
                    Ignore(entity, configurationSource);
                }
            }

            if (clrType is null) {
                Metadata.NotIngore(clrType.Name);

                entity = Metadata.AddEntity(identity.Name, definingNavigationName, definingEntityType, configurationSource);
            } else {
                Metadata.NotIngore(clrType);

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
        ) => IsIgnoring(new TypeIdentity(clrType), configurationSource);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="configurationSource"></param>
        /// <returns></returns>
        public virtual bool IsIgnoring(
            [NotNull] string name, 
            ConfigurationSource configurationSource
        ) => IsIgnoring(new TypeIdentity(name), configurationSource);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="configurationSource"></param>
        /// <returns></returns>
        internal bool IsIgnoring(TypeIdentity identity, ConfigurationSource configurationSource) {
            if (configurationSource == ConfigurationSource.Explicit)
            {
                return false;
            }

            var ignoredConfigurationSource = Metadata.FindIgnoredTypeConfigurationSource(identity.Name);
            return ignoredConfigurationSource.HasValue &&
                ignoredConfigurationSource.Value.Overrides(configurationSource);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="configurationSource"></param>
        /// <returns></returns>
        public virtual bool Ignore([NotNull] Type clrType, ConfigurationSource configurationSource)
            => Ignore(clrType.DisplayName(), clrType, configurationSource);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="configurationSource"></param>
        /// <returns></returns>
        public virtual bool Ignore([NotNull] string name, ConfigurationSource configurationSource)
            => Ignore(name, null, configurationSource);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="configurationSource"></param>
        /// <returns></returns>
        private bool Ignore([NotNull] string name, [CanBeNull] Type clrType, ConfigurationSource configurationSource)
        {
            var ignoredConfigurationSource = Metadata.FindIgnoredTypeConfigurationSource(name);
            if (ignoredConfigurationSource.HasValue)
            {
                if (configurationSource.Overrides(ignoredConfigurationSource) && 
                    configurationSource != ignoredConfigurationSource)
                {
                    Metadata.Ignore(name, configurationSource);
                }

                return true;
            }

            var entity = Metadata.FindEntity(name);
            if (entity == null)
            {
                if (!(clrType is null)) {
                    Metadata.Ignore(clrType, configurationSource);
                } else {
                    Metadata.Ignore(name, configurationSource);
                }

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
                    Metadata.Ignore(entity.Name, configurationSource);
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
                foreach (var definedType in Metadata.GetEntities().Where(e => e.DefiningEntityType == entity).ToList())
                {
                    RemoveEntity(definedType, configurationSource);
                }

                Metadata.RemoveEntity(entity.Name);
            }

            return true;
        }
    }
}