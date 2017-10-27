// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    /// <summary>
    /// Internal entity builder
    /// </summary>
    public class CypherInternalEntityBuilder: CypherInternalMetadataItemBuilder<CypherEntity> {

        public CypherInternalEntityBuilder(
            [NotNull] CypherEntity metadata, 
            [NotNull] CypherInternalGraphBuilder graphBuilder
        ) : base(metadata, graphBuilder) {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseClrType"></param>
        /// <param name="configurationSource"></param>
        /// <returns></returns>
        public virtual CypherInternalEntityBuilder HasBaseType(
            [CanBeNull] Type baseClrType, 
            ConfigurationSource configurationSource
        ) {
            if (baseClrType == null) {
                return HasBaseType((CypherEntity)null, configurationSource);
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
        public virtual CypherInternalEntityBuilder HasBaseType(
            [CanBeNull] string name,
            ConfigurationSource configurationSource
        ) {
            if (name is null) {
                return HasBaseType((CypherEntity)null, configurationSource);
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
        public virtual CypherInternalEntityBuilder HasBaseType([CanBeNull] CypherEntity baseType, ConfigurationSource configurationSource) {
            if (Metadata.BaseType == baseType) {
                Metadata.HasBaseType(baseType, configurationSource);
                return this;
            }

            if (!configurationSource.Overrides(Metadata.GetBaseTypeConfigurationSource())) {
                return null;
            }

            using (Metadata.Graph.CypherConventionDispatcher.StartBatch()) {
                CypherPropertyBuildersSnapshot detachedProperties = null;
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
                            Metadata.NotIgnore(ignoredMember);
                        }
                    }

                    baseType.UpdateConfigurationSource(configurationSource);
                }

                // TODO: Relationships
            }

            return this;
        }

        /// <summary>
        /// Define property
        /// </summary>
        /// <param name="name"></param>
        /// <param name="clrType"></param>
        /// <param name="configurationSource"></param>
        /// <returns></returns>
        public virtual CypherInternalPropertyBuilder Property(
            [NotNull] string name,
            [NotNull] Type clrType,
            ConfigurationSource configurationSource
        ) => Property(name, clrType, configurationSource, typeConfigurationSource: configurationSource);

        /// <summary>
        /// Define property
        /// </summary>
        /// <param name="name"></param>
        /// <param name="clrType"></param>
        /// <param name="configurationSource"></param>
        /// <param name="typeConfigurationSource"></param>
        /// <returns></returns>
        public virtual CypherInternalPropertyBuilder Property(
            [NotNull] string name,
            [NotNull] Type clrType,
            ConfigurationSource configurationSource,
            [CanBeNull] ConfigurationSource? typeConfigurationSource
        ) => Property(name, clrType, memberInfo: null, configurationSource: configurationSource, typeConfigurationSource: configurationSource);

        /// <summary>
        /// Define property
        /// </summary>
        /// <param name="name"></param>
        /// <param name="configurationSource"></param>
        /// <returns></returns>
        public virtual CypherInternalPropertyBuilder Property(
            [NotNull] string name,
            ConfigurationSource configurationSource
        ) => Property(name, clrType: null, memberInfo: null, configurationSource: configurationSource, typeConfigurationSource: configurationSource);

        /// <summary>
        /// Define property
        /// </summary>
        /// <param name="clrProperty"></param>
        /// <param name="configurationSource"></param>
        /// <returns></returns>
        public virtual CypherInternalPropertyBuilder Property(
            [NotNull] MemberInfo clrProperty, 
            ConfigurationSource configurationSource
        ) => Property(clrProperty.Name, clrProperty.GetMemberType(), clrProperty, configurationSource, configurationSource);


        /// <summary>
        /// Define property
        /// </summary>
        /// <param name="name"></param>
        /// <param name="clrType"></param>
        /// <param name="memberInfo"></param>
        /// <param name="configurationSource"></param>
        /// <param name="typeConfigurationSource"></param>
        /// <returns></returns>
        private CypherInternalPropertyBuilder Property(
            [NotNull] string name,
            [NotNull] Type clrType,
            [CanBeNull] MemberInfo memberInfo,
            ConfigurationSource configurationSource,
            [CanBeNull] ConfigurationSource? typeConfigurationSource
        ) {
            if (IsIgnoring(name, configurationSource)) {
                return null;
            }

            Metadata.NotIgnore(name);

            IEnumerable<CypherProperty> detachables = null;

            var existing = Metadata.FindProperty(name);
            if (!(existing is null)) {
                if (existing.DeclaringEntityType != Metadata) {
                    if (!(memberInfo is null) && existing.MemberInfo is null) {
                        detachables = new[] { existing };
                    } else {
                        return existing
                            .DeclaringEntityType
                            .Builder
                            .Property(existing, name, clrType, memberInfo, configurationSource, typeConfigurationSource);
                    }
                }
            } else {
                detachables = Metadata.FindDerivedProperties(name);
            }

            CypherInternalPropertyBuilder builder;
            using (Metadata.Graph.CypherConventionDispatcher.StartBatch()) {
                var snapshot = detachables == null
                    ? null 
                    : DetachProperties(detachables);

                builder = Property(existing, name, clrType, memberInfo, configurationSource, typeConfigurationSource);
                snapshot?.Attach(this);
            }

            if (!(builder is null) && builder.Metadata.Builder == null) {
                return Metadata.FindProperty(name)?.Builder;
            }

            return builder;
        }

        /// <summary>
        /// Define property
        /// </summary>
        /// <param name="property"></param>
        /// <param name="name"></param>
        /// <param name="clrType"></param>
        /// <param name="memberInfo"></param>
        /// <param name="configurationSource"></param>
        /// <param name="typeConfigurationSource"></param>
        /// <returns></returns>
        private CypherInternalPropertyBuilder Property(
            [CanBeNull] CypherProperty property,
            [NotNull] string name,
            [NotNull] Type clrType,
            [CanBeNull] MemberInfo memberInfo,
            [CanBeNull] ConfigurationSource? configurationSource,
            [CanBeNull] ConfigurationSource? typeConfigurationSource
        ) {
            var existing = property;

            if (existing is null) {
                if (!configurationSource.HasValue) {
                    return null;
                }

                var duplicate = Metadata.FindNavigationsInHierarchy(name).FirstOrDefault();
                if (!(duplicate is null)) {
                    throw new InvalidOperationException(CoreStrings.PropertyCalledOnNavigation(name, Metadata.DisplayName()));
                }

                existing = clrType != null
                    ? Metadata.AddProperty(memberInfo, configurationSource.Value)
                    : Metadata.AddProperty(name, clrType, configurationSource.Value, typeConfigurationSource);
            } else {
                // when passed Clr not equal to passed property's Clr or when passed member info isn't null but the passed property's info is null
                if ((!(clrType is null) && clrType != property.ClrType) || 
                    (!(memberInfo is null) && property.PropertyInfo is null)) {
                    if (!configurationSource.HasValue || !configurationSource.Value.Overrides(property.GetConfigurationSource())) {
                        return null;
                    }

                    using (Metadata.Graph.CypherConventionDispatcher.StartBatch()) {
                        var snapshot = DetachProperties(new[] { property });

                        existing = memberInfo != null
                            ? Metadata.AddProperty(memberInfo, configurationSource.Value)
                            : Metadata.AddProperty(name, clrType, configurationSource.Value, typeConfigurationSource);

                        snapshot.Attach(this);
                    }
                } else {
                    if (configurationSource.HasValue) {
                        existing.UpdateConfigurationSource(configurationSource.Value);
                    }

                    if (typeConfigurationSource.HasValue) {
                        existing.UpdateTypeConfigurationSource(typeConfigurationSource.Value);
                    }
                }
            }

            return existing?.Builder;
        }

        /// <summary>
        /// Ignoring member (property/field)
        /// </summary>
        /// <param name="name"></param>
        /// <param name="configurationSource"></param>
        /// <returns></returns>
        public virtual bool IsIgnoring([NotNull] string name, ConfigurationSource? configurationSource) {
            Check.NotEmpty(name, nameof(name));

            var ignoredConfigurationSource = Metadata.FindIgnoredMemberConfigurationSource(name);
            if (!configurationSource.HasValue
                || !configurationSource.Value.Overrides(ignoredConfigurationSource))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="configurationSource"></param>
        /// <returns></returns>
        public virtual bool Ignore([NotNull] string name, ConfigurationSource configurationSource) {
            var ignoredConfigurationSource = Metadata.FindIgnoredMemberConfigurationSource(name);
            if (ignoredConfigurationSource.HasValue
                && ignoredConfigurationSource.Value.Overrides(configurationSource))
            {
                return true;
            }

            using (Metadata.Graph.CypherConventionDispatcher.StartBatch())
            {
                Metadata.Ignore(name, configurationSource);

                var navigation = Metadata.FindNavigation(name);
                if (!(navigation is null)) {
                    if (navigation.DeclaringEntityType != Metadata) {
                        if (configurationSource == ConfigurationSource.Explicit) {
                            throw new InvalidOperationException(
                                CoreStrings.InheritedPropertyCannotBeIgnored(
                                    name, 
                                    Metadata.DisplayName(), 
                                    navigation.DeclaringEntityType.DisplayName()
                                )
                            );
                        }

                        return false;
                    }
                } else {
                    var property = Metadata.FindProperty(name);
                    if (!(property is null)) {
                        if (property.DeclaringEntityType != Metadata) {
                            if (configurationSource == ConfigurationSource.Explicit) {
                                throw new InvalidOperationException(
                                    CoreStrings.InheritedPropertyCannotBeIgnored(
                                        name, 
                                        Metadata.DisplayName(), 
                                        property.DeclaringEntityType.DisplayName()
                                    )
                                );
                            }

                            return false;
                        }

                        if (property.DeclaringEntityType.Builder.RemoveProperty(
                            property,
                            configurationSource,
                            canOverrideSameSource: configurationSource == ConfigurationSource.Explicit
                        ) is null) {
                            Metadata.NotIgnore(name);
                            return false;
                        }
                    }
                }

                foreach (var derivedType in Metadata.GetDerivedTypes()) {
                    var derivedNavigation = derivedType.FindDeclaredNavigation(name);
                    if (derivedNavigation is null) {
                        var derivedProperty = derivedType.FindDeclaredProperty(name);
                        if (!(derivedProperty is null)) {
                            derivedType.Builder.RemoveProperty(derivedProperty, configurationSource, canOverrideSameSource: false);
                        }
                    }

                    var derivedIgnoredSource = derivedType.FindDeclaredIgnoredMemberConfigurationSource(name);
                    if (derivedIgnoredSource.HasValue
                        && configurationSource.Overrides(derivedIgnoredSource))
                    {
                        derivedType.NotIgnore(name);
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Remove property
        /// </summary>
        /// <param name="property"></param>
        /// <param name="configurationSource"></param>
        /// <param name="canOverrideSameSource"></param>
        /// <returns></returns>
        private ConfigurationSource? RemoveProperty(
            CypherProperty property,
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
        private static CypherPropertyBuildersSnapshot DetachProperties(IEnumerable<CypherProperty> propertiesToDetach)
        {
            var detachables = propertiesToDetach.ToList();
            if (detachables.Count == 0) {
                return null;
            }

            // TODO: Relationships

            // TODO: Indexes

            // TODO: Keys

            var detachedProperties = new List<Tuple<CypherInternalPropertyBuilder, ConfigurationSource>>();
            foreach (var detach in detachables) {
                var property = detach.DeclaringEntityType.FindDeclaredProperty(detach.Name);
                if (!(property is null)) {

                }
            }

            return new CypherPropertyBuildersSnapshot(detachedProperties);
        }
    }
}