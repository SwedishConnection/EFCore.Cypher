
using System;
using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    public class InternalEntityBuilder: InternalNodeBuilder {

        public InternalEntityBuilder(
            [NotNull] Entity baze, 
            [NotNull] InternalGraphBuilder graphBuilder
        ) : base(baze, graphBuilder) {
        }

        public new virtual Entity Base { get { return (Entity)base.Base; } }

        /// <summary>
        /// Labels are used by the Graph Builder to create a base entity while 
        /// null labels are deferred to this method which takes an Entity
        /// </summary>
        /// <param name="labels"></param>
        /// <param name="configurationSource"></param>
        /// <returns></returns>
        public virtual InternalEntityBuilder HasBaseNode([CanBeNull] string[] labels, ConfigurationSource configurationSource) {
            if (labels is null) {
                return HasBaseNode((Entity)null, configurationSource);
            }

            InternalEntityBuilder builder = GraphBuilder.Entity(labels, configurationSource);
            return builder is null
                ? null
                : HasBaseNode(builder.Base, configurationSource);
        }

        /// <summary>
        /// When the constructor's base is the same as the passed entity then set the base
        /// to the passed otherwise bail when overriding is not possible and when possible 
        /// batch ?? to the graph dispatcher
        /// </summary>
        /// <param name="baseNode"></param>
        /// <param name="configurationSource"></param>
        /// <returns></returns>
        public virtual InternalEntityBuilder HasBaseNode([CanBeNull] Entity baseNode, ConfigurationSource configurationSource) {
            if (Base.BaseNode == baseNode) {
                Base.HasBaseNode(baseNode, configurationSource);
                return this;
            }

            if (!configurationSource.Overrides(Base.GetBaseNodeConfigurationSource())) {
                return null;
            }

            using (Base.Graph.GraphConventionDispatcher.StartBatch()) {
                // TODO: Snapshots of properties, relationships, constraints

                // TODO: Merge from base
            }

            return null;
        }
    }
}