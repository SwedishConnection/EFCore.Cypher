// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Utilities;
using Remotion.Linq.Clauses;

namespace Microsoft.EntityFrameworkCore.Query
{
    public class CypherQueryCompilationContext: QueryCompilationContext {
        private readonly List<CypherQueryModelVisitor> _visitors = new List<CypherQueryModelVisitor>();

        private readonly ISet<string> _nodeAliasSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        private const string DefaultAliasPrefix = "n";

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
        /// TODO: What is the actual max size
        /// </summary>
        public virtual int MaxNodeAliasLength => 128;

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

        /// <summary>
        /// First read only expression
        /// </summary>
        /// <param name="querySource"></param>
        /// <returns></returns>
        public virtual ReadOnlyExpression FindReadOnlyExpression(
            [NotNull] IQuerySource querySource
        ) {
            return
                (from v in _visitors
                 let readOnlyExpression = v.TryGetQuery(querySource)
                 where readOnlyExpression != null
                 select readOnlyExpression)
                .First();
        }

        /// <summary>
        /// Create unique node alias
        /// </summary>
        /// <returns></returns>
        public virtual string CreateUniqueNodeAlias()
            => CreateUniqueNodeAlias(DefaultAliasPrefix);

        /// <summary>
        /// Create unqiue node alias
        /// </summary>
        /// <param name="existing"></param>
        /// <returns></returns>
        public virtual string CreateUniqueNodeAlias(
            [NotNull] string existing
        ) {
            Check.NotNull(existing, nameof(existing));

            if (existing.Length == 0) {
                return existing;
            }

            while (existing.Length > MaxNodeAliasLength - 3) {
                var whereIsDot = existing.IndexOf(
                    ".",
                    StringComparison.OrdinalIgnoreCase
                );

                existing = whereIsDot > 0 
                    ? existing.Substring(whereIsDot + 1)
                    : existing.Substring(0, MaxNodeAliasLength - 3);
            }

            var counter = 0;
            var unique = existing;

            while (_nodeAliasSet.Contains(unique)) {
                unique = existing + counter++;
            }
            _nodeAliasSet.Add(unique);

            return unique;
        }
    }
}