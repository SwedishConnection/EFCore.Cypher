using System;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal
{
    public partial class GraphConventionDispatcher {
        private readonly GraphImmediateConventionScope _immediateConventionScope;

        public GraphConventionDispatcher([NotNull] GraphConventionSet graphConventionSet) {
            Check.NotNull(graphConventionSet, nameof(graphConventionSet));

            _immediateConventionScope = new GraphImmediateConventionScope(graphConventionSet);
            _scope = _immediateConventionScope;
        }

        private GraphConventionScope _scope;

        public virtual IGraphConventionBatch StartBatch() => new GraphConventionBatch(this);

        public virtual InternalEntityBuilder OnEntityAdded([NotNull] InternalEntityBuilder builder)
            => _scope.OnEntityAdded(Check.NotNull(builder, nameof(builder)));

        public virtual bool OnPropertyFieldChanged(
            [NotNull] InternalNodePropertyBuilder builder,
            [CanBeNull] FieldInfo oldFieldInfo
        ) => _scope.OnPropertyFieldChanged(builder, oldFieldInfo);
        
    }
}