// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Metadata
{
    /// <summary>
    /// All of the concretes in the EF metadata are annotatables.  The purpose
    /// of annotatables (which the relational extensions lean on) is to allow 
    /// for arbitrary metadata (i.e. objects) to be stored inside EF metadata.
    /// </summary>
    /// <remarks>Class is a rip of the RelationalAnnotations</remarks>
    public class CypherAnnotations
    {
        /// <summary>
        /// Wraps an EF annotatable (think bag)
        /// </summary>
        /// <param name="metadata"></param>
        public CypherAnnotations(
            [NotNull] IAnnotatable metadata
        )
        {
            Check.NotNull(metadata, nameof(metadata));

            Metadata = metadata;
        }

        /// <summary>
        /// Wrapped annotation
        /// </summary>
        /// <returns></returns>
        public virtual IAnnotatable Metadata { get; }

        /// <summary>
        /// Stores the value by name
        /// </summary>
        /// <param name="annotationName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool SetAnnotation(
            [NotNull] string name,
            [CanBeNull] object value)
        {
            ((IMutableAnnotatable)Metadata)[name] = value;

            return true;
        }

        public virtual bool CanSetAnnotation(
            [NotNull] string name,
            [CanBeNull] object value
        ) => true;
    }
}