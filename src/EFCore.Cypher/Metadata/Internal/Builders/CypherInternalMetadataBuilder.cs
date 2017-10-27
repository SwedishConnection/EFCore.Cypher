// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{

    public abstract class CypherInternalMetadataBuilder {

        protected CypherInternalMetadataBuilder([NotNull] ConventionalAnnotatable metadata) {
            Metadata = metadata;
        }

        /// <summary>
        /// Metadata (concrete thing being built)
        /// </summary>
        /// <returns></returns>
        public virtual ConventionalAnnotatable Metadata { get; }

        /// <summary>
        /// Graph builder
        /// </summary>
        /// <returns></returns>
        public abstract CypherInternalGraphBuilder GraphBuilder { get; }
    }

    public abstract class CypherInternalMetadataBuilder<TMetadata>: CypherInternalMetadataBuilder where TMetadata: ConventionalAnnotatable {
        protected CypherInternalMetadataBuilder([NotNull] TMetadata metadata): base(metadata) {
        }

        /// <summary>
        /// Metadata
        /// </summary>
        /// <returns></returns>
        public new virtual TMetadata Metadata { 
            get { return (TMetadata)base.Metadata; } 
        }
    }
}