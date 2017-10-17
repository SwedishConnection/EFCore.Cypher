using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{

    public abstract class InternalMetadataBuilder {

        protected InternalMetadataBuilder([NotNull] ConventionalAnnotatable metadata) {
            Metadata = metadata;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual ConventionalAnnotatable Metadata { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract InternalGraphBuilder GraphBuilder { get; }
    }

    public abstract class InternalMetadataBuilder<TMetadata>: InternalMetadataBuilder where TMetadata: ConventionalAnnotatable {
        protected InternalMetadataBuilder([NotNull] TMetadata metadata): base(metadata) {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public new virtual TMetadata Metadata { 
            get { return (TMetadata)base.Metadata; } 
        }
    }
}