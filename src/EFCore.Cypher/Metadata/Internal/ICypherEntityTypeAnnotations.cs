// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Microsoft.EntityFrameworkCore.Metadata
{
    public interface ICypherEntityTypeAnnotations {
        /// <summary>
        /// Labels associated with this entity
        /// </summary>
        /// <returns></returns>
        string[] Labels { get; }
    }
}