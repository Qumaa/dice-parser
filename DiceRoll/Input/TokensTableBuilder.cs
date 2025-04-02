using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DiceRoll.Input
{
    public class TokensTableBuilder
    {
        private readonly List<string> _openParenthesis;
        private readonly List<string> _closeParenthesis;
        private readonly List<TokenizedOperator> _operators;
        private readonly List<IToken> _operands;

        public TokensTableBuilder(string defaultOpenParenthesis, string defaultCloseParenthesis)
        {
            _openParenthesis = new List<string> { defaultOpenParenthesis };
            _closeParenthesis = new List<string> { defaultCloseParenthesis };

            _operators = new List<TokenizedOperator>();
            _operands = new List<IToken>();
        }

        public void AddOpenParenthesisPattern(string pattern) =>
            _openParenthesis.Add(pattern);
        public void AddOpenParenthesisPattern(IEnumerable<string> patterns) =>
            _openParenthesis.AddRange(patterns);
        public void AddOpenParenthesisPattern(params string[] patterns) =>
            _openParenthesis.AddRange(patterns);

        public void AddCloseParenthesisPattern(string pattern) =>
            _closeParenthesis.Add(pattern);
        public void AddCloseParenthesisPattern(IEnumerable<string> patterns) =>
            _closeParenthesis.AddRange(patterns);
        public void AddCloseParenthesisPattern(params string[] patterns) =>
            _closeParenthesis.AddRange(patterns);

        public void AddOperatorToken(int precedence, Regex pattern) =>
            _operators.Add(new TokenizedOperator(new RegexToken(pattern), precedence));
        public void AddOperatorToken(int precedence, IEnumerable<Regex> patterns) =>
            _operators.Add(new TokenizedOperator(new RegexToken(patterns), precedence));
        public void AddOperatorToken(int precedence, params Regex[] patterns) =>
            AddOperatorToken(precedence, patterns as IEnumerable<Regex>);
        public void AddOperatorToken(int precedence, string word) =>
            AddOperatorToken(precedence, ExactIgnoreCase(word));
        public void AddOperatorToken(int precedence, IEnumerable<string> words) =>
            AddOperatorToken(precedence, words.Select(static x => ExactIgnoreCase(x)));
        public void AddOperatorToken(int precedence, params string[] words) =>
            AddOperatorToken(precedence, words as IEnumerable<string>);
        public void AddOperandToken(Regex pattern) =>
            _operands.Add(new RegexToken(pattern));
        public void AddOperandToken(IEnumerable<Regex> patterns) =>
            _operands.Add(new RegexToken(patterns));
        public void AddOperandToken(params Regex[] patterns) =>
            AddOperandToken(patterns as IEnumerable<Regex>);

        public TokensTable Build() =>
            new(
                RegexToken.ExactIgnoreCase(_openParenthesis),
                RegexToken.ExactIgnoreCase(_closeParenthesis),
                _operators,
                _operands
            );

        private static Regex ExactIgnoreCase(string word) =>
            RegexToken.CreateExactIgnoreCaseRegex(word);
    }
}
