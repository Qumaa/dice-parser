using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DiceRoll.Input.Parsing
{
    public class TokensTableBuilder
    {
        private readonly List<string> _openParenthesis;
        private readonly List<string> _closeParenthesis;
        private readonly List<TokenizedOperator> _operators;
        private readonly List<TokenizedOperand> _operands;

        public TokensTableBuilder(string defaultOpenParenthesis, string defaultCloseParenthesis)
        {
            _openParenthesis = new List<string> { defaultOpenParenthesis };
            _closeParenthesis = new List<string> { defaultCloseParenthesis };

            _operators = new List<TokenizedOperator>();
            _operands = new List<TokenizedOperand>();
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

        public void AddOperatorToken<TLeft, TRight>(int precedence, BinaryInvocationHandler<TLeft, TRight> handler,
            Regex pattern) where TLeft : INode where TRight : INode =>
            _operators.Add(
                new TokenizedOperator(
                    new RegexToken(pattern),
                    precedence,
                    OperatorInvoker.Binary(handler)
                    )
                );

        public void AddOperatorToken<TLeft, TRight>(int precedence, BinaryInvocationHandler<TLeft, TRight> handler,
            IEnumerable<Regex> patterns) where TLeft : INode where TRight : INode =>
            _operators.Add(
                new TokenizedOperator(
                    new RegexToken(patterns),
                    precedence,
                    OperatorInvoker.Binary(handler)
                    )
                );

        public void AddOperatorToken<TLeft, TRight>(int precedence, BinaryInvocationHandler<TLeft, TRight> handler,
            params Regex[] patterns) where TLeft : INode where TRight : INode =>
            AddOperatorToken(precedence, handler, patterns as IEnumerable<Regex>);

        public void AddOperatorToken<TLeft, TRight>(int precedence, BinaryInvocationHandler<TLeft, TRight> handler,
            string word) where TLeft : INode where TRight : INode =>
            AddOperatorToken(precedence, handler, ExactIgnoreCase(word));

        public void AddOperatorToken<TLeft, TRight>(int precedence, BinaryInvocationHandler<TLeft, TRight> handler,
            IEnumerable<string> words) where TLeft : INode where TRight : INode =>
            AddOperatorToken(precedence, handler, words.Select(static x => ExactIgnoreCase(x)));

        public void AddOperatorToken<TLeft, TRight>(int precedence, BinaryInvocationHandler<TLeft, TRight> handler,
            params string[] words) where TLeft : INode where TRight : INode =>
            AddOperatorToken(precedence, handler, words as IEnumerable<string>);

        public void AddOperatorToken<T>(int precedence, UnaryInvocationHandler<T> handler,
            Regex pattern) where T : INode =>
            _operators.Add(
                new TokenizedOperator(
                    new RegexToken(pattern),
                    precedence,
                    OperatorInvoker.Unary(handler)
                    )
                );

        public void AddOperatorToken<T>(int precedence, UnaryInvocationHandler<T> handler,
            IEnumerable<Regex> patterns) where T : INode =>
            _operators.Add(
                new TokenizedOperator(
                    new RegexToken(patterns),
                    precedence,
                    OperatorInvoker.Unary(handler)
                    )
                );

        public void AddOperatorToken<T>(int precedence, UnaryInvocationHandler<T> handler,
            params Regex[] patterns) where T : INode =>
            AddOperatorToken(precedence, handler, patterns as IEnumerable<Regex>);

        public void AddOperatorToken<T>(int precedence, UnaryInvocationHandler<T> handler,
            string word) where T : INode =>
            AddOperatorToken(precedence, handler, ExactIgnoreCase(word));

        public void AddOperatorToken<T>(int precedence, UnaryInvocationHandler<T> handler,
            IEnumerable<string> words) where T : INode =>
            AddOperatorToken(precedence, handler, words.Select(static x => ExactIgnoreCase(x)));

        public void AddOperatorToken<T>(int precedence, UnaryInvocationHandler<T> handler,
            params string[] words) where T : INode =>
            AddOperatorToken(precedence, handler, words as IEnumerable<string>);

        public void AddOperandToken(TokenizedOperand operand) =>
            _operands.Add(operand);

        public void AddOperandToken(OperandHandler handler, Regex pattern) =>
            AddOperandToken(new TokenizedOperand(new RegexToken(pattern), handler));

        public void AddOperandToken(OperandHandler handler, IEnumerable<Regex> patterns) =>
            AddOperandToken(new TokenizedOperand(new RegexToken(patterns), handler));

        public void AddOperandToken(OperandHandler handler, params Regex[] patterns) =>
            AddOperandToken(handler, patterns as IEnumerable<Regex>);

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
