namespace DiceRoll.Input
{
    internal sealed class BinaryOperatorInvoker<TLeft, TRight> : OperatorInvoker
        where TLeft : INode where TRight : INode
    {
        private readonly BinaryInvocationHandler<TLeft, TRight> _handler;
        
        public BinaryOperatorInvoker(BinaryInvocationHandler<TLeft, TRight> handler) : base(2)
        {
            _handler = handler;
        }

        public override void Invoke(OperandsStackAccess operands)
        {
            TRight right = operands.Pop<TRight>();
            TLeft left = operands.Pop<TLeft>();
            
            operands.PushResult(_handler.Invoke(left, right));
        }
    }
}
