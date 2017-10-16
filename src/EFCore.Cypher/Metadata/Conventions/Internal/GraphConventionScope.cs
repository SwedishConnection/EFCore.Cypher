using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal
{
    public class GraphConventionScope: GraphConventionNode {
        private readonly List<GraphConventionNode> _children;
        private bool _readonly;

        public GraphConventionScope(GraphConventionScope parent, List<GraphConventionNode> children) {
            Parent = parent;
            _children = children ?? new List<GraphConventionNode>();
        }

        /// <summary>
        /// Readonly switch that cannot be reversed
        /// </summary>
        public void MakeReadonly() => _readonly = true;

        /// <summary>
        /// Parent scope
        /// </summary>
        /// <returns></returns>
        public GraphConventionScope Parent { get; }

        /// <summary>
        /// Children
        /// </summary>
        /// <returns></returns>
        public IReadOnlyCollection<GraphConventionNode> Children { get { return _children; } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="visitor"></param>
        /// <returns></returns>
        public override GraphConventionNode Accept(GraphConventionVisitor visitor) => 
            visitor.VisitGraphConventionScope(this);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="oldFieldInfo"></param>
        /// <returns></returns>
        public virtual bool OnPropertyFieldChanged([NotNull] InternalNodePropertyBuilder builder, [CanBeNull] FieldInfo oldFieldInfo) {
            Add(new OnPropertyFieldChangedNode(builder, oldFieldInfo));
            return true;
        }

        /// <summary>
        /// When readonly error otherwise add node to the internal list
        /// </summary>
        /// <param name="node"></param>
        public void Add(GraphConventionNode node) {
            if (_readonly) {
                throw new InvalidOperationException();
            }

            _children.Add(node);
        }
    }
}