using System;
using System.Collections.Generic;

namespace DiceRoll.Input.Parsing
{
    public class TokensTableBuilder
    {
        private readonly List<IToken> _openParenthesis;
        private readonly List<IToken> _closeParenthesis;
        private readonly List<OperatorDefinition> _operators;
        private readonly List<OperandDefinition> _operands;

        public TokensTableBuilder(IToken defaultOpenParenthesis, IToken defaultCloseParenthesis)
        {
            ArgumentNullException.ThrowIfNull(defaultOpenParenthesis);
            ArgumentNullException.ThrowIfNull(defaultCloseParenthesis);
            
            _openParenthesis = new List<IToken> { defaultOpenParenthesis };
            _closeParenthesis = new List<IToken> { defaultCloseParenthesis };

            _operators = new List<OperatorDefinition>();
            _operands = new List<OperandDefinition>();
        }

        public TokensTableBuilder OpenParenthesis(IToken token)
        {
            _openParenthesis.Add(token);
            return this;
        }

        public TokensTableBuilder CloseParenthesis(IToken token)
        {
            _closeParenthesis.Add(token);
            return this;
        }

        public TokensTableBuilder Operator(in OperatorDefinition definition)
        {
            _operators.Add(definition);
            return this;
        }

        public TokensTableBuilder Operand(in OperandDefinition definition)
        {
            _operands.Add(definition);
            return this;
        }

        public TokensTable Build() =>
            new(
                _openParenthesis.ToCompositeToken(),
                _closeParenthesis.ToCompositeToken(),
                _operators,
                _operands
                );
    }

    public static class TokensTableBuilderExtensions
    {
        public static TokensTableBuilder OpenParenthesis(this TokensTableBuilder builder, IEnumerable<IToken> tokens)
        {
            foreach (IToken token in tokens)
                builder.OpenParenthesis(token);
            
            return builder;
        }
        
        public static TokensTableBuilder CloseParenthesis(this TokensTableBuilder builder, IEnumerable<IToken> tokens)
        {
            foreach (IToken token in tokens)
                builder.CloseParenthesis(token);
            
            return builder;
        }

        public static TokensTableBuilder Operator(this TokensTableBuilder builder, int precedence,
            OperatorInvoker invoker, IToken token) =>
            builder.Operator(new OperatorDefinition(token, precedence, invoker));

        public static TokensTableBuilder Operator(this TokensTableBuilder builder, int precedence,
            OperatorInvoker invoker, IEnumerable<IToken> tokens) =>
            builder.Operator(precedence, invoker, tokens.ToCompositeToken());

        public static TokensTableBuilder BinaryOperator<TLeft, TRight>(this TokensTableBuilder builder, int precedence, 
            BinaryInvocationHandler<TLeft, TRight> handler, IToken token) where TLeft : INode where TRight : INode =>
            builder.Operator(precedence, OperatorInvoker.Binary(handler), token);

        public static TokensTableBuilder BinaryOperator<TLeft, TRight>(this TokensTableBuilder builder, int precedence,
            BinaryInvocationHandler<TLeft, TRight> handler, IEnumerable<IToken> tokens)
            where TLeft : INode where TRight : INode =>
            builder.BinaryOperator(precedence, handler, tokens.ToCompositeToken());

        public static TokensTableBuilder PrefixUnaryOperator<T>(this TokensTableBuilder builder, int precedence, 
            UnaryInvocationHandler<T> handler, IToken token) where T : INode =>
            builder.Operator(precedence, OperatorInvoker.PrefixUnary(handler), token);

        public static TokensTableBuilder PrefixUnaryOperator<T>(this TokensTableBuilder builder, int precedence, 
            UnaryInvocationHandler<T> handler, IEnumerable<IToken> tokens) where T : INode =>
            builder.PrefixUnaryOperator(precedence, handler, tokens.ToCompositeToken());

        public static TokensTableBuilder PostfixUnaryOperator<T>(this TokensTableBuilder builder, int precedence, 
            UnaryInvocationHandler<T> handler, IToken token) where T : INode =>
            builder.Operator(precedence, OperatorInvoker.PostfixUnary(handler), token);

        public static TokensTableBuilder PostfixUnaryOperator<T>(this TokensTableBuilder builder, int precedence, 
            UnaryInvocationHandler<T> handler, IEnumerable<IToken> tokens) where T : INode =>
            builder.PostfixUnaryOperator(precedence, handler, tokens.ToCompositeToken());

        public static TokensTableBuilder CompositionOperator(this TokensTableBuilder builder, int precedence,
            IToken token, CompositionHandler handler) =>
            builder.Operator(new OperatorDefinition(token, precedence, new CompositionInvoker(handler)));

        public static TokensTableBuilder CompositionOperator(this TokensTableBuilder builder, int precedence,
            in CompositionDefinition definition) =>
            builder.CompositionOperator(precedence, definition.Token, definition.CompositionHandler);

        public static TokensTableBuilder Operand(this TokensTableBuilder builder, OperandHandler handler, IToken token) =>
            builder.Operand(new OperandDefinition(token, handler));
        public static TokensTableBuilder Operand(this TokensTableBuilder builder, OperandHandler handler, IEnumerable<IToken> tokens) =>
            builder.Operand(handler, tokens.ToCompositeToken());
    }
}
