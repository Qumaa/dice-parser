using System.Runtime.InteropServices;

namespace DiceRoll.Input.Parsing
{
    [StructLayout(LayoutKind.Auto)]
    internal readonly struct DiceParser
    {
        private readonly IToken _delimiter;
        private readonly CompositionDefinition[] _compositionTokens;
        
        public DiceParser(IToken delimiter, CompositionDefinition[] compositionTokens)
        {
            _delimiter = delimiter;
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
            private readonly CompositionDefinition[] _compositionTokens;
            private readonly Substring _expression;
            private readonly Substring _delimiter;
            private readonly int _diceNotationEnd;
            
            public Helper(DiceParser context, Substring expression)
            {
                _expression = expression;
                _compositionTokens = context._compositionTokens;
                _delimiter = FindDelimiter(expression, context._delimiter);
                _diceNotationEnd = IndexOfDiceNotationEnd(expression, _delimiter.End);
            }
            
            public int DiceCount()
            {
                int diceCount = 1;
            
                if (_delimiter is { IsEmpty: false, Start: > 0 })
                    diceCount = int.Parse(_expression[.._delimiter.RelativeStart(in _expression)].AsSpan());

                return diceCount;
            }
            
            public int FacesCount() =>
                int.Parse(_expression[_delimiter.RelativeEnd(in _expression).._diceNotationEnd].AsSpan());
            
            public CompositionHandler CompositionHandler()
            {
                if (!ExpressionEndsWithCompositionToken(out Substring compositionToken))
                    return DefaultCompositionHandler();

                foreach (CompositionDefinition token in _compositionTokens)
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
            
            private CompositionHandler DefaultCompositionHandler() =>
                _compositionTokens[0].CompositionHandler;
            
            private static Substring FindDelimiter(in Substring expression, IToken delimiter) =>
                delimiter.Matches(in expression, out Substring match) ? match : Substring.Empty(in expression);

            private static int IndexOfDiceNotationEnd(in Substring expression, int delimiterEndIndex)
            {
                int notationEnd = expression.Length;
            
                for (int i = delimiterEndIndex; i < notationEnd; i++)
                    if (!char.IsDigit(expression[i]))
                        notationEnd = i;

                return notationEnd;
            }
        }
    }
}
