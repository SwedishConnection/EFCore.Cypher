
using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Microsoft.EntityFrameworkCore.Metadata
{
    /// <summary>
    /// Mimics the ITypeBase
    /// </summary>
    public interface INode: IAnnotatable {
        /// <summary>
        /// Graph this node belongs to
        /// </summary>
        /// <returns></returns>
        IGraph Graph { get; }

        /// <summary>
        /// Labels of this type
        /// </summary>
        /// <returns></returns>
        string[] Labels { get; }

        /// <summary>
        /// Base type
        /// </summary>
        /// <returns></returns>
        INode BaseType { get; }

        /// <summary>
        /// CLR class representings instances of this type
        /// </summary>
        /// <returns></returns>
        Type ClrType { get; }

        /// <summary>
        /// Get property by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        INodeProperty FindProperty([NotNull] string name);

        /// <summary>
        /// Get properties
        /// </summary>
        /// <returns></returns>
        IEnumerable<INodeProperty> GetProperties();

        /// <summary>
        /// Gets constraints (e.g. unique, exists etc.)
        /// </summary>
        /// <returns></returns>
        IEnumerable<IConstraint> GetConstraints();
    }
}