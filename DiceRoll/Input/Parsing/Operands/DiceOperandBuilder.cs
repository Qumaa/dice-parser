using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DiceRoll.Input.Parsing
{
    public class DiceOperandBuilder
    {
        private readonly List<string> _delimiterTokens;
        private readonly List<RawCompositionToken> _compositionTokens;

        private DiceOperandBuilder(List<string> delimiterTokens, IEnumerable<string> defaultComposition,
            DiceCompositionHandler compositionHandler)
        {
            _delimiterTokens = delimiterTokens;
            _compositionTokens = new List<RawCompositionToken> { new(defaultComposition, compositionHandler) };
        }

        public DiceOperandBuilder(IEnumerable<string> defaultDelimiters, IEnumerable<string> defaultComposition, 
            DiceCompositionHandler compositionHandler) :
            this(new List<string>(defaultDelimiters), defaultComposition, compositionHandler) { }

        public DiceOperandBuilder(string defaultDelimiter, IEnumerable<string> defaultComposition, 
            DiceCompositionHandler compositionHandler) :
            this(new List<string> { defaultDelimiter }, defaultComposition, compositionHandler) { }

        public DiceOperandBuilder(IEnumerable<string> defaultDelimiters,
            string defaultComposition, DiceCompositionHandler compositionHandler) :
            this(new List<string>(defaultDelimiters), ToEnumerable(defaultComposition), compositionHandler) { }

        public DiceOperandBuilder(string defaultDelimiter,
            string defaultComposition, DiceCompositionHandler compositionHandler) :
            this(new List<string> { defaultDelimiter }, ToEnumerable(defaultComposition), compositionHandler) { }

        public void AddDelimiter(string token)
        {
            if (_delimiterTokens.Contains(token))
                return;
            
            _delimiterTokens.Add(token);
        }

        public void AddDelimiter(char token) =>
            AddDelimiter(char.ToString(token));

        public void AddComposition(IEnumerable<string> tokens, DiceCompositionHandler handler) =>
            _compositionTokens.Add(new RawCompositionToken(tokens, handler));

        public void AddComposition(string token, DiceCompositionHandler handler) =>
            AddComposition(new[] { token }, handler);

        public TokenizedOperand Build()
        {
            DiceParser parser = new(_delimiterTokens.ToArray(), _compositionTokens.Select(x => x.Convert()).ToArray());
            return new TokenizedOperand(CreateToken(), diceExpression => parser.Parse(diceExpression));
        }

        private IToken CreateToken()
        {
            string regexPattern = BuildRegexPattern();
            Regex patterns = new(regexPattern, RegexOptions.IgnoreCase);
            return new RegexToken(patterns);
        }

        private string BuildRegexPattern()
        {
            const char separator = '|';
            
            // @"(?:(\d+)(delimiter1|...)(\d+)|(delimiter1|...)(\d+))\s*((composition1|...)|...)?"
            StringBuilder stringBuilder = new(64);
            stringBuilder.Append(@"(?:(\d+)(");

            stringBuilder.AppendJoin(separator, _delimiterTokens);

            stringBuilder.Append(@")(\d+)|(");
            
            stringBuilder.AppendJoin(separator, _delimiterTokens);

            stringBuilder.Append(@")(\d+))\s*(");

            for (int i = 0; i < _compositionTokens.Count; i++)
            {
                stringBuilder.Append('(');
                
                stringBuilder.AppendJoin(separator, _compositionTokens[i].Tokens);
                
                stringBuilder.Append(')');

                if (i < _compositionTokens.Count - 1)
                    stringBuilder.Append(separator);
            }

            stringBuilder.Append(")?");

            return stringBuilder.ToString();
        }

        private static IEnumerable<T> ToEnumerable<T>(T value) =>
            new[] { value };

        private readonly struct RawCompositionToken
        {
            public readonly IEnumerable<string> Tokens;
            public readonly DiceCompositionHandler CompositionHandler;
            
            public RawCompositionToken(IEnumerable<string> tokens, DiceCompositionHandler compositionHandler)
            {
                Tokens = tokens;
                CompositionHandler = compositionHandler;
            }

            public DiceCompositionToken Convert() =>
                new(RegexToken.ExactIgnoreCase(Tokens), CompositionHandler);
        }
    }
}
