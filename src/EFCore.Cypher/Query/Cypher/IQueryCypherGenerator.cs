using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;

namespace Microsoft.EntityFrameworkCore.Query.Cypher {

    public interface IQueryCypherGenerator {

        /// <summary>
        /// Generate Cypher
        /// </summary>
        /// <param name="IReadOnlyDictionary<string"></param>
        /// <param name="parameterValues"></param>
        /// <returns></returns>
        ICypherCommand GenerateCypher([NotNull] IReadOnlyDictionary<string, object> parameterValues);

        /// <summary>
        /// Is Cypher cachable
        /// </summary>
        /// <returns></returns>
        bool IsCachable { get; }
    }
}