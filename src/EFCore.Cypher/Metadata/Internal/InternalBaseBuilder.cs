using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{

    public abstract class InternalBaseBuilder {

        protected InternalBaseBuilder([NotNull] ConventionalAnnotatable baze) {
            Base = baze;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual ConventionalAnnotatable Base { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract InternalGraphBuilder GraphBuilder { get; }
    }

    public abstract class InternalBaseBuilder<TBase>: InternalBaseBuilder where TBase: ConventionalAnnotatable {
        protected InternalBaseBuilder([NotNull] TBase baze): base(baze) {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public new virtual TBase Base { get { return (TBase)base.Base; } }
    }
}