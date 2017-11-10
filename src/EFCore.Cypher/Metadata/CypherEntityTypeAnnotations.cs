// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Metadata
{
    public class CypherEntityTypeAnnotations: ICypherEntityTypeAnnotations {

        public CypherEntityTypeAnnotations(
            [NotNull] IEntityType entityType
        ) : this(new CypherAnnotations(entityType)) {
            
        }

        protected CypherEntityTypeAnnotations(
            [NotNull] CypherAnnotations annotations
        ) => Annotations = annotations;

        /// <summary>
        /// Cypher annotations 
        /// </summary>
        /// <returns></returns>
        protected virtual CypherAnnotations Annotations { get; }

        /// <summary>
        /// Wrapped entity type
        /// </summary>
        /// <returns></returns>
        protected virtual IEntityType EntityType => (IEntityType)Annotations.Metadata;

        protected virtual CypherModelAnnotations GetAnnotations([NotNull] IModel model)
            => new CypherModelAnnotations(model);

        /// <summary>
        /// Annotations on an entity type
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        protected virtual CypherEntityTypeAnnotations GetAnnotations([NotNull] IEntityType entityType)
            => new CypherEntityTypeAnnotations(entityType);

        /// <summary>
        /// When the base type exists then return the root's labels otherwise
        /// fetch from the annotations or the default labels
        /// </summary>
        /// <returns></returns>
        public virtual string[] Labels {
            get => !(EntityType.BaseType is null)
                ? GetAnnotations(EntityType.BaseType).Labels.Concat(GetLabels()).ToArray()
                : GetLabels();

            set => SetLabels(value);
        }

        /// <summary>
        /// Labels
        /// </summary>
        /// <returns></returns>
        private string[] GetLabels() =>
            ((string[])Annotations.Metadata[CypherAnnotationNames.Labels] ?? GetDefaultLabels());

        /// <summary>
        /// Default labels
        /// </summary>
        /// <remarks>Defining navigations are nuked by entity added conventions</remarks> 
        /// <returns></returns>
        private string[] GetDefaultLabels()
            => EntityType.HasDefiningNavigation()
                ? GetAnnotations(EntityType.DefiningEntityType)
                    .Labels
                    .Select(l => $"{l}_{EntityType.DefiningNavigationName}")
                    .ToArray()
                : new[] { EntityType.ShortName() };

        /// <summary>
        /// Set labels
        /// </summary>
        /// <param name="labels"></param>
        /// <returns></returns>
        protected virtual bool SetLabels([CanBeNull] string[] labels) 
            => Annotations.SetAnnotation(
                CypherAnnotationNames.Labels,
                Check.NullButNotEmpty(labels, nameof(labels))
            );
    }
}