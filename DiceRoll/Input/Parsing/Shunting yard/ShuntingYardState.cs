using System;

namespace DiceRoll.Input
{
    internal sealed class ShuntingYardState
    {
        public TokensTable Tokens { get; }
        public int ParenthesisLevel { get; private set; }
        public FormulaAccumulator Accumulator { get; }
        public FormulaTokensStack<OperatorToken> Operators { get; }
        public FormulaTokensStack<INode> Operands { get; }
        public FormulaTokensStack<DelayedOperatorToken> DelayedOperators { get; }
        public TokenKind PrecedingTokenKind { get; private set; }

        public bool ClosingParenthesisWouldImposeImbalance => ParenthesisLevel is 0;

        public ShuntingYardState(TokensTable tokensTable)
        {
            Tokens = tokensTable;
            
            Accumulator = new FormulaAccumulator();
            
            Operators = Accumulator.CreateStack<OperatorToken>();
            Operands = Accumulator.CreateStack<INode>();
            DelayedOperators = Accumulator.CreateStack<DelayedOperatorToken>();

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
        
        public void Throw<T>(in FormulaToken<T> context, string message) =>
            throw new ParsingException(Accumulator.GetFormulaSubstring(in context), message);
        
        public void Throw(in Substring context, string message) =>
            throw new ParsingException(Accumulator.AppendAndGetSubstring(in context), message);

        public ParsingException Wrap<T>(in FormulaToken<T> context, Exception innerException) =>
            new(Accumulator.GetFormulaSubstring(in context), innerException);
        
        public ParsingException Wrap(in Substring context, Exception innerException) =>
            new(Accumulator.AppendAndGetSubstring(in context), innerException);
    }
}
