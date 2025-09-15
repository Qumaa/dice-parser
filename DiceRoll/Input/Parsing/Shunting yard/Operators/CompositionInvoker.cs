namespace DiceRoll.Input.Parsing
{
    public sealed class CompositionInvoker : OperatorInvoker
    {
        private readonly CompositionHandler _handler;

        public CompositionInvoker(CompositionHandler handler) : base(2, 0)
        {
            _handler = handler;
        }
        
        public override INode Invoke(OperandsStackAccess operands)
        {
            INumeric node = operands.Pop<INumeric>();
            int times = operands.Pop<INumeric>().Evaluate().Value;
            
            return _handler(node, times);
        }
    }
}
