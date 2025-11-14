using System;

namespace DiceRoll.Input.Parsing
{
    public sealed class ShuntingYardState
    {
        public int ParenthesisLevel { get; private set; }
        public InputMapper Mapper { get; }
        public MappedStack<Operator> Operators { get; }
        public MappedStack<LinkedNode> Operands { get; }
        public MappedStack<DelayedOperator> DelayedOperators { get; }
        public TokenKind PrecedingTokenKind { get; private set; }
        public OperatorInvocationHandler InvocationHandler { get; }

        public ShuntingYardState(OperandCastingTable castingTable)
        {
            ArgumentNullException.ThrowIfNull(castingTable);
            
            Mapper = new InputMapper();
            
            Operators = Mapper.CreateLinkedStack<Operator>();
            Operands = Mapper.CreateLinkedStack<LinkedNode>();
            DelayedOperators = Mapper.CreateLinkedStack<DelayedOperator>();

            InvocationHandler = new OperatorInvocationHandler(this, castingTable);

            PrecedingTokenKind = TokenKind.ExpressionStart;
            ParenthesisLevel = 0;
        }

        public Annotator Annotate() =>
            new(this);

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
