namespace DiceRoll.Input.Parsing
{
    internal sealed class CompositionInvoker : OperatorInvoker
    {
        private readonly CompositionHandler _handler;

        public CompositionInvoker(CompositionHandler handler) : 
            base(Signature.Arguments<INumeric, INumeric>().Returns<INumeric>(), 2, 0)
        {
            _handler = handler;
        }
        
        public override INode Invoke(OperandsAccess operandsAccess)
        {
            operandsAccess.Sequential().Get(out INumeric times).Get(out INumeric node);
            
            return _handler(node, times.Evaluate());
        }
    }
}
