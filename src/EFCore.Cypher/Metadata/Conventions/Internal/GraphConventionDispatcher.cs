using System;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal
{
    public partial class GraphConventionDispatcher {

        private GraphConventionScope _scope;

        public virtual IGraphConventionBatch StartBatch() => new GraphConventionBatch(this);

        public virtual bool OnPropertyFieldChanged(
            [NotNull] InternalNodePropertyBuilder builder,
            [CanBeNull] FieldInfo oldFieldInfo
        ) => _scope.OnPropertyFieldChanged(builder, oldFieldInfo);
        
    }
}