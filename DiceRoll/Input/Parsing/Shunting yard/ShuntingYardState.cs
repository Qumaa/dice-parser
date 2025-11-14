using System;

namespace DiceRoll.Input.Parsing
{
    public sealed class ShuntingYardState
    {
        internal int ParenthesisLevel { get; private set; }
        internal InputMapper Mapper { get; }
        internal MappedStack<Operator> Operators { get; }
        internal MappedStack<LinkedNode> Operands { get; }
        internal MappedStack<DelayedOperator> DelayedOperators { get; }
        internal TokenKind PrecedingTokenKind { get; private set; }
        internal OperatorInvocationHandler InvocationHandler { get; }

        public ShuntingYardState(OperandCastingTable castingTable)
        {
            Mapper = new InputMapper();
            
            Operators = Mapper.CreateLinkedStack<Operator>();
            Operands = Mapper.CreateLinkedStack<LinkedNode>();
            DelayedOperators = Mapper.CreateLinkedStack<DelayedOperator>();

            InvocationHandler = new OperatorInvocationHandler(this, castingTable);

            PrecedingTokenKind = TokenKind.ExpressionStart;
            ParenthesisLevel = 0;
        }

        internal Annotator Annotate() =>
            new(this);

        internal void MapAndThrow(in Range context, string message) =>
            throw new ParsingException(Mapper.GetSubstringOf(in context), message);

        internal void MapAndThrow<T>(in Mapped<T> context, string message) =>
            MapAndThrow(in context.Range, message);

        internal ParsingException MapException(in Substring context, Exception innerException) =>
            new(Mapper.MapAndGetSubstringOf(in context), innerException);

        internal readonly ref struct Annotator
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
