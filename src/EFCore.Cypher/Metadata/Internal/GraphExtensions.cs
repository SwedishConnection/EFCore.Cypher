using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    public static class GraphExtensions {

        public static PropertyAccessMode? GetPropertyAccessMode(
            [NotNull] this IGraph graph)
            => (PropertyAccessMode?)graph[CoreAnnotationNames.PropertyAccessModeAnnotation];
    }
}