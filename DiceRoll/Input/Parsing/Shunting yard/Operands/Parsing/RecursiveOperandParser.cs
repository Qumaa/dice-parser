using System;

namespace DiceRoll.Input.Parsing
{
    public abstract class RecursiveOperandParser : OperandParser
    {
        public abstract INode Parse(in Substring operandSubstring, FlatOperandParser parser);
        
        public new static RecursiveOperandParser FromDelegate(RecursiveOperandParsingHandler handler) =>
            new DelegateImplementation(handler);
        
        private sealed class DelegateImplementation : RecursiveOperandParser
        {
            private readonly RecursiveOperandParsingHandler _handler;

            public DelegateImplementation(RecursiveOperandParsingHandler handler)
            {
                ArgumentNullException.ThrowIfNull(handler);
                
                _handler = handler;
            }

            public override INode Parse(in Substring operandSubstring, FlatOperandParser parser) =>
                _handler(operandSubstring, parser);
        }
    }
}
