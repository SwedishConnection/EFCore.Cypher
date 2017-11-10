// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.


using System;
using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore
{
    public struct CypherRawString {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        public CypherRawString(
            [NotNull] string s
        ) => Format = s;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string Format { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        public static implicit operator CypherRawString(
             [NotNull] string s
        ) => new CypherRawString(s);

        /// <summary>
        /// TODO: use default (7.1)
        /// </summary>
        /// <param name="fs"></param>
        public static implicit operator CypherRawString(
            [NotNull] FormattableString fs
        ) => new CypherRawString(fs.Format);
    }
}