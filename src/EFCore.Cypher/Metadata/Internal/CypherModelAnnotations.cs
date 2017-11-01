// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Metadata
{
    public class CypherModelAnnotations : ICypherModelAnnotations {

        public CypherModelAnnotations(
            [NotNull] IModel model
        ) : this(new CypherAnnotations(model))
        {
        }

        protected CypherModelAnnotations(
            [NotNull] CypherAnnotations annotations
        ) => Annotations = annotations;

        /// <summary>
        /// Cypher annotations 
        /// </summary>
        /// <returns></returns>
        protected virtual CypherAnnotations Annotations { get; }
    }
}
