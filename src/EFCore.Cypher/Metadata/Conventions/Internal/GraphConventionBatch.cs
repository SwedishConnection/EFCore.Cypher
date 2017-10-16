namespace Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal
{
    public class GraphConventionBatch: IGraphConventionBatch {

        private readonly GraphConventionDispatcher _dispatcher;

        public GraphConventionBatch(GraphConventionDispatcher dispatcher) {
            _dispatcher = dispatcher;
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}