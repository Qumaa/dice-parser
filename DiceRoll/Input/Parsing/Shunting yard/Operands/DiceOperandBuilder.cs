using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DiceRoll.Input.Parsing
{
    public class DiceOperandBuilder
    {
        private readonly List<string> _delimiterTokens;
        private readonly List<CompositionTokenDescriptor> _compositionTokens;
        
        public DiceOperandBuilder(IEnumerable<string> delimiterTokens, in CompositionTokenDescriptor defaultComposition)
        {
            _delimiterTokens = new List<string>(delimiterTokens);
            _compositionTokens = new List<CompositionTokenDescriptor> { defaultComposition };
        }
        public DiceOperandBuilder(string delimiterToken, in CompositionTokenDescriptor defaultComposition)
        {
            _delimiterTokens = new List<string> { delimiterToken };
            _compositionTokens = new List<CompositionTokenDescriptor> { defaultComposition };
        }

        public DiceOperandBuilder AddDelimiter(string token)
        {
            if (!_delimiterTokens.Contains(token))
                _delimiterTokens.Add(token);

            return this;
        }

        public DiceOperandBuilder AddDelimiter(char token) =>
            AddDelimiter(char.ToString(token));

        public DiceOperandBuilder AddComposition(in CompositionTokenDescriptor descriptor)
        {
            _compositionTokens.Add(descriptor);
            return this;
        }

        public DiceOperandBuilder AddComposition(IEnumerable<string> tokens, CompositionHandler handler) =>
            AddComposition(new CompositionTokenDescriptor(tokens, handler));

        public DiceOperandBuilder AddComposition(string token, CompositionHandler handler) =>
            AddComposition(new[] { token }, handler);

        public Operand Build()
        {
            DiceParser parser = new(_delimiterTokens.ToArray(), _compositionTokens.Select(x => x.Convert()).ToArray());
            return new Operand(CreateToken(), diceExpression => parser.Parse(diceExpression));
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
            
            // @"(?:(\d+)(delimiter1|...)(\d+)|(delimiter1|...)(\d+))((composition1|...)|...)?"
            StringBuilder stringBuilder = new(64);
            stringBuilder.Append(@"(?:(\d+)(");

            stringBuilder.AppendJoin(separator, _delimiterTokens);

            stringBuilder.Append(@")(\d+)|(");
            
            stringBuilder.AppendJoin(separator, _delimiterTokens);

            stringBuilder.Append(@")(\d+))(");

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
    }
}
