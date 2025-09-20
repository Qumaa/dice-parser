namespace DiceRoll.Input.Parsing
{
    public sealed class CompositionInvoker : OperatorInvoker
    {
        private readonly CompositionHandler _handler;

        public CompositionInvoker(CompositionHandler handler) : 
            base(Signature.Define<INumeric, INumeric>().Returns<INumeric>())
        {
            _handler = handler;
        }
        
        public override INode Invoke(OperandsAccess operandsAccess)
        {
            operandsAccess.Get(1, out INumeric node).Get(0, out INumeric times);
            
            return _handler(node, times.Evaluate().Value);
        }
    }
}
