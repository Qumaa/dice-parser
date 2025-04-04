using System.Collections.Generic;

namespace DiceRoll.Input
{
    public sealed class UnaryRPNOperatorParser<T> : RPNOperatorParser where T : INode
    {
        private readonly UnaryOperatorParseHandler<T> _handler;
        
        public UnaryRPNOperatorParser(UnaryOperatorParseHandler<T> handler) : base(1)
        {
            _handler = handler;
        }

        public override void TransformOperands(Stack<INode> operands) =>
            operands.Push(_handler.Invoke((T) operands.Pop()));
    }
}
