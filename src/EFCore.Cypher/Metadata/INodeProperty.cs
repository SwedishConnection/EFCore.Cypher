using System;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Microsoft.EntityFrameworkCore.Metadata
{
    public interface INodeProperty: IAnnotatable {
        
        /// <summary>
        /// Property name
        /// </summary>
        /// <returns></returns>
        string Name { get; }

        /// <summary>
        /// Ownership
        /// </summary>
        /// <returns></returns>
        INode DeclaringNode { get; }

        /// <summary>
        /// CLR property type
        /// </summary>
        /// <returns></returns>
        Type ClrType { get; }

        /// <summary>
        /// Property info (may be null for shadow properties or properties mapped directly to fields)
        /// </summary>
        /// <returns></returns>
        PropertyInfo PropertyInfo { get; }

        /// <summary>
        /// Field infor (may be null for shadow properties or if the backing field for the property is not known)
        /// </summary>
        /// <returns></returns>
        FieldInfo FieldInfo { get; }

        /// <summary>
        /// Is shadow property
        /// </summary>
        /// <returns></returns>
        bool IsShadowProperty { get; }

        /// <summary>
        /// Is there a property existence constraint
        /// </summary>
        /// <returns></returns>
        bool IsNullable { get; }
    }
}