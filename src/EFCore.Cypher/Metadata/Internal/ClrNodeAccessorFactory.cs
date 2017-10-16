using System;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    public abstract class ClrNodeAccessorFactory<TAccessor> where TAccessor: class {
        private static readonly MethodInfo _genericCreate
            = typeof(ClrNodeAccessorFactory<TAccessor>).GetTypeInfo().GetDeclaredMethods(nameof(CreateGeneric)).Single();

        public virtual TAccessor Create([NotNull] INodeProperty nodeProperty)
            => nodeProperty as TAccessor ?? Create(null, nodeProperty);

        public virtual TAccessor Create([NotNull] PropertyInfo propertyInfo)
            => Create(propertyInfo, null);

        private TAccessor Create(PropertyInfo propertyInfo, INodeProperty nodeProperty)
        {
            var boundMethod = !(nodeProperty is null)
                ? _genericCreate.MakeGenericMethod(
                    nodeProperty.DeclaringNode.ClrType,
                    nodeProperty.ClrType,
                    nodeProperty.ClrType.UnwrapNullableType()
                )
                : _genericCreate.MakeGenericMethod(
                    propertyInfo.DeclaringType,
                    propertyInfo.PropertyType,
                    propertyInfo.PropertyType.UnwrapNullableType()
                );

            try {
                return (TAccessor)boundMethod.Invoke(this, new object[] { propertyInfo, nodeProperty });
            }
            catch (TargetInvocationException e) when (e.InnerException != null) {
                throw e.InnerException;
            }
        }

        protected abstract TAccessor CreateGeneric<TNode, TValue, TNonNullableEnumValue>(
            [CanBeNull] PropertyInfo propertyInfo,
            [CanBeNull] INodeProperty nodeProperty)
            where TNode : class;
    }
}