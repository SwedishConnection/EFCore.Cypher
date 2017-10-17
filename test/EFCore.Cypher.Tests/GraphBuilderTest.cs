
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
            IMutableEntity entity = builder.Graph.FindEntity(new string[] { "Person" });

            Assert.Equal(entity.ClrType, typeof(Person));
        }

        private class Person {

        }
    }
}