// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Microsoft.EntityFrameworkCore.Utilities {

    public class AnonymousTypeBuilderTest {

        /// <summary>
        /// Create an anonymous type where the order of the properties 
        /// is reflected in the constructor
        /// </summary>
        [Fact]
        public void Create() {
            var properties = new List<KeyValuePair<string, Type>> {
                new KeyValuePair<string, Type>("Name", typeof(string)),
                new KeyValuePair<string, Type>("HowManyHotdogs", typeof(int)),
                new KeyValuePair<string, Type>("IsDiningOut", typeof(bool))
            };

            var anonymous = AnonymousTypeBuilder.Create(properties);

            var parameters = anonymous
                .GetConstructors()
                .First()
                .GetParameters()
                .Select(p => p.ParameterType);


            Assert.True(
                parameters
                    .SequenceEqual(properties.Select(p => p.Value))
            );

            ConstructorInfo ctor = anonymous
                .GetConstructor(properties.Select(p => p.Value).ToArray());

            object me = ctor.Invoke(new object[] { "Mary", 3, true });

            Assert.NotNull(me);
            Assert.Equal("Mary", anonymous.GetProperty("Name").GetValue(me, null));
            Assert.Equal(3, anonymous.GetProperty("HowManyHotdogs").GetValue(me, null));
            Assert.Equal(true, anonymous.GetProperty("IsDiningOut").GetValue(me, null));
        }

        /// <summary>
        /// Fail if the properties list is empty
        /// </summary>
        [Fact]
        public void Fail_if_no_properties() {
            var properties = new List<KeyValuePair<string, Type>> {};

            Assert.Throws<InvalidOperationException>(
                () => AnonymousTypeBuilder.Create(properties)
            );
        }

        /// <summary>
        /// Fail if the properties list is null
        /// </summary>
        [Fact]
        public void Fail_if_properties_null() {
            Assert.Throws<ArgumentNullException>(
                () => AnonymousTypeBuilder.Create(null)
            );
        }

        /// <summary>
        /// Fail if duplicates exist in the properties list
        /// </summary>
        [Fact]
        public void Fail_if_properties_has_duplicates() {
            var properties = new List<KeyValuePair<string, Type>> {
                new KeyValuePair<string, Type>("Name", typeof(string)),
                new KeyValuePair<string, Type>("Name", typeof(int))
            };

            Assert.Throws<InvalidOperationException>(
                () => AnonymousTypeBuilder.Create(properties)
            );
        }

        /// <summary>
        /// Fail if property list has a key with null or whitespace
        /// </summary>
        [Fact]
        public void Fail_if_properties_has_bad_key() {
            var propsWithNullKey = new List<KeyValuePair<string, Type>> {
                new KeyValuePair<string, Type>(null, typeof(string))
            };

            Assert.Throws<ArgumentException>(
                () => AnonymousTypeBuilder.Create(propsWithNullKey)
            );

            var propsWithEmptyKey = new List<KeyValuePair<string, Type>> {
                new KeyValuePair<string, Type>(String.Empty, typeof(string))
            };

            Assert.Throws<ArgumentException>(
                () => AnonymousTypeBuilder.Create(propsWithEmptyKey)
            );

            var propsWithWhitespaceKey = new List<KeyValuePair<string, Type>> {
                new KeyValuePair<string, Type>("   ", typeof(string))
            };

            Assert.Throws<ArgumentException>(
                () => AnonymousTypeBuilder.Create(propsWithWhitespaceKey)
            );
        }

        /// <summary>
        /// Fail if property list has a value that is null
        /// </summary>
        [Fact]
        public void Fail_if_properties_has_bad_value() {
            var propsWithNullValue = new List<KeyValuePair<string, Type>> {
                new KeyValuePair<string, Type>("Name", null)
            };

            Assert.Throws<ArgumentException>(
                () => AnonymousTypeBuilder.Create(propsWithNullValue)
            );
        }
    }
}