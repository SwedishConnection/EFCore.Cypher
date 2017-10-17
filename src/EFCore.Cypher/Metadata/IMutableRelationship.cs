using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Metadata
{
    public interface IMutableRelationship: IRelationship, IMutableNode {
        /// <summary>
        /// Get or set the base type
        /// </summary>
        /// <returns></returns>
        new IMutableRelationship BaseType {get; [param: CanBeNull] set; }

        /// <summary>
        /// Get or set start
        /// </summary>
        /// <returns></returns>
        new IMutableEntity Start { get; set; }

        /// <summary>
        /// Get or set end
        /// </summary>
        /// <returns></returns>
        new IMutableEntity End { get; set; }
    }
}