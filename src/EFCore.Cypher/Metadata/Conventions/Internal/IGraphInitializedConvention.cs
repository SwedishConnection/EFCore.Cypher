using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal
{
    public interface IGraphInitializedConvention {
        InternalGraphBuilder Apply([NotNull] InternalGraphBuilder modelBuilder);
    }
}