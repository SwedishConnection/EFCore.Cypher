// Based on https://github.com/aspnet/EntityFrameworkCore
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Xunit;

namespace Microsoft.EntityFrameworkCore {

    public class GraphBuilderTest {

        public GraphBuilderTest() {
        }

        /// <summary>
        /// Create with generics then find by name
        /// </summary>
        [Fact]
        public void EntityWithType() {
            GraphBuilder builder = new GraphBuilder(new CypherConventionSet());
            
            var entityBuilder = builder.Entity<Person>();
            IMutableEntityType entity = builder
                .Graph
                .FindEntityType(typeof(Person).DisplayName());

            Assert.Equal(entity.ClrType, typeof(Person));
        }

        /// <summary>
        /// Duplicate create with generics no side-effects
        /// </summary>
        [Fact]
        public void DuplicatesSwallowed() {
            GraphBuilder builder = new GraphBuilder(new CypherConventionSet());

            builder.Entity<Person>();
            builder.Entity<Person>();
        }

        /// <summary>
        /// Explicit
        /// </summary>
        [Fact]
        public void CreateEvenIfIgnoring() {
            GraphBuilder builder = new GraphBuilder(new CypherConventionSet());

            var entityBuilder = builder.Ignore<Person>().Entity<Person>();
            IMutableEntityType entity = builder
                .Graph
                .FindEntityType(typeof(Person).DisplayName());

            Assert.Equal(entity.ClrType, typeof(Person));
        }

        private class Person {
            public string Name { get; set; }
        }
    }
}