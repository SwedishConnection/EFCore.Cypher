using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Internal;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    public static class NodeExtensions {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static string[] DisplayLabels([NotNull] this INode node) {
            return node.ClrType != null
                ? new string[] { node.ClrType.ShortDisplayName() }
                : node.Labels;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static bool HasClrType([NotNull] this INode node) {
            return node.ClrType != null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static bool IsAbstract([NotNull] this INode node) {
            return node.ClrType?.GetTypeInfo().IsAbstract ?? false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static PropertyAccessMode? GetPropertyAccessMode([NotNull] this INode node) =>
            (PropertyAccessMode?)node[CoreAnnotationNames.PropertyAccessModeAnnotation] ?? node.Graph.GetPropertyAccessMode();

    }
}