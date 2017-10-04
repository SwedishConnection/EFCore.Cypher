using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{

    public abstract class InternalBaseBuilder {

        protected InternalBaseBuilder([NotNull] ConventionalAnnotatable baze) {
            Base = baze;
        }

        public virtual ConventionalAnnotatable Base { get; }

        public abstract InternalGraphBuilder GraphBuilder { get; }
    }

    public abstract class InternalBaseBuilder<TBase>: InternalBaseBuilder where TBase: ConventionalAnnotatable {
        protected InternalBaseBuilder([NotNull] TBase annotatable): base(annotatable) {

        }

        public new virtual TBase Base { get { return (TBase)base.Base; } }
    }
}