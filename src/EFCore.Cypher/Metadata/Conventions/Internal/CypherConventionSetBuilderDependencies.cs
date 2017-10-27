// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal
{
    public sealed class CypherConventionSetBuilderDependencies {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeMapper"></param>
        public CypherConventionSetBuilderDependencies([NotNull] ITypeMapper typeMapper) {
            Check.NotNull(typeMapper, nameof(typeMapper));
        }

        /// <summary>
        /// Type mapper (storage)
        /// </summary>
        /// <returns></returns>
        public ITypeMapper TypeMapper { get; }

        /// <summary>
        /// With type mapper
        /// </summary>
        /// <param name="typeMapper"></param>
        /// <returns></returns>
        public CypherConventionSetBuilderDependencies With([NotNull] ITypeMapper typeMapper) 
            => new CypherConventionSetBuilderDependencies(typeMapper);
    }
}