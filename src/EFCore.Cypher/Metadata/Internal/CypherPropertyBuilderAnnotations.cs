// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    public class CypherPropertyBuilderAnnotations: CypherPropertyAnnotations {

        /// <summary>
        /// Sematic differences with <see cref="RelationalPropertyBuilderAnnotations" /> 
        /// </summary>
        /// <param name="propertyBuilder"></param>
        /// <param name="configurationSource"></param>
        /// <returns></returns>
        public CypherPropertyBuilderAnnotations(
            [NotNull] InternalPropertyBuilder propertyBuilder,
            ConfigurationSource configurationSource
        ) : base(new CypherAnnotationsBuilder(propertyBuilder, configurationSource)) {
        }

        /// <summary>
        /// Annotations builder
        /// </summary>
        /// <returns></returns>
        protected new virtual CypherAnnotationsBuilder Annotations => (CypherAnnotationsBuilder)base.Annotations;

        /// <summary>
        /// 
        /// </summary>
        protected override bool ShouldThrowOnConflict => false;

        /// <summary>
        /// Set storage name
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool HasStorageName([CanBeNull] string value) => SetStorageName(value);

        /// <summary>
        /// Can set storage name
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool CanSetStorageName([CanBeNull] string value)
            => Annotations.CanSetAnnotation(CypherAnnotationNames.PropertyStorageName, value);

        /// <summary>
        /// Set storage type
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool HasStorageType([CanBeNull] string value) => SetStorageType(value);

        /// <summary>
        /// Set default storage constraint
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool HasDefaultStorageConstraint([CanBeNull] string value)
            => SetDefaultStorageConstraint(value);

        /// <summary>
        /// Set computed storage constraint
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool HasComputedStorageConstraint([CanBeNull] string value)
            => SetComputedStorageConstraint(value);

        /// <summary>
        /// Set default value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool HasDefaultValue([CanBeNull] object value)
            => SetDefaultValue(value);
    }
}