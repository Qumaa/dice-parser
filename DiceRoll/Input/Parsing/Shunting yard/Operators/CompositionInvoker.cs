namespace DiceRoll.Input.Parsing
{
    public sealed class CompositionInvoker : OperatorInvoker
    {
        private readonly CompositionHandler _handler;
        
        public CompositionInvoker(CompositionHandler handler) : base(2, ArgumentsLayout.FullLeft)
        {
            _handler = handler;
        }
        
        public override void Invoke(OperandsStackAccess operands)
        {
            INumeric node = operands.Pop<INumeric>();
            int times = operands.Pop<INumeric>().Evaluate().Value;
            
            operands.PushResult(_handler(node, times));
        }

        public static CompositionInvoker Factory<T>() where T : Composer, new() =>
            new(Node.Value.Composite<T>);
    }
}
