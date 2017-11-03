// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Microsoft.EntityFrameworkCore.Metadata
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    [SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes", Justification = "We want users to be able to extend this class")]
    public class StorageAttribute: Attribute {
        private readonly string _name;

        private string _typeName;

        private int _order = -1;

        public StorageAttribute() {}

        public StorageAttribute(string name) {
            if (string.IsNullOrWhiteSpace(name)) {
                throw new ArgumentException(String.Format(
                    CultureInfo.CurrentCulture, 
                    $"Argument {name} is null or whitespace", 
                    "name"
                ));
            }

            _name = name;
        }

        public string Name { get { return _name; } }

        public int Order {
            get { return _order; }
            set {
                if (value < 0) {
                    throw new ArgumentOutOfRangeException("value");
                }

                _order = value;
            }
        }

        public string TypeName {
            get { return _typeName; }
            set {
                if (string.IsNullOrWhiteSpace(value)) {
                    throw new ArgumentException(String.Format(
                        CultureInfo.CurrentCulture, 
                        $"Argument {value} is null or whitespace", 
                        "value"
                    ));
                }

                _typeName = value;
            }
        }
    }
}