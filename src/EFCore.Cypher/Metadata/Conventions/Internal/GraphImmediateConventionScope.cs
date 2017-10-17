using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal
{
    public class GraphImmediateConventionScope: GraphConventionScope {
        private readonly GraphConventionSet _graphConventionSet;

        public GraphImmediateConventionScope([NotNull] GraphConventionSet graphConventionSet): base(parent: null, children: null) {
            _graphConventionSet = graphConventionSet;
            MakeReadonly();
        }

        public override bool OnPropertyFieldChanged(InternalNodePropertyBuilder builder, FieldInfo oldFieldInfo) {
            if (builder.Metadata.Builder == null || 
                builder.Metadata.DeclaringType.Builder == null) {
                return false;        
            }

            return true;
        }
    }
}