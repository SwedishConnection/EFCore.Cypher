// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{

    public class CypherAnnotationsBuilder : CypherAnnotations
    {
        public CypherAnnotationsBuilder(
            [NotNull] InternalMetadataBuilder internalBuilder,
            ConfigurationSource configurationSource
        ) : base(internalBuilder.Metadata)
        {
            Check.NotNull(internalBuilder, nameof(internalBuilder));

            MetadataBuilder = internalBuilder;
            ConfigurationSource = configurationSource;
        }

        /// <summary>
        /// Configuration source
        /// </summary>
        /// <returns></returns>
        public virtual ConfigurationSource ConfigurationSource { get; }

        /// <summary>
        /// Wrapped builder
        /// </summary>
        /// <returns></returns>
        public virtual InternalMetadataBuilder MetadataBuilder { get; }

        /// <summary>
        /// Defers to the metadata builder using the instance configuration source 
        /// </summary>
        /// <param name="relationalAnnotationName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool SetAnnotation(
            string annotationName,
            object value)
            => MetadataBuilder.HasAnnotation(annotationName, value, ConfigurationSource);

        /// <summary>
        /// Defers to the metadata builder which when the value differs 
        /// the configuration source must override the existing
        /// </summary>
        /// <param name="relationalAnnotationName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool CanSetAnnotation(
            string annotationName,
            object value)
            => MetadataBuilder.CanSetAnnotation(annotationName, value, ConfigurationSource);
    }
}