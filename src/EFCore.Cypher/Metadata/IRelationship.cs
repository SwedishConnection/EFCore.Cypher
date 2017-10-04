namespace Microsoft.EntityFrameworkCore.Metadata
{
    public interface IRelationship: INode {
        /// <summary>
        /// Starting entity
        /// </summary>
        /// <returns></returns>
        IEntity Start { get; }

        /// <summary>
        /// Ending entity
        /// </summary>
        /// <returns></returns>
        IEntity End { get; }
    }
}