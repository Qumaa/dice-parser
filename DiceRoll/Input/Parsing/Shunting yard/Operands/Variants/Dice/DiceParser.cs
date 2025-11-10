using System;
using System.Runtime.InteropServices;

namespace DiceRoll.Input.Parsing
{
    internal sealed class DiceParser : FlatOperandParser
    {
        private readonly IToken _delimiter;
        private readonly CompositionDefinition[] _compositionTokens;
        
        public DiceParser(IToken delimiter, CompositionDefinition[] compositionTokens)
        {
            _delimiter = delimiter;
            _compositionTokens = compositionTokens;
        }

        public override INode Parse(in Substring expression)
        {
            Helper helper = StartParsing(expression);

            INumeric dice = Node.Value.Dice(helper.FacesCount());
            
            int diceCount = helper.DiceCount();

            return diceCount is 1 ?
                dice :
                helper.CompositionHandler().Invoke(dice, diceCount);
        }

        private Helper StartParsing(Substring expression) =>
            new(this, expression);

        [StructLayout(LayoutKind.Auto)]
        private readonly struct Helper
        {
            private readonly CompositionDefinition[] _compositionTokens;
            private readonly Substring _expression;
            private readonly Range _delimiter;
            private readonly int _diceNotationEnd;
            
            public Helper(DiceParser context, Substring expression)
            {
                _expression = expression;
                _compositionTokens = context._compositionTokens;
                _delimiter = FindDelimiter(in expression, context._delimiter).AsRange();
                _diceNotationEnd = IndexOfDiceNotationEnd(expression.SetStart(_delimiter.End.Value));
            }
            
            public int DiceCount()
            {
                int diceCount = 1;
            
                if (_delimiter.End.Value > _delimiter.Start.Value && _delimiter.Start.Value != _expression.Start)
                    diceCount = int.Parse(_expression.SetEnd(_delimiter.Start.Value).AsSpan());

                return diceCount;
            }
            
            public int FacesCount() =>
                int.Parse(_expression.SetRange(_delimiter.End.._diceNotationEnd).AsSpan());
            
            public CompositionHandler CompositionHandler()
            {
                if (!ExpressionEndsWithCompositionToken(out Substring compositionToken))
                    return DefaultCompositionHandler();

                foreach (CompositionDefinition token in _compositionTokens)
                    if (token.Token.MatchesAll(compositionToken))
                        return token.CompositionHandler;

                return DefaultCompositionHandler();
            }

            private bool ExpressionEndsWithCompositionToken(out Substring compositionToken)
            {
                if (_diceNotationEnd == _expression.End)
                {
                    compositionToken = default;
                    return false;
                }
                
                compositionToken = _expression.SetStart(_diceNotationEnd).Trim();
                return true;
            }
            
            private CompositionHandler DefaultCompositionHandler() =>
                _compositionTokens[0].CompositionHandler;
            
            private static Substring FindDelimiter(in Substring expression, IToken delimiter) =>
                delimiter.Matches(in expression, out Substring match) ? match : expression.Empty();

            private static int IndexOfDiceNotationEnd(in Substring postDelimiter)
            {
                for (int i = 0; i < postDelimiter.Length; i++)
                    if (!char.IsDigit(postDelimiter[i]))
                        return i + postDelimiter.Start;

                return postDelimiter.End;
            }
        }
    }
}
