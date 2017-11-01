// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
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
        /// Labels
        /// </summary>
        /// <returns></returns>
        public virtual string[] Labels {
            get => new string[] {};

            set => SetLabels(value);
        }

        /// <summary>
        /// Set labels
        /// </summary>
        /// <param name="labels"></param>
        /// <returns></returns>
        protected virtual bool SetLabels([NotNull] string[] labels) 
            => Annotations.SetAnnotation(
                CypherAnnotationNames.Labels,
                Check.NotEmpty(labels, nameof(labels))
            );
    }
}