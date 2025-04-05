using System.Collections.Generic;

namespace DiceRoll.Input
{
    public sealed class BinaryOperatorInvoker<TLeft, TRight> : RPNOperatorInvoker where TLeft : INode where TRight : INode
    {
        private readonly BinaryInvocationHandler<TLeft, TRight> _handler;
        
        public BinaryOperatorInvoker(BinaryInvocationHandler<TLeft, TRight> handler) : base(2)
        {
            _handler = handler;
        }

        public override void Invoke(Stack<INode> operands)
        {
            TRight right = (TRight) operands.Pop();
            TLeft left = (TLeft) operands.Pop();
            
            operands.Push(_handler.Invoke(left, right));
        }
    }
}
