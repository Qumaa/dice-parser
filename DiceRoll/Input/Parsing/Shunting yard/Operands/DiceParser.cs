using System;
using System.Runtime.InteropServices;

namespace DiceRoll.Input.Parsing
{
    [StructLayout(LayoutKind.Auto)]
    public readonly struct DiceParser
    {
        private readonly string[] _delimiters;
        private readonly DiceCompositionToken[] _compositionTokens;
        
        public DiceParser(string[] delimiters, DiceCompositionToken[] compositionTokens)
        {
            _delimiters = delimiters;
            _compositionTokens = compositionTokens;
        }

        public INumeric Parse(Substring expression)
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
            private readonly DiceCompositionToken[] _compositionTokens;
            private readonly Substring _expression;
            private readonly int _delimiterIndex;
            private readonly int _diceNotationEnd;
            
            public Helper(DiceParser context, Substring expression)
            {
                _expression = expression;
                _compositionTokens = context._compositionTokens;
                _delimiterIndex = IndexOfDelimiter(expression, context._delimiters);
                _diceNotationEnd = IndexOfDiceNotationEnd(expression, _delimiterIndex);
            }
            
            public int DiceCount()
            {
                int diceCount = 1;
            
                if (_delimiterIndex is not 0)
                    diceCount = int.Parse(_expression[.._delimiterIndex].AsSpan());

                return diceCount;
            }
            
            public int FacesCount() =>
                int.Parse(_expression.AsSpan(_delimiterIndex + 1, _diceNotationEnd - _delimiterIndex - 1));
            
            public DiceCompositionHandler CompositionHandler()
            {
                if (!ExpressionEndsWithCompositionToken(out Substring compositionToken))
                    return DefaultCompositionHandler();

                foreach (DiceCompositionToken token in _compositionTokens)
                    if (token.Token.Matches(compositionToken))
                        return token.CompositionHandler;

                return DefaultCompositionHandler();
            }

            private bool ExpressionEndsWithCompositionToken(out Substring compositionToken)
            {
                if (_diceNotationEnd == _expression.Length)
                {
                    compositionToken = default;
                    return false;
                }
                
                compositionToken = _expression[_diceNotationEnd..].Trim();
                return true;
            }
            
            private DiceCompositionHandler DefaultCompositionHandler() =>
                _compositionTokens[0].CompositionHandler;
            
            private static int IndexOfDelimiter(Substring expression, string[] delimiters)
            {
                foreach (string delimiter in delimiters)
                {
                    int index = expression.IndexOf(delimiter, StringComparison.OrdinalIgnoreCase);

                    if (index >= 0)
                        return index;
                }

                return -1;
            }
            
            private static int IndexOfDiceNotationEnd(Substring expression, int delimiterIndex)
            {
                int notationEnd = expression.Length;
            
                for (int i = delimiterIndex + 1; i < notationEnd; i++)
                    if (!char.IsDigit(expression[i]))
                        notationEnd = i;

                return notationEnd;
            }
        }
    }
}
