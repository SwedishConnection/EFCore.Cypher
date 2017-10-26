using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal
{
    public partial class GraphConventionDispatcher {

        private GraphConventionScope _scope;

        private readonly GraphImmediateConventionScope _immediateScope;

        public GraphConventionDispatcher([NotNull] GraphConventionSet conventions) {
            Check.NotNull(conventions, nameof(conventions));

            _immediateScope = new GraphImmediateConventionScope(conventions);
            _scope = _immediateScope;
        }

        /// <summary>
        /// When graph initialized (immediate only)
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public virtual InternalGraphBuilder OnGraphInitialized([NotNull] InternalGraphBuilder builder) =>
            _immediateScope.OnGraphInitialized(Check.NotNull(builder, nameof(builder)));

        /// <summary>
        /// Add entity
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public virtual InternalEntityBuilder OnEntityAdded([NotNull] InternalEntityBuilder builder) =>
            _scope.OnEntityAdded(Check.NotNull(builder, nameof(builder)));

        /// <summary>
        /// Ignore entity
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual bool OnEntityIgnored([NotNull] InternalGraphBuilder builder, [NotNull] string name, [CanBeNull] Type type)
            => _scope.OnEntityIgnored(Check.NotNull(builder, nameof(builder)), Check.NotNull(name, nameof(name)), type);

        public virtual InternalEntityBuilder OnBaseEntityChanged([NotNull] InternalEntityBuilder builder, [CanBeNull] Entity previous)
            => _scope.OnBaseEntityChanged(Check.NotNull(builder, nameof(builder)), previous);

        /// <summary>
        /// Started delayed scope
        /// </summary>
        /// <returns></returns>
        public virtual IConventionBatch StartBatch() => new GraphConventionBatch(this);

        private class GraphConventionBatch : IConventionBatch
        {
            private readonly GraphConventionDispatcher _dispatcher;

            private int _runCount;

            public GraphConventionBatch(GraphConventionDispatcher dispatcher)
            {
                _dispatcher = dispatcher;
                var currentScope = _dispatcher._scope;
                dispatcher._scope = new GraphConventionScope(currentScope, children: null);

                if (currentScope != _dispatcher._immediateScope) {
                    currentScope.Add(dispatcher._scope);
                }
            }

            private void Run()
            {
                while (true)
                {
                    if (_runCount++ == short.MaxValue)
                    {
                        throw new InvalidOperationException(CoreStrings.ConventionsInfiniteLoop);
                    }

                    var currentScope = _dispatcher._scope;
                    if (currentScope == _dispatcher._immediateScope)
                    {
                        return;
                    }

                    _dispatcher._scope = currentScope.Parent;
                    currentScope.MakeReadonly();

                    if (currentScope.Parent != _dispatcher._immediateScope
                        || currentScope.GetLeafCount() == 0)
                    {
                        return;
                    }

                    // Capture all nested convention invocations to unwind the stack
                    _dispatcher._scope = new GraphConventionScope(_dispatcher._immediateScope, children: null);
                    new RunVisitor(_dispatcher).VisitGraphConventionScope(currentScope);
                }
            }

            public ForeignKey Run(ForeignKey foreignKey)
            {
                throw new System.NotImplementedException();
            }

            public void Dispose()
            {
                if (_runCount == 0)
                {
                    Run();
                }
            }
        }
    }

}