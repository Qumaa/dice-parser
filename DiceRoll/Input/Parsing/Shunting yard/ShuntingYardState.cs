using System;

namespace DiceRoll.Input.Parsing
{
    internal sealed class ShuntingYardState
    {
        public TokensTable Tokens { get; }
        public int ParenthesisLevel { get; private set; }
        public InputMapper Mapper { get; }
        public MappedStack<OperatorToken> Operators { get; }
        public MappedStack<LinkedNode> Operands { get; }
        public MappedStack<DelayedOperatorToken> DelayedOperators { get; }
        public TokenKind PrecedingTokenKind { get; private set; }

        public bool ClosingParenthesisWouldImposeImbalance => ParenthesisLevel is 0;

        public ShuntingYardState(TokensTable tokensTable)
        {
            Tokens = tokensTable;
            
            Mapper = new InputMapper();
            
            Operators = Mapper.CreateLinkedStack<OperatorToken>();
            Operands = Mapper.CreateLinkedStack<LinkedNode>();
            DelayedOperators = Mapper.CreateLinkedStack<DelayedOperatorToken>();

            PrecedingTokenKind = TokenKind.ExpressionStart;
            ParenthesisLevel = 0;
        }

        public Annotator Annotate() =>
            new(this);
        
        public void MapAndThrow<T>(in Mapped<T> context, string message) =>
            throw new ParsingException(Mapper.GetSubstringOf(in context), message);
        
        public void MapAndThrow(in Substring context, string message) =>
            throw new ParsingException(Mapper.MapAndGetSubstringOf(in context), message);

        public ParsingException MapException<T>(in Mapped<T> context, Exception innerException) =>
            new(Mapper.GetSubstringOf(in context), innerException);
        
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
