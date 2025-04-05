using System.Collections.Generic;

namespace DiceRoll.Input
{
    public sealed class UnaryOperatorInvoker<T> : RPNOperatorInvoker where T : INode
    {
        private readonly UnaryInvocationHandler<T> _handler;
        
        public UnaryOperatorInvoker(UnaryInvocationHandler<T> handler) : base(1)
        {
            _handler = handler;
        }

        public override void Invoke(Stack<INode> operands) =>
            operands.Push(_handler.Invoke((T) operands.Pop()));
    }
}
