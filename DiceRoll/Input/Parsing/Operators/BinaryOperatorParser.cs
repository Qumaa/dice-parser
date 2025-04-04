using System.Collections.Generic;

namespace DiceRoll.Input
{
    public sealed class BinaryOperatorParser<TLeft, TRight> : OperatorParser where TLeft : INode where TRight : INode
    {
        private readonly BinaryParsingHandler<TLeft, TRight> _handler;
        
        public BinaryOperatorParser(BinaryParsingHandler<TLeft, TRight> handler) : base(2)
        {
            _handler = handler;
        }

        public override void TransformOperands(Stack<INode> operands)
        {
            TRight right = (TRight) operands.Pop();
            TLeft left = (TLeft) operands.Pop();
            
            operands.Push(_handler.Invoke(left, right));
        }
    }
}
