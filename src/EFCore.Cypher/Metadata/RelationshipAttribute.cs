// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Microsoft.EntityFrameworkCore.Metadata
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    [SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes", Justification = "We want users to be able to extend this class")]
    public class RelationshipAttribute: Attribute {
        private readonly string _name;

        private readonly Type _clrType;

        public RelationshipAttribute(string name) {
            if (string.IsNullOrWhiteSpace(name)) {
                throw new ArgumentException(String.Format(
                    CultureInfo.CurrentCulture, 
                    $"Argument {name} is null or whitespace", 
                    "name"
                ));
            }

            _name = name;
        }

        public RelationshipAttribute(Type clrType) {
            if(ReferenceEquals(clrType, null)) {
                throw new ArgumentException(String.Format(
                    CultureInfo.CurrentCulture, 
                    $"Argument {clrType} is null", 
                    "clrType"
                ));
            }

            _clrType = clrType;
        }

        public string Name { get { return _name; } }

        public Type Type { get { return _clrType; } }
    }
}