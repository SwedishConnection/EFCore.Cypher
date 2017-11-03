// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Diagnostics;

namespace System.Reflection
{
    [DebuggerStepThrough]
    public static class PropertyInfoExtensions
    {
        public static Type FindCandidateNavigationPropertyType(this PropertyInfo propertyInfo, Func<Type, bool> isPrimitiveProperty)
        {
            var targetType = propertyInfo.PropertyType;
            var targetSequenceType = targetType.TryGetSequenceType();
            if (!propertyInfo.IsCandidateProperty(targetSequenceType == null))
            {
                return null;
            }

            targetType = targetSequenceType ?? targetType;
            targetType = targetType.UnwrapNullableType();

            if (isPrimitiveProperty(targetType)
                || targetType.GetTypeInfo().IsInterface
                || targetType.GetTypeInfo().IsValueType
                || targetType == typeof(object))
            {
                return null;
            }

            return targetType;
        }

        public static bool IsCandidateProperty(this PropertyInfo propertyInfo, bool needsWrite = true)
            => !propertyInfo.IsStatic()
               && propertyInfo.GetIndexParameters().Length == 0
               && propertyInfo.CanRead
               && (!needsWrite || propertyInfo.CanWrite)
               && propertyInfo.GetMethod != null && propertyInfo.GetMethod.IsPublic;

        public static bool IsStatic(this PropertyInfo property)
            => (property.GetMethod ?? property.SetMethod).IsStatic;
    }
}