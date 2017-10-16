using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Metadata
{
    public interface IMutableNode: INode, IMutableAnnotatable {

        /// <summary>
        /// Graph
        /// </summary>
        /// <returns></returns>
        new IMutableGraph Graph { get; }

        /// <summary>
        /// Get or set base type
        /// </summary>
        /// <returns></returns>
        new IMutableNode BaseNode { get; [param: CanBeNull] set; }

        /// <summary>
        /// Get mutable constraints
        /// </summary>
        /// <returns></returns>
        new IEnumerable<IMutableConstraint> GetConstraints();

        /// <summary>
        /// Add exist constraint
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        IMutableConstraint AddExistConstraint([NotNull] IMutableNodeProperty property);

        /// <summary>
        /// Remove unique constraint
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        IMutableConstraint RemoveExistConstraint([NotNull] INodeProperty property);

        /// <summary>
        /// Defined properties
        /// </summary>
        /// <returns></returns>
        new IEnumerable<IMutableNodeProperty> GetProperties();

        /// <summary>
        /// Find property by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        new IMutableNodeProperty FindProperty([NotNull] string name);

        /// <summary>
        /// Add property
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IMutableProperty AddProperty([NotNull] string name, [CanBeNull] Type propertyType);

        /// <summary>
        /// Remove property
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IMutableProperty RemoveProperty([NotNull] string name);
    }
}