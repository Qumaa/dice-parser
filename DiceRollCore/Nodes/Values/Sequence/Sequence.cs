using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DiceRoll
{
    public class Sequence<T> : ISequence<T> where T : INode
    {
        protected readonly T[] _nodes;

        public int Count => _nodes.Length;
        public T this[int index] => _nodes[index];

        public Sequence(IEnumerable<T> nodes)
        {
            ArgumentNullException.ThrowIfNull(nodes);
            
            _nodes = nodes.ToArray();
        }

        public Sequence(T node, int repetitionTimes)
        {
            ArgumentNullException.ThrowIfNull(node);
            ArgumentOutOfRangeException.ThrowIfLessThan(repetitionTimes, 1);
            
            T[] sourceNodes = new T[repetitionTimes];
            sourceNodes[0] = node;

            if (repetitionTimes > 1)
                for (int i = 1; i < repetitionTimes; i++)
                    sourceNodes[i] = node.CloneTyped();

            _nodes = sourceNodes;
        }

        public object Clone() =>
            Clone(CloneSourceNodes());

        public virtual void Visit<TVisitor>(TVisitor visitor) where TVisitor : INodeVisitor =>
            visitor.ForSequence(this);

        public virtual void NextEvaluation()
        {
            foreach (T node in _nodes)
                node.NextEvaluation();
        }

        protected virtual Sequence<T> Clone(T[] clonedNodes) =>
            new(clonedNodes);

        private T[] CloneSourceNodes()
        {
            T[] clones = new T[_nodes.Length];

            for (int i = 0; i < _nodes.Length; i++)
                clones[i] = _nodes[i].CloneTyped();

            return clones;
        }

        public IEnumerator<T> GetEnumerator() =>
            ((IEnumerable<T>) _nodes).GetEnumerator();
        
        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();
    }
}
