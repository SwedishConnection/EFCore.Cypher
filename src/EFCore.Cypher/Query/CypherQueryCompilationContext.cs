// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Query
{
    public class CypherQueryCompilationContext: QueryCompilationContext {
        private readonly List<CypherQueryModelVisitor> _visitors = new List<CypherQueryModelVisitor>();

        public CypherQueryCompilationContext(
            [NotNull] QueryCompilationContextDependencies dependencies,
            [NotNull] ILinqOperatorProvider linqOperatorProvider,
            [NotNull] IQueryMethodProvider queryMethodProvider,
            bool trackQueryResults
        ) : base(dependencies, linqOperatorProvider, trackQueryResults) {
            Check.NotNull(queryMethodProvider, nameof(queryMethodProvider));

            QueryMethodProvider = queryMethodProvider;
        }

        /// <summary>
        /// Query method provider
        /// </summary>
        /// <returns></returns>
        public virtual IQueryMethodProvider QueryMethodProvider { get; }

        /// <summary>
        /// Query model visitor
        /// </summary>
        /// <returns></returns>
        public override EntityQueryModelVisitor CreateQueryModelVisitor()
        {
            var visitor = (CypherQueryModelVisitor)base.CreateQueryModelVisitor();
            _visitors.Add(visitor);

            return visitor;
        }

        /// <summary>
        /// Query model visitor
        /// </summary>
        /// <param name="parentEntityQueryModelVisitor"></param>
        /// <returns></returns>
        public override EntityQueryModelVisitor CreateQueryModelVisitor(
            EntityQueryModelVisitor parentEntityQueryModelVisitor
        ) {
            var visitor
                = (CypherQueryModelVisitor)base.CreateQueryModelVisitor(parentEntityQueryModelVisitor);
            _visitors.Add(visitor);

            return visitor;
        }
    }
}