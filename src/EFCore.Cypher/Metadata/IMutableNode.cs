using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Metadata
{
    public interface IMutableNode: INode, IMutableAnnotatable {

        new IMutableGraph Graph { get; }

        new IMutableNode BaseNode { get; [param: CanBeNull] set; }

        /// <summary>
        /// Get constraints
        /// </summary>
        /// <returns></returns>
        new IEnumerable<IMutableConstraint> GetConstraints();

        /// <summary>
        /// Add unique constraint
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        IMutableConstraint AddUniqueConstraint([NotNull] IMutableNodeProperty property);

        /// <summary>
        /// Remove unique constraint
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        IMutableConstraint RemoveUniqueConstraint([NotNull] INodeProperty property);

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
        /// Add keys constraint (implicit exists assertion)
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        IMutableConstraint AddKeysConstraint([NotNull] IReadOnlyList<IMutableNodeProperty> properties);

        /// <summary>
        /// Remove keys constraint (implicit exists assertion)
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        IMutableConstraint RemoveKeysConstraint([NotNull] IReadOnlyList<INodeProperty> properties);

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