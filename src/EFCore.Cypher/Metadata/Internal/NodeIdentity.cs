using System;
using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    public struct NodeIdentity {
        private readonly object _labelsOrType;

        public NodeIdentity([NotNull] string[] labels): this((object)labels) {

        }

        public NodeIdentity([NotNull] Type type): this((object)type) {

        }

        private NodeIdentity(object labelsOrType) {
            _labelsOrType = labelsOrType;
        }

        public string[] Labels {
            get { return Type?.DisplayLabels() ?? (string[])_labelsOrType; }
        }

        public Type Type {
            get { return _labelsOrType as Type; }
        }
    }
}