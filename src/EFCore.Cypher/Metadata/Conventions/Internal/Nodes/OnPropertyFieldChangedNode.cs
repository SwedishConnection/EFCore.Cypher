using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal
{
    public class OnPropertyFieldChangedNode: GraphConventionNode {
        public OnPropertyFieldChangedNode(InternalNodePropertyBuilder builder, FieldInfo oldFieldInfo) {
            PropertyBuilder = builder;
            OldFieldInfo = oldFieldInfo;
        }

        public InternalNodePropertyBuilder PropertyBuilder { get; }

        public FieldInfo OldFieldInfo { get; }

        public override GraphConventionNode Accept(GraphConventionVisitor visitor) => visitor.VisitOnPropertyFieldChanged(this);
    }
}