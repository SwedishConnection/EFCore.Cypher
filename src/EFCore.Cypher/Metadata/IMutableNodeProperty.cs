namespace Microsoft.EntityFrameworkCore.Metadata
{
    public interface IMutableNodeProperty: INodeProperty, IMutableAnnotatable {

        /// <summary>
        /// Delcaring type
        /// </summary>
        /// <returns></returns>
        new IMutableNode DeclaringType { get; }

        /// <summary>
        /// Is there a property existence constraint
        /// </summary>
        /// <returns></returns>
        new bool IsNullable { get; set; }
    }
}