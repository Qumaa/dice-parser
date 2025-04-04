using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace DiceRoll.Input
{
    public class DiceOperandBuilder
    {
        private readonly List<string> _delimiterTokens;
        private readonly List<DiceCompositionToken> _compositionTokens;

        private DiceOperandBuilder(List<string> delimiterTokens, IToken defaultComposition,
            DiceCompositionHandler compositionHandler)
        {
            _delimiterTokens = delimiterTokens;
            _compositionTokens = new List<DiceCompositionToken> { new(defaultComposition, compositionHandler) };
        }

        public DiceOperandBuilder(string defaultDelimiter, IToken defaultComposition, 
            DiceCompositionHandler compositionHandler) :
            this(new List<string> { defaultDelimiter }, defaultComposition, compositionHandler) { }

        public DiceOperandBuilder(IEnumerable<string> defaultDelimiters, IToken defaultComposition, 
            DiceCompositionHandler compositionHandler) :
            this(new List<string>(defaultDelimiters), defaultComposition, compositionHandler) { }

        public DiceOperandBuilder(string defaultDelimiter,
            string defaultComposition, DiceCompositionHandler compositionHandler) :
            this(defaultDelimiter, RegexToken.ExactIgnoreCase(defaultComposition),
                compositionHandler) { }
        public DiceOperandBuilder(IEnumerable<string> defaultDelimiters,
            string defaultComposition, DiceCompositionHandler compositionHandler) :
            this(defaultDelimiters, RegexToken.ExactIgnoreCase(defaultComposition),
                compositionHandler) { }

        public DiceOperandBuilder(string defaultDelimiter,
            IEnumerable<string> defaultComposition, DiceCompositionHandler compositionHandler) :
            this(defaultDelimiter, RegexToken.ExactIgnoreCase(defaultComposition),
                compositionHandler) { }
        public DiceOperandBuilder(IEnumerable<string> defaultDelimiters,
            IEnumerable<string> defaultComposition, DiceCompositionHandler compositionHandler) :
            this(defaultDelimiters, RegexToken.ExactIgnoreCase(defaultComposition),
                compositionHandler) { }

        public void AddDelimiter(string token)
        {
            if (_delimiterTokens.Contains(token))
                return;
            
            _delimiterTokens.Add(token);
        }

        public void AddDelimiter(char token) =>
            AddDelimiter(char.ToString(token));

        public void AddComposition(IToken token, DiceCompositionHandler handler) =>
            _compositionTokens.Add(new DiceCompositionToken(token, handler));
        public void AddComposition(string token, DiceCompositionHandler handler) =>
            AddComposition(RegexToken.ExactIgnoreCase(token), handler);
        public void AddComposition(IEnumerable<string> tokens, DiceCompositionHandler handler) =>
            AddComposition(RegexToken.ExactIgnoreCase(tokens), handler);

        public TokenizedOperand Build()
        {
            DiceParser parser = new(_delimiterTokens.ToArray(), _compositionTokens.ToArray());
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
                
                stringBuilder.AppendJoin(separator, _compositionTokens[i].Token.EnumerateRawTokens());
                
                stringBuilder.Append(')');

                if (i < _compositionTokens.Count - 1)
                    stringBuilder.Append(separator);
            }

            stringBuilder.Append(")?");

            return stringBuilder.ToString();
        }
    }
}
