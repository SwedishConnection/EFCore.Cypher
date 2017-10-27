// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;

namespace System.Reflection
{
    [DebuggerStepThrough]
    internal static class SharedPropertyInfoExtensions
    {
        public static bool IsStatic(this PropertyInfo property)
            => (property.GetMethod ?? property.SetMethod).IsStatic;

        public static bool IsCandidateProperty(this PropertyInfo propertyInfo, bool needsWrite = true)
            => !propertyInfo.IsStatic()
               && propertyInfo.GetIndexParameters().Length == 0
               && propertyInfo.CanRead
               && (!needsWrite || propertyInfo.CanWrite)
               && propertyInfo.GetMethod != null && propertyInfo.GetMethod.IsPublic;
    }
}