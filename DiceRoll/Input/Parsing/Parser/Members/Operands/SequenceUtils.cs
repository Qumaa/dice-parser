using System;

namespace DiceRoll.Input.Parsing
{
    internal static class SequenceUtils
    {
        private static readonly TypingVisitor _visitor = new();
        
        public static bool TryGroupNodes(Mapped<Operand>[] nodes, out Mapped<Operand> grouped)
        {
            if (!_visitor.TryCreateTypedSequence(nodes, out INode sequence, out Type sequenceType))
            {
                grouped = default;
                return false;
            }

            Range poolRange = nodes[0].Range;

            for (int i = 1; i < nodes.Length; i++)
                poolRange = poolRange.And(nodes[i].Range);
            
            Operand linkedSequence = new(sequence, sequenceType, nodes);
            
            grouped = new Mapped<Operand>(linkedSequence, poolRange);
            return true;
        }
        
        private sealed class TypingVisitor : INodeVisitor
        {
            private Mapped<Operand>[] _nodes;
            private INode _sequence;
            private Type _type;

            public void ForNumeric(INumeric numeric) =>
                _sequence = Type<INumeric>();

            public void ForAssertion(IAssertion assertion) =>
                _sequence = Type<IAssertion>();

            public void ForOperation(IOperation operation) =>
                _sequence = Type<IOperation>();

            public void ForSequence<T>(ISequence<T> sequence) where T : INode =>
                _sequence = Type<ISequence<T>>();

            public bool TryCreateTypedSequence(Mapped<Operand>[] nodes, out INode sequence, out Type type)
            {
                _nodes = nodes;
                
                _nodes[0].Value.Node.Visit(this);

                type = _type;
                sequence = _sequence;
                return sequence is not null;
            }

            private Sequence<T> Type<T>() where T : INode
            {
                _type = typeof(ISequence<T>);
                
                T[] typedNodes = new T[_nodes.Length];

                for (int i = 0; i < _nodes.Length; i++)
                {
                    if (_nodes[i].Value.Node is not T typedNode)
                        return null;

                    typedNodes[i] = typedNode;
                }

                return new Sequence<T>(typedNodes);
            }
        }
    }
}
