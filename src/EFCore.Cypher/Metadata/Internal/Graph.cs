using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Microsoft.EntityFrameworkCore.Metadata.Internal
{
    public class Graph : ConventionalAnnotatable, IMutableGraph
    {
        private readonly SortedDictionary<string, INode> _nodes =
            new SortedDictionary<string, INode>();

        private readonly IDictionary<Type, INode> _clrTypeMap = 
            new Dictionary<Type, INode>();

        public Graph() {

        }

        // TODO: Conventions

        public virtual InternalGraphBuilder Buider { get; }

        public IMutableEntity AddEntity([NotNull] string[] labels)
        {
            throw new NotImplementedException();
        }

        public IMutableRelationship AddRelationship([NotNull] string[] labels)
        {
            throw new NotImplementedException();
        }

        public IMutableEntity FindEntity([NotNull] string[] labels)
        {
            throw new NotImplementedException();
        }

        public INode FindNode([NotNull] string[] labels)
        {
            throw new NotImplementedException();
        }

        public IMutableRelationship FindRelationship([NotNull] string[] labels)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<INode> GetNodes()
        {
            throw new NotImplementedException();
        }

        public IMutableNode RemoveEntity([NotNull] string[] labels)
        {
            throw new NotImplementedException();
        }

        public IMutableNode RemoveRelationship([NotNull] string[] labels)
        {
            throw new NotImplementedException();
        }
    }
}