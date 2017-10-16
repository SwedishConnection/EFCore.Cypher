using System;
using System.Reflection;
using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    public static class NodePropertyExensions {

        public static MemberInfo GetMemberInfo(
            [NotNull] this INodeProperty nodeProperty,
            bool forConstruction,
            bool forSet)
        {
            if (nodeProperty.TryGetMemberInfo(forConstruction, forSet, out var memberInfo, out var errorMessage))
            {
                return memberInfo;
            }

            throw new InvalidOperationException(errorMessage);
        }

        public static PropertyAccessMode? GetPropertyAccessMode([NotNull] this INodeProperty nodeProperty) => 
            (PropertyAccessMode?)nodeProperty[CoreAnnotationNames.PropertyAccessModeAnnotation] ?? nodeProperty.DeclaringNode.GetPropertyAccessMode();

        public static bool TryGetMemberInfo(
            [NotNull] this INodeProperty nodeProperty,
            bool forConstruction,
            bool forSet,
            out MemberInfo memberInfo,
            out string errorMessage)
        {
            // default outs
            memberInfo = null;
            errorMessage = null;

            var propertyInfo = nodeProperty.PropertyInfo;
            var fieldInfo = nodeProperty.FieldInfo;
            // TODO: navigation

            var mode = nodeProperty.GetPropertyAccessMode();
            if (mode is null ||
                mode == PropertyAccessMode.FieldDuringConstruction) {
                if (forConstruction &&
                    fieldInfo != null &&
                    !fieldInfo.IsInitOnly) {
                    memberInfo = fieldInfo;
                    return true;
                }
            }

            return true;
        }
    }
}