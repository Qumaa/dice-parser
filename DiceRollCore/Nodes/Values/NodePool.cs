using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DiceRoll
{
    public sealed class NodePool : INodePool
    {
        private readonly IEnumerable<INode> _nodes;
        
        public NodePool(IEnumerable<INode> nodes)
        {
            ArgumentNullException.ThrowIfNull(nodes);
            
            _nodes = nodes;
        }

        public object Clone() =>
            new NodePool(_nodes.Select(x => x.CloneTyped()));

        public void Visit<T>(T visitor) where T : INodeVisitor =>
            visitor.ForNodePool(this);

        public void NextEvaluation()
        {
            foreach (INode node in _nodes)
                node.NextEvaluation();
        }

        public IEnumerator<INode> GetEnumerator() =>
            _nodes.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();
    }
}
