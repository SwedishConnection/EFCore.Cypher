using System.Reflection;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    public class ClrNodePropertyGetterFactory: ClrNodeAccessorFactory<IClrNodePropertyGetter> {
        protected override IClrNodePropertyGetter CreateGeneric<TNode, TValue, TNonNullableEnumValue>(
            PropertyInfo propertyInfo, 
            INodeProperty nodeProperty
        ) {
            var memberInfo = nodeProperty?.GetMemberInfo(forConstruction: false, forSet: false)
                ?? propertyInfo.FindGetterProperty();
        }    
    }
}