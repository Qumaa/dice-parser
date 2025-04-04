using System.Collections.Generic;

namespace DiceRoll.Input
{
    public sealed class UnaryOperatorParser<T> : OperatorParser where T : INode
    {
        private readonly UnaryParsingHandler<T> _handler;
        
        public UnaryOperatorParser(UnaryParsingHandler<T> handler) : base(1)
        {
            _handler = handler;
        }

        public override void TransformOperands(Stack<INode> operands) =>
            operands.Push(_handler.Invoke((T) operands.Pop()));
    }
}
