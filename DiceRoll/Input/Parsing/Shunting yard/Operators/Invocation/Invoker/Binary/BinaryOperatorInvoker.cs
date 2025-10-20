namespace DiceRoll.Input.Parsing
{
    internal sealed class BinaryOperatorInvoker<TReturn, TLeft, TRight> : OperatorInvoker
        where TReturn : INode where TLeft : INode where TRight : INode
    {
        private readonly BinaryInvocationHandler<TReturn, TLeft, TRight> _handler;

        public BinaryOperatorInvoker(BinaryInvocationHandler<TReturn, TLeft, TRight> handler) : 
            base(Signature.Arguments<TLeft, TRight>().Returns<TReturn>())
        {
            _handler = handler;
        }

        public override INode Invoke(OperandsAccess operandsAccess)
        {
            operandsAccess.Sequential().Get(out TLeft left).Get(out TRight right);
            
            return _handler.Invoke(left, right);
        }
    }
}
