// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.EntityFrameworkCore.Metadata
{
    /// <summary>
    /// Labels for nodes and relationships (singular)
    /// </summary>
    public class LabelsAttribute: Attribute {
        
        public LabelsAttribute(string[] names) {
            Names = names;
        }

        public string[] Names { get; }
    }
}