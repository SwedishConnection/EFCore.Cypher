namespace Microsoft.EntityFrameworkCore.Metadata
{
    public interface IMutableNodeProperty: INodeProperty, IMutableAnnotatable {

        /// <summary>
        /// Ownership
        /// </summary>
        /// <returns></returns>
        new IMutableNode DeclaringNode { get; }

        /// <summary>
        /// Is there a property existence constraint
        /// </summary>
        /// <returns></returns>
        new bool IsNullable { get; set; }
    }
}