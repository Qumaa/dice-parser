using System;

namespace DiceRoll.Input.Parsing
{
    internal sealed class ShuntingYardState
    {
        public int ParenthesisLevel { get; private set; }
        public InputMapper Mapper { get; }
        public MappedStack<Operator> Operators { get; }
        public MappedStack<LinkedNode> Operands { get; }
        public MappedStack<DelayedOperator> DelayedOperators { get; }
        public TokenKind PrecedingTokenKind { get; private set; }

        public bool ClosingParenthesisWouldImposeImbalance => ParenthesisLevel is 0;

        public ShuntingYardState()
        {
            Mapper = new InputMapper();
            
            Operators = Mapper.CreateLinkedStack<Operator>();
            Operands = Mapper.CreateLinkedStack<LinkedNode>();
            DelayedOperators = Mapper.CreateLinkedStack<DelayedOperator>();

            PrecedingTokenKind = TokenKind.ExpressionStart;
            ParenthesisLevel = 0;
        }

        public Annotator Annotate() =>
            new(this);

        public void MapAndThrow(in Range context, string message) =>
            throw new ParsingException(Mapper.GetSubstringOf(in context), message);

        public void MapAndThrow<T>(in Mapped<T> context, string message) =>
            MapAndThrow(in context.Range, message);

        public void MapAndThrow(in Substring context, string message) =>
            throw new ParsingException(Mapper.MapAndGetSubstringOf(in context), message);

        public ParsingException MapException(in Range context, Exception innerException) =>
            new(Mapper.GetSubstringOf(in context), innerException);

        public ParsingException MapException<T>(in Mapped<T> context, Exception innerException) =>
            MapException(in context.Range, innerException);

        public ParsingException MapException(in Substring context, Exception innerException) =>
            new(Mapper.MapAndGetSubstringOf(in context), innerException);

        public readonly ref struct Annotator
        {
            private readonly ShuntingYardState _context;
            
            public Annotator(ShuntingYardState context)
            {
                _context = context;
            }
            
            public Annotator ParenthesisOpening()
            {
                _context.ParenthesisLevel++;
                return NewExpressionStart();
            }

            public Annotator ParenthesisClosing()
            {
                _context.ParenthesisLevel--;
                return OperandProcessing();
            }

            public Annotator NewExpressionStart()
            {
                _context.PrecedingTokenKind = TokenKind.ExpressionStart;
                return this;
            }

            public Annotator OperatorProcessing()
            {
                _context.PrecedingTokenKind = TokenKind.Operator;
                return this;
            }
        
            public Annotator OperandProcessing()
            {
                _context.PrecedingTokenKind = TokenKind.Operand;
                return this;
            }
        }
    }
}
