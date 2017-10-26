
using System;
using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    public class InternalEntityBuilder: InternalMetadataItemBuilder<Entity> {

        public InternalEntityBuilder(
            [NotNull] Entity metadata, 
            [NotNull] InternalGraphBuilder graphBuilder
        ) : base(metadata, graphBuilder) {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseClrType"></param>
        /// <param name="configurationSource"></param>
        /// <returns></returns>
        public virtual InternalEntityBuilder HasBaseType(
            [CanBeNull] Type baseClrType, 
            ConfigurationSource configurationSource
        ) {
            if (baseClrType == null) {
                return HasBaseType((Entity)null, configurationSource);
            }

            var builder = GraphBuilder.Entity(baseClrType, configurationSource);
            return builder is null
                ? null
                : HasBaseType(builder.Metadata, configurationSource);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="configurationSource"></param>
        /// <returns></returns>
        public virtual InternalEntityBuilder HasBaseType(
            [CanBeNull] string name,
            ConfigurationSource configurationSource
        ) {
            if (name is null) {
                return HasBaseType((Entity)null, configurationSource);
            }

            var builder = GraphBuilder.Entity(name, configurationSource);
            return builder is null
                ? null 
                : HasBaseType(builder.Metadata, configurationSource);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseType"></param>
        /// <param name="configurationSource"></param>
        /// <returns></returns>
        public virtual InternalEntityBuilder HasBaseType([CanBeNull] Entity baseType, ConfigurationSource configurationSource) {
            if (Metadata.BaseType == baseType) {
                Metadata.HasBaseType(baseType, configurationSource);
                return this;
            }

            if (!configurationSource.Overrides(Metadata.GetBaseTypeConfigurationSource())) {
                return null;
            }

            using (Metadata.Graph.GraphConventionDispatcher.StartBatch()) {
                // TODO: Everything
            }

            Metadata.HasBaseType(baseType, configurationSource);

            // TODO: Cleanup from dispatcher

            return this;
        }
    }
}