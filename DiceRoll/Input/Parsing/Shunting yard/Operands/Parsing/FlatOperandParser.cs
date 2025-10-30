using System;

namespace DiceRoll.Input.Parsing
{
    public abstract class FlatOperandParser : OperandParser
    {
        public abstract INode Parse(in Substring operandSubstring);
        
        public new static FlatOperandParser FromDelegate(FlatOperandParsingHandler handler) =>
            new DelegateImplementation(handler);

        private sealed class DelegateImplementation : FlatOperandParser
        {
            private readonly FlatOperandParsingHandler _handler;

            public DelegateImplementation(FlatOperandParsingHandler handler)
            {
                ArgumentNullException.ThrowIfNull(handler);
                
                _handler = handler;
            }

            public override INode Parse(in Substring operandSubstring) =>
                _handler(operandSubstring);
        }
    }
}
