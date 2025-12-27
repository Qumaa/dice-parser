using System;

namespace DiceRoll.Input.Parsing
{
    public abstract class OperandParser
    {
        public abstract INode Parse(in Substring operandSubstring);
        
        public static OperandParser FromDelegate(OperandParsingHandler handler) =>
            new DelegateImplementation(handler);

        private sealed class DelegateImplementation : OperandParser
        {
            private readonly OperandParsingHandler _handler;

            public DelegateImplementation(OperandParsingHandler handler)
            {
                ArgumentNullException.ThrowIfNull(handler);
                
                _handler = handler;
            }

            public override INode Parse(in Substring operandSubstring) =>
                _handler(operandSubstring);
        }
    }
}
