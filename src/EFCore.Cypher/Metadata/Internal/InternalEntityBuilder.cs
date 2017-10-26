// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    /// <summary>
    /// Internal entity builder
    /// </summary>
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
        /// Set base type
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
        /// Set base type
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
                PropertyBuildersSnapshot detachedProperties = null;
                var configurationSourceForRemoval = ConfigurationSource.DataAnnotation.Max(configurationSource);

                // when base is not null
                if (!(baseType is null)) {
                    // TODO: keys

                    // TODO: Clean up relationships

                    // properties
                    var duplicatedProperties = baseType.GetProperties()
                        .SelectMany(p => Metadata.FindDerivedPropertiesInclusive(p.Name))
                        .Where(p => p != null);

                    detachedProperties = DetachProperties(duplicatedProperties);

                    var propertiesToRemove = Metadata
                        .GetDerivedTypesInclusive()
                        .SelectMany(et => et.GetDeclaredProperties())
                        .Where(p => !p.GetConfigurationSource().Overrides(baseType.FindIgnoredMemberConfigurationSource(p.Name)))
                        .ToList();

                    foreach (var property in propertiesToRemove)
                    {
                        property
                            .DeclaringEntityType
                            .Builder
                            .RemoveProperty(property, ConfigurationSource.Explicit);
                    }

                    foreach (var ignoredMember in Metadata.GetIgnoredMembers().ToList())
                    {
                        var ignoredSource = Metadata.FindDeclaredIgnoredMemberConfigurationSource(ignoredMember);
                        var baseIgnoredSource = baseType.FindIgnoredMemberConfigurationSource(ignoredMember);

                        if (baseIgnoredSource.HasValue
                            && baseIgnoredSource.Value.Overrides(ignoredSource))
                        {
                            Metadata.Unignore(ignoredMember);
                        }
                    }

                    baseType.UpdateConfigurationSource(configurationSource);
                }

                // TODO: Relationships
            }

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="property"></param>
        /// <param name="configurationSource"></param>
        /// <param name="canOverrideSameSource"></param>
        /// <returns></returns>
        private ConfigurationSource? RemoveProperty(
            Property property,
            ConfigurationSource configurationSource,
            bool canOverrideSameSource = true
        ) {
            var currentConfigurationSource = property.GetConfigurationSource();

            // TODO: Everything

            return currentConfigurationSource;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertiesToDetach"></param>
        /// <returns></returns>
        private static PropertyBuildersSnapshot DetachProperties(IEnumerable<Property> propertiesToDetach)
        {
            // TODO: Everything
            return null;
        }
    }
}