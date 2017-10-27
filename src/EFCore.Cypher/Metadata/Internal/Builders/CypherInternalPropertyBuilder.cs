// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    public class CypherInternalPropertyBuilder: CypherInternalMetadataItemBuilder<CypherProperty> {

        public CypherInternalPropertyBuilder([NotNull] CypherProperty property, [NotNull] CypherInternalGraphBuilder builder)
            : base(property, builder) {   
        }

        /// <summary>
        /// Associate property with correct configuration source
        /// </summary>
        /// <param name="entityBuilder"></param>
        /// <param name="configurationSource"></param>
        /// <returns></returns>
        public virtual CypherInternalPropertyBuilder Attach(
            [NotNull] CypherInternalEntityBuilder entityBuilder, ConfigurationSource configurationSource)
        {
            var typeConfigurationSource = Metadata.GetTypeConfigurationSource();
            var me = entityBuilder.Metadata.FindProperty(Metadata.Name);

            CypherInternalPropertyBuilder builder;

            if (!(me is null) && 
                (me.GetConfigurationSource().Overrides(configurationSource) || 
                me.GetTypeConfigurationSource().Overrides(typeConfigurationSource) ||
                (Metadata.ClrType == me.ClrType && Metadata.PropertyInfo?.Name == me.PropertyInfo?.Name))) {
                    builder = me.Builder;
                    me.UpdateConfigurationSource(configurationSource);

                    if (typeConfigurationSource.HasValue) {
                        me.UpdateTypeConfigurationSource(typeConfigurationSource.Value);
                    }
            } else {
                builder = Metadata.PropertyInfo == null
                    ? entityBuilder.Property(Metadata.Name, Metadata.ClrType, configurationSource, Metadata.GetTypeConfigurationSource())
                    : entityBuilder.Property(Metadata.PropertyInfo, configurationSource);
            }

            if (me == Metadata) {
                return builder;
            }

            // TODO: Before/After/Nullable/ConcurrencyToken/ValueGenerated/FieldInfo

            return builder;
        }
    }
}