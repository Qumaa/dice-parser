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

        public static TokensTableBuilder Operator(this TokensTableBuilder builder, IToken token,
            OperatorInvocationBehaviour invocationBehaviour) =>
            builder.Operator(new OperatorDefinition(token, invocationBehaviour));

        public static OverloadBuilder OverloadedOperator(this TokensTableBuilder builder, IToken token, int precedence, Associativity associativity) =>
            new(builder, token, precedence, associativity);

        public static TokensTableBuilder BinaryOperator<TReturn, TLeft, TRight>(this TokensTableBuilder builder,
            IToken token, int precedence,
            BinaryInvocationHandler<TReturn, TLeft, TRight> handler,
            Associativity associativity = Associativity.Left) 
            where TReturn : INode where TLeft : INode where TRight : INode =>
            builder.Operator(
                token,
                OperatorInvocationBehaviour.WithoutOverloads(precedence, associativity, OperatorInvoker.Binary(handler))
                );

        public static OverloadBuilder OverloadedBinaryOperator(this TokensTableBuilder builder,
            IToken token, int precedence, Associativity associativity = Associativity.Left) =>
            OverloadedOperator(builder, token, precedence, associativity);

        public static TokensTableBuilder PrefixUnaryOperator<TReturn, T>(this TokensTableBuilder builder, IToken token,
            int precedence, UnaryInvocationHandler<TReturn, T> handler) where TReturn : INode where T : INode =>
            builder.Operator(
                token,
                OperatorInvocationBehaviour.WithoutOverloads(
                    precedence,
                    Associativity.Right,
                    OperatorInvoker.PrefixUnary(handler)
                    )
                );

        public static OverloadBuilder OverloadedPrefixUnaryOperator(this TokensTableBuilder builder,
            IToken token, int precedence) =>
            OverloadedOperator(builder, token, precedence, Associativity.Right);

        public static TokensTableBuilder PostfixUnaryOperator<TReturn, T>(this TokensTableBuilder builder, IToken token,
            int precedence, UnaryInvocationHandler<TReturn, T> handler) where TReturn : INode where T : INode =>
            builder.Operator(
                token,
                OperatorInvocationBehaviour.WithoutOverloads(
                    precedence,
                    Associativity.Left,
                    OperatorInvoker.PostfixUnary(handler)
                    )
                );
        
        public static OverloadBuilder OverloadedPostfixUnaryOperator(this TokensTableBuilder builder,
            IToken token, int precedence) =>
            OverloadedOperator(builder, token, precedence, Associativity.Left);

        public static TokensTableBuilder CompositionOperator(this TokensTableBuilder builder, int precedence,
            in CompositionDefinition definition) =>
            builder.CompositionOperator(definition.Token, precedence, definition.CompositionHandler);

        public static TokensTableBuilder CompositionOperator(this TokensTableBuilder builder, IToken token, 
            int precedence, CompositionHandler handler) =>
            builder.Operator(
                token,
                OperatorInvocationBehaviour.WithoutOverloads(
                    precedence,
                    Associativity.Left,
                    OperatorInvoker.Composition(handler)
                    )
                );

        public static TokensTableBuilder Operand<T>(this TokensTableBuilder builder, OperandParsingHandler parsingHandler,
            IToken token) where T : INode =>
            builder.Operand(OperandDefinition.OfType<T>(token, parsingHandler));

        public static TokensTableBuilder Operand<T>(this TokensTableBuilder builder, OperandParsingHandler parsingHandler,
            IEnumerable<IToken> tokens) where T : INode =>
            builder.Operand<T>(parsingHandler, tokens.ToCompositeToken());

        public sealed class OverloadBuilder
        {
            private readonly IToken _token;
            private readonly TokensTableBuilder _tableBuilder;
            private readonly OperatorInvocationBehaviour.Builder _behaviourBuilder;

            public OverloadBuilder(TokensTableBuilder tableBuilder, IToken token, int precedence, Associativity associativity)
            {
                _tableBuilder = tableBuilder;
                _token = token;

                _behaviourBuilder = OperatorInvocationBehaviour.WithOverloads(precedence, associativity);
            }

            public OverloadBuilder Overload(OperatorInvoker overload)
            {
                _behaviourBuilder.Overload(overload);
                return this;
            }

            public TokensTableBuilder Finish() =>
                _tableBuilder.Operator(_token, _behaviourBuilder.Build());
        }
    }
}
