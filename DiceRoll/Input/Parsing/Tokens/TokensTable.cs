using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace DiceRoll.Input.Parsing
{
    public class TokensTable
    {
        public static readonly TokensTable Default = BuildDefaultTable();

        private readonly IToken _openParenthesis;

        private readonly IToken _closeParenthesis;

        private readonly OperatorDefinition[] _operators;

        private readonly OperandDefinition[] _operands;

        public TokensTable(IToken openParenthesis, IToken closeParenthesis, IEnumerable<OperatorDefinition> operators,
            IEnumerable<OperandDefinition> operands)
        {
            _openParenthesis = openParenthesis;
            _closeParenthesis = closeParenthesis;
            
            _operators = operators.ToArray();
            _operands = operands.ToArray();
        }

        public bool StartsWithOpenParenthesis(in Substring expression, out Substring tokenMatch) =>
            _openParenthesis.MatchesStart(in expression, out tokenMatch);

        public bool StartsWithCloseParenthesis(in Substring expression, out Substring tokenMatch) =>
            _closeParenthesis.MatchesStart(in expression, out tokenMatch);

        public bool StartsWithOperator(in Substring expression, OperatorUsageForm usageForm, out Substring tokenMatch, 
            out int precedence, out OperatorInvocationBehaviour invocationBehaviour) =>
            new StartsWithOperatorLookup(_operators, stackalloc int[_operators.Length], in expression, usageForm)
                .IsSuccessful(out invocationBehaviour, out precedence, out tokenMatch);

        public bool StartsWithOperand(in Substring expression, out Substring tokenMatch, out Operand operand)
        {
            foreach (OperandDefinition operandDefinition in _operands)
            {
                if (!operandDefinition.Token.MatchesStart(in expression, out tokenMatch))
                    continue;

                operand = new Operand(operandDefinition.ParsingHandler(tokenMatch), operandDefinition.OperandType);
                return true;
            }

            operand = default;
            tokenMatch = default;
            return false;
        }

        public Substring UntilFirstKnownToken(in Substring expression, OperatorUsageForm currentOperatorUsageForm)
        {
            return _MatchesAnyToken(in expression, currentOperatorUsageForm, out Substring match) ?
                expression.SetLength(match.Start - expression.Start) :
                expression;

            bool _MatchesAnyToken(in Substring expression, OperatorUsageForm usageForm, out Substring match)
            {
                if (_openParenthesis.Matches(in expression, out match))
                    return true;
                
                if (_closeParenthesis.Matches(in expression, out match))
                    return true;

                foreach (OperatorDefinition operatorDefinition in _operators)
                    if (StartsWithOperator(in expression, in operatorDefinition, usageForm, out match))
                        return true;
                
                foreach (OperandDefinition operandDefinition in _operands)
                    if (operandDefinition.Token.Matches(in expression, out match))
                        return true;

                return false;
            }
        }

        private static bool StartsWithOperator(in Substring expression, in OperatorDefinition definition,
            OperatorUsageForm usageForm, out Substring tokenMatch)
        {
            if (MatchesUsageForm(definition.InvocationBehaviour, usageForm) &&
                definition.Token.MatchesStart(in expression, out tokenMatch))
                return true;

            tokenMatch = default;
            return false;
        }

        private static bool MatchesUsageForm(OperatorInvocationBehaviour invocationBehaviour, OperatorUsageForm usageForm) =>
            invocationBehaviour is { LeftArity: 0, RightArity: > 0 } == usageForm is OperatorUsageForm.Prefix;

        private static TokensTable BuildDefaultTable() =>
            new TokensTableBuilder(Token("("), Token(")"))
                .Operand(in DiceOperand.Default)
                .Operand(in NumericOperand.Default)
                .Operand(in BinaryOperand.Default)
                
                .PrefixUnaryOperator(Token("!", "not"), 120, static (IAssertion node) => node.Not())
                .PrefixUnaryOperator(Token("-"), 120, static (INumeric node) => node.Negate())
                
                .CompositionOperator(110, in CompositionDefinition.Summation)
                .CompositionOperator(110, in CompositionDefinition.Highest)
                .CompositionOperator(110, in CompositionDefinition.Lowest)
                
                .BinaryOperator(Token("*"), 100, static (INumeric left, INumeric right) => left.Multiply(right))
                .BinaryOperator(Token("//"), 100, static (INumeric left, INumeric right) => left.DivideRoundUp(right))
                .BinaryOperator(Token("/"), 100, static (INumeric left, INumeric right) => left.DivideRoundDown(right))
                
                .BinaryOperator(Token("+"), 90, static (INumeric left, INumeric right) => left.Add(right))
                .BinaryOperator(Token("-"), 90, static (INumeric left, INumeric right) => left.Subtract(right))
                
                .BinaryOperator(Token(">="), 80, static (INumeric left, INumeric right) => left.GreaterThanOrEqual(right))
                .BinaryOperator(Token("<="), 80, static (INumeric left, INumeric right) => left.LessThanOrEqual(right))
                .BinaryOperator(Token(">"), 80, static (INumeric left, INumeric right) => left.GreaterThan(right))
                .BinaryOperator(Token("<"), 80, static (INumeric left, INumeric right) => left.LessThan(right))
                
                // todo: overload syntax (overload = equal precedence, equal token)
                .OverloadedBinaryOperator(Token("==", "="), 70)
                    .Overload(OperatorInvoker.Binary(static (INumeric left, INumeric right) => left.Equal(right)))
                    .Overload(OperatorInvoker.Binary(static (IAssertion left, IAssertion right) => left.Equal(right)))
                    .Finish()
                
                .OverloadedBinaryOperator(Token("!=", "=/="), 70)
                    .Overload(OperatorInvoker.Binary(static (INumeric left, INumeric right) => left.NotEqual(right)))
                    .Overload(OperatorInvoker.Binary(static (IAssertion left, IAssertion right) => left.NotEqual(right)))
                    .Finish()
                
                .BinaryOperator(Token("&&", "&", "and"), 60, static (IAssertion left, IAssertion right) => left.And(right))
                .BinaryOperator(Token("||", "|", "or"), 60, static (IAssertion left, IAssertion right) => left.Or(right))

                .Build();

        private static StringBasedToken Token(params string[] values) =>
            StringBasedToken.CaseInsensitive(values);

        [StructLayout(LayoutKind.Auto)]
        // todo: simplify. This no longer merges multiple invokers into the overload variant
        private readonly ref struct StartsWithOperatorLookup
        {
            private readonly OperatorDefinition[] _definitions;

            private readonly Bag _bag;

            public StartsWithOperatorLookup(OperatorDefinition[] definitions, Span<int> operatorsLengthBuffer,
                in Substring expression, OperatorUsageForm usageForm)
            {
                _definitions = definitions;

                _bag = BuildBag(definitions, operatorsLengthBuffer, in expression, usageForm);
            }

            public bool IsSuccessful(out OperatorInvocationBehaviour invocationBehaviour, out int precedence, out Substring operatorSubstring)
            {
                if (!TryGetInvokerBasedOnMatches(out invocationBehaviour))
                {
                    operatorSubstring = default;
                    precedence = 0;
                    return false;
                }

                operatorSubstring = _bag.OperatorSubstring;
                precedence = _bag.OperatorPrecedence;
                return true;
            }

            private bool TryGetInvokerBasedOnMatches(out OperatorInvocationBehaviour invocationBehaviour) =>
                (invocationBehaviour = _bag.MatchesCount switch
                {
                    0 => NoMatch(),
                    1 => SingleMatch(),
                    _ => ManyMatches()
                }) is not null;

            private static OperatorInvocationBehaviour NoMatch() =>
                null;

            private OperatorInvocationBehaviour SingleMatch() =>
                _definitions[_bag.MatchesPtrs[0]].InvocationBehaviour;

            private static OperatorInvocationBehaviour ManyMatches() => // todo: several behaviours matched the same token 
                throw new Exception();

            private static Bag BuildBag(OperatorDefinition[] definitions, Span<int> matchPtrsBuffer,
                in Substring expression, OperatorUsageForm usageForm)
            {
                int matchesCount = 0;
                int precedence = 0;
                Substring tokenMatch = Substring.Empty(in expression);
                
                for (int i = 0; i < definitions.Length; i++)
                {
                    OperatorDefinition @operator = definitions[i];
                
                    if (!StartsWithOperator(in expression, in @operator, usageForm, out Substring match))
                        continue;
                
                    bool isFirstMatch = matchesCount is 0;

                    if (!(isFirstMatch || IsOverload(in definitions[matchPtrsBuffer[0]], in @operator)))
                        continue;
                    
                    if (isFirstMatch)
                    {
                        precedence = @operator.Precedence;
                        tokenMatch = match;
                    }

                    matchPtrsBuffer[matchesCount++] = i;
                }

                return new Bag(matchesCount, matchPtrsBuffer, tokenMatch, precedence);
            }
            
            private static bool IsOverload(in OperatorDefinition mainDefinition, in OperatorDefinition matchCandidate) =>
                matchCandidate.Precedence == mainDefinition.Precedence &&
                matchCandidate.InvocationBehaviour.LeftArity == mainDefinition.InvocationBehaviour.LeftArity &&
                matchCandidate.InvocationBehaviour.RightArity == mainDefinition.InvocationBehaviour.RightArity;

            [StructLayout(LayoutKind.Auto)]
            private readonly ref struct Bag
            {
                public readonly int MatchesCount;
                public readonly Span<int> MatchesPtrs;

                public readonly Substring OperatorSubstring;
                public readonly int OperatorPrecedence;

                public Bag(int matchesCount, Span<int> matchPtrs, Substring operatorSubstring,
                    int operatorPrecedence)
                {
                    MatchesCount = matchesCount;
                    MatchesPtrs = matchPtrs;
                    OperatorSubstring = operatorSubstring;
                    OperatorPrecedence = operatorPrecedence;
                }
            }
        }
    }
}
