using System;
using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    public class ClrNodePropertyGetter<TNode, TValue> : IClrNodePropertyGetter where TNode: class {
        private readonly Func<TNode, TValue> _getter;

        public ClrNodePropertyGetter([NotNull] Func<TNode, TValue> getter) {
            _getter = getter;
        }

        public object GetClrValue([NotNull] object instance) =>
            _getter((TNode)instance);
    }
}