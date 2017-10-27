// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal
{
    /// <summary>
    /// Graph convention dispatcher
    /// </summary>
    public partial class CypherConventionDispatcher {

        private CypherConventionScope _scope;

        private readonly CypherImmediateConventionScope _immediateScope;

        public CypherConventionDispatcher([NotNull] CypherConventionSet conventions) {
            Check.NotNull(conventions, nameof(conventions));

            _immediateScope = new CypherImmediateConventionScope(conventions);
            _scope = _immediateScope;
        }

        /// <summary>
        /// When graph initialized (immediate only)
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public virtual CypherInternalGraphBuilder OnGraphInitialized([NotNull] CypherInternalGraphBuilder builder) =>
            _immediateScope.OnGraphInitialized(Check.NotNull(builder, nameof(builder)));

        /// <summary>
        /// Add entity
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public virtual CypherInternalEntityBuilder OnEntityAdded([NotNull] CypherInternalEntityBuilder builder) =>
            _scope.OnEntityAdded(Check.NotNull(builder, nameof(builder)));
            

        /// <summary>
        /// Ignore entity
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual bool OnEntityIgnored([NotNull] CypherInternalGraphBuilder builder, [NotNull] string name, [CanBeNull] Type type)
            => _scope.OnEntityIgnored(Check.NotNull(builder, nameof(builder)), Check.NotNull(name, nameof(name)), type);

        /// <summary>
        /// Base entity changed
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="previous"></param>
        /// <returns></returns>
        public virtual CypherInternalEntityBuilder OnBaseEntityChanged([NotNull] CypherInternalEntityBuilder builder, [CanBeNull] CypherEntity previous)
            => _scope.OnBaseEntityChanged(Check.NotNull(builder, nameof(builder)), previous);

        /// <summary>
        /// Started delayed scope
        /// </summary>
        /// <returns></returns>
        public virtual IConventionBatch StartBatch() => new CypherConventionBatch(this);

        /// <summary>
        /// Batching (i.e. deferred scoping)
        /// </summary>
        private class CypherConventionBatch : IConventionBatch
        {
            private readonly CypherConventionDispatcher _dispatcher;

            private int _runCount;

            /// <summary>
            /// Lift the dispatcher's scope into a delayed scope
            /// </summary>
            /// <param name="dispatcher"></param>
            public CypherConventionBatch(CypherConventionDispatcher dispatcher)
            {
                _dispatcher = dispatcher;
                var currentScope = _dispatcher._scope;
                dispatcher._scope = new CypherConventionScope(currentScope, children: null);

                if (currentScope != _dispatcher._immediateScope) {
                    currentScope.Add(dispatcher._scope);
                }
            }

            /// <summary>
            /// 
            /// </summary>
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
                    _dispatcher._scope = new CypherConventionScope(_dispatcher._immediateScope, children: null);
                    new RunVisitor(_dispatcher).VisitCypherConventionScope(currentScope);
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