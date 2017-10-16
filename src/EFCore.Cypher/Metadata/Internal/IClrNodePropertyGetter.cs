using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    public interface IClrNodePropertyGetter {
        object GetClrValue([NotNull] object instance);
    }
}