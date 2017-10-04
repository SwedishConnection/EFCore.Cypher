using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Microsoft.EntityFrameworkCore.Metadata.Builders
{
    public class EntityBuilder : IInfrastructure<IMutableGraph>
    {
        public EntityBuilder() {

        }
        
        public IMutableGraph Instance => throw new System.NotImplementedException();
    }
}