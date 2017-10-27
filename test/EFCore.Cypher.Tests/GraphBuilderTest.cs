
using System;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Xunit;

namespace Microsoft.EntityFrameworkCore {

    public class GraphBuilderTest {
        private readonly CypherConventionSet _cypherConventionSet;

        public GraphBuilderTest() {
            _cypherConventionSet = new CypherConventionSet();
        }

        [Fact]
        public void EntityWithType() {
            GraphBuilder builder = new GraphBuilder(_cypherConventionSet);
            
            var entityBuilder = builder.Entity<Person>();
            IMutableEntityType entity = builder
                .Graph
                .FindEntityType(typeof(Person).DisplayName());

            Assert.Equal(entity.ClrType, typeof(Person));
        }

        [Fact]
        public void DuplicatesSwallowed() {
            GraphBuilder builder = new GraphBuilder(_cypherConventionSet);

            builder.Entity<Person>();
            builder.Entity<Person>();
        }

        private class Person {

        }
    }
}