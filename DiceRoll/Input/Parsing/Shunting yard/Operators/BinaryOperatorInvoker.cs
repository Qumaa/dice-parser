namespace DiceRoll.Input.Parsing
{
    internal sealed class BinaryOperatorInvoker<TLeft, TRight> : OperatorInvoker
        where TLeft : INode where TRight : INode
    {
        private readonly BinaryInvocationHandler<TLeft, TRight> _handler;
        
        public BinaryOperatorInvoker(BinaryInvocationHandler<TLeft, TRight> handler) : base(1, 1)
        {
            _handler = handler;
        }

        public override INode Invoke(OperandsStackAccess operands)
        {
            TRight right = operands.Pop<TRight>();
            TLeft left = operands.Pop<TLeft>();
            
            return _handler.Invoke(left, right);
        }
    }
}
