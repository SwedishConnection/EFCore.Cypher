using System.Collections.Generic;

namespace Microsoft.EntityFrameworkCore.Metadata
{
    public interface IMutableConstraint: IConstraint, IMutableAnnotatable {

        /// <summary>
        /// Properties for this constraint
        /// </summary>
        /// <returns></returns>
        new IReadOnlyList<INodeProperty> Properties { get; }

        /// <summary>
        /// Node the constraint is defined on
        /// </summary>
        /// <returns></returns>
        new IMutableNode DeclaringNode { get; }
    }
}