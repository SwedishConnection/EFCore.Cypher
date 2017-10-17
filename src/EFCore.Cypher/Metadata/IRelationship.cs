namespace Microsoft.EntityFrameworkCore.Metadata
{
    public interface IRelationship: INode {
        /// <summary>
        /// Relationships derive only from relationships
        /// </summary>
        /// <returns></returns>
        new IRelationship BaseType { get; }

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