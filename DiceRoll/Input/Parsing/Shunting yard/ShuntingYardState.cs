using System;

namespace DiceRoll.Input
{
    internal sealed class ShuntingYardState
    {
        public TokensTable Tokens { get; }
        public int ParenthesisLevel { get; private set; }
        public InputMapper Mapper { get; }
        public MappedStack<OperatorToken> Operators { get; }
        public MappedStack<INode> Operands { get; }
        public MappedStack<DelayedOperatorToken> DelayedOperators { get; }
        public TokenKind PrecedingTokenKind { get; private set; }

        public bool ClosingParenthesisWouldImposeImbalance => ParenthesisLevel is 0;

        public ShuntingYardState(TokensTable tokensTable)
        {
            Tokens = tokensTable;
            
            Mapper = new InputMapper();
            
            Operators = Mapper.CreateLinkedStack<OperatorToken>();
            Operands = Mapper.CreateLinkedStack<INode>();
            DelayedOperators = Mapper.CreateLinkedStack<DelayedOperatorToken>();

            PrecedingTokenKind = TokenKind.ExpressionStart;
            ParenthesisLevel = 0;
        }

        public void DenoteParenthesisOpening() =>
            ParenthesisLevel++;

        public void DenoteParenthesisClosing() =>
            ParenthesisLevel--;

        public void DenoteNewExpressionStart() =>
            PrecedingTokenKind = TokenKind.ExpressionStart;

        public void DenoteOperatorProcessing() =>
            PrecedingTokenKind = TokenKind.Operator;
        
        public void DenoteOperandProcessing() =>
            PrecedingTokenKind = TokenKind.Operand;
        
        public void Throw<T>(in Mapped<T> context, string message) =>
            throw new ParsingException(Mapper.GetSubstringOf(in context), message);
        
        public void Throw(in Substring context, string message) =>
            throw new ParsingException(Mapper.MapAndGetSubstringOf(in context), message);

        public ParsingException Wrap<T>(in Mapped<T> context, Exception innerException) =>
            new(Mapper.GetSubstringOf(in context), innerException);
        
        public ParsingException Wrap(in Substring context, Exception innerException) =>
            new(Mapper.MapAndGetSubstringOf(in context), innerException);
    }
}
