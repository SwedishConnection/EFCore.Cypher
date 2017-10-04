
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Microsoft.EntityFrameworkCore.Metadata
{
    public interface IConstraint: IAnnotatable {
        
        /// <summary>
        /// Properties for this constraint
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<INodeProperty> Properties { get; }

        /// <summary>
        /// Constraint Type
        /// </summary>
        /// <returns></returns>
        ConstraintType Type { get; }

        /// <summary>
        /// Node the constraint is defined on
        /// </summary>
        /// <returns></returns>
        INode DeclaringNode { get; }
    }
}