// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Microsoft.EntityFrameworkCore.Metadata
{
    /// <summary>
    /// Labels for nodes and relationships (singular)
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    [SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes", Justification = "We want users to be able to extend this class")]
    public class LabelsAttribute: Attribute {
        
        private readonly string[] _names;

        public LabelsAttribute(string[] names) {
            if (ReferenceEquals(names, null) || 
                names.Any(n => string.IsNullOrWhiteSpace(n))) {
                 throw new ArgumentException(String.Format(
                     CultureInfo.CurrentCulture, 
                     $"Argument {names} is null or an item is null or whitespace", 
                     "names"
                    ));   
            }

            _names = names;
        }

        public string[] Names { get { return _names; } }
    }
}