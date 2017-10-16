using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Internal;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    public static class NodeExtensions {

        public static string[] DisplayLabels([NotNull] this INode node) {
            return node.ClrType != null
                ? new string[] { node.ClrType.ShortDisplayName() }
                : node.Labels;
        }

        public static bool HasClrType([NotNull] this INode node) {
            return node.ClrType != null;
        }

        public static bool HasDefiningNode([NotNull] this INode node) {
            return node.DefiningNode != null;
        }

        public static bool IsAbstract([NotNull] this INode node) {
            return node.ClrType?.GetTypeInfo().IsAbstract ?? false;
        }

        public static PropertyAccessMode? GetPropertyAccessMode([NotNull] this INode node) =>
            (PropertyAccessMode?)node[CoreAnnotationNames.PropertyAccessModeAnnotation] ?? node.Graph.GetPropertyAccessMode();
    }
}