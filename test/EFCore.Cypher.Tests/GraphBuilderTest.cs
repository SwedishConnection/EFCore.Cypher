
using System;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Xunit;

namespace Microsoft.EntityFrameworkCore {

    public class GraphBuilderTest {
        private readonly GraphConventionSet _graphConventionSet;

        public GraphBuilderTest() {
            _graphConventionSet = new GraphConventionSet();
        }

        [Fact]
        public void EntityWithType() {
            GraphBuilder builder = new GraphBuilder(_graphConventionSet);
            
            var entityBuilder = builder.Entity<Person>();
            IMutableEntityType entity = builder
                .Graph
                .FindEntityType(typeof(Person).DisplayName());

            Assert.Equal(entity.ClrType, typeof(Person));
        }

        [Fact]
        public void DuplicatesSwallowed() {
            GraphBuilder builder = new GraphBuilder(_graphConventionSet);

            builder.Entity<Person>();
            builder.Entity<Person>();
        }

        private class Person {

        }
    }
}