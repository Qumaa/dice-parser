using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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
            out int precedence, out OperatorInvoker invoker)
        {
            if (!TryGetMatchingOperatorInvokers(
                    in expression,
                    usageForm,
                    out tokenMatch,
                    out OperatorInvoker[] invokers,
                    out precedence
                    ))
            {
                invoker = null;
                return false;
            }

            invoker = invokers.Length is 1 ? invokers[0] : new OverloadInvoker(invokers);
            return true;
        }

        public bool StartsWithOperand(in Substring expression, out Substring tokenMatch, out INode operand)
        {
            foreach (OperandDefinition operandToken in _operands)
            {
                if (!operandToken.Token.MatchesStart(in expression, out tokenMatch))
                    continue;

                operand = operandToken.Parse(tokenMatch);
                return true;
            }

            operand = null;
            tokenMatch = default;
            return false;
        }

        public Substring UntilFirstKnownToken(in Substring expression, OperatorUsageForm usageForm)
        {
            return _MatchesAnyToken(in expression, usageForm, out Substring match) ?
                expression.SetLength(match.Start - expression.Start) :
                expression;

            bool _MatchesAnyToken(in Substring expression, OperatorUsageForm usageForm, out Substring match)
            {
                if (_openParenthesis.Matches(in expression, out match))
                    return true;
                
                if (_closeParenthesis.Matches(in expression, out match))
                    return true;

                foreach (OperatorDefinition tokenizedOperator in _operators)
                    if (OperatorMatches(in tokenizedOperator, usageForm, in expression, out match))
                        return true;
                
                foreach (OperandDefinition tokenizedOperand in _operands)
                    if (tokenizedOperand.Token.Matches(in expression, out match))
                        return true;

                return false;
            }
        }

        private bool TryGetMatchingOperatorInvokers(in Substring expression, OperatorUsageForm usageForm,
            out Substring tokenMatch, out OperatorInvoker[] invokers, out int precedence)
        {
            precedence = 0;
            tokenMatch = default;
            
            int matches = 0;
            Span<int> matchesPtr = stackalloc int[_operators.Length];
            
            for (int i = 0; i < _operators.Length; i++)
            {
                OperatorDefinition matchCandidate = _operators[i];
                
                if (!OperatorMatches(in matchCandidate, usageForm, in expression, out Substring match))
                    continue;
                
                bool isFirstMatch = matches is 0;

                if (!(isFirstMatch || _IsOverload(in _operators[matchesPtr[0]], in matchCandidate)))
                    continue;
                
                if (isFirstMatch)
                {
                    precedence = matchCandidate.Precedence;
                    tokenMatch = match;
                }

                matchesPtr[matches++] = i;
            }

            if (matches is 0)
            {
                invokers = Array.Empty<OperatorInvoker>();
                return false;
            }
            
            invokers = new OperatorInvoker[matches];

            for (int i = 0; i < matches; i++)
                invokers[i] = _operators[matchesPtr[i]].Invoker;

            return true;

            bool _IsOverload(in OperatorDefinition mainDefinition, in OperatorDefinition matchCandidate) =>
                matchCandidate.Precedence == mainDefinition.Precedence &&
                matchCandidate.Invoker.LeftArity == mainDefinition.Invoker.LeftArity &&
                matchCandidate.Invoker.RightArity == mainDefinition.Invoker.RightArity;
        }

        private static bool OperatorMatches(in OperatorDefinition matchCandidate, OperatorUsageForm usageForm, in Substring expression, out Substring tokenMatch)
        {
            if (MatchesUsageForm(matchCandidate.Invoker, usageForm) &&
                matchCandidate.Token.MatchesStart(in expression, out tokenMatch))
                return true;

            tokenMatch = default;
            return false;
        }

        private static bool MatchesUsageForm(OperatorInvoker invoker, OperatorUsageForm usageForm) =>
            invoker is { LeftArity: 0, RightArity: > 0 } == usageForm is OperatorUsageForm.Prefix;

        private static TokensTable BuildDefaultTable() =>
            new TokensTableBuilder(Token("("), Token(")"))
                .Operand(in DiceOperand.Default)
                .Operand(in NumericOperand.Default)
                .Operand(in BinaryOperand.Default)
                
                .PrefixUnaryOperator<IAssertion>(110, static node => node.Not(), Token("!", "not"))
                .PrefixUnaryOperator<INumeric>(110, static node => node.Negate(), Token("-"))
                
                .CompositionOperator(120, in CompositionDefinition.Summation)
                .CompositionOperator(120, in CompositionDefinition.Highest)
                .CompositionOperator(120, in CompositionDefinition.Lowest)
                
                .BinaryOperator<INumeric, INumeric>(100, static (left, right) => left.Multiply(right), Token("*"))
                .BinaryOperator<INumeric, INumeric>(100, static (left, right) => left.DivideRoundUp(right), Token("//"))
                .BinaryOperator<INumeric, INumeric>(100, static (left, right) => left.DivideRoundDown(right), Token("/"))
                
                .BinaryOperator<INumeric, INumeric>(90, static (left, right) => left.Add(right), Token("+"))
                .BinaryOperator<INumeric, INumeric>(90, static (left, right) => left.Subtract(right), Token("-"))
                
                .BinaryOperator<INumeric, INumeric>(80, static (left, right) => left.GreaterThanOrEqual(right), Token(">="))
                .BinaryOperator<INumeric, INumeric>(80, static (left, right) => left.LessThanOrEqual(right), Token("<="))
                .BinaryOperator<INumeric, INumeric>(80, static (left, right) => left.GreaterThan(right), Token(">"))
                .BinaryOperator<INumeric, INumeric>(80, static (left, right) => left.LessThan(right), Token("<"))
                
                .BinaryOperator<INumeric, INumeric>(70, static (left, right) => left.Equal(right), Token("==", "="))
                .BinaryOperator<INumeric, INumeric>(70, static (left, right) => left.NotEqual(right), Token("!=", "=/="))
                
                .BinaryOperator<IAssertion, IAssertion>(70, static (left, right) => left.Equal(right), Token("==", "="))
                .BinaryOperator<IAssertion, IAssertion>(70, static (left, right) => left.NotEqual(right), Token("!=", "=/="))
                
                .BinaryOperator<IAssertion, IAssertion>(60, static (left, right) => left.And(right), Token("&&", "&", "and"))
                .BinaryOperator<IAssertion, IAssertion>(60, static (left, right) => left.Or(right), Token("||", "|", "or"))

                .Build();

        private static IToken Token(params string[] values) =>
            ComparisonToken.CaseInsensitive(values);
        
        private sealed class OverloadInvoker : OperatorInvoker
        {
            private readonly OperatorInvoker[] _invokers;

            public OverloadInvoker(OperatorInvoker[] invokers) : base(
                invokers[0].LeftArity,
                invokers[0].RightArity
                )
            {
                _invokers = invokers;
            }

            public override INode Invoke(OperandsStackAccess operands)
            {
                Exception lastException = null;
                
                foreach (OperatorInvoker operatorInvoker in _invokers)
                {
                    try
                    {
                        return operatorInvoker.Invoke(operands);
                    }
                    catch (OperatorInvocationException e)
                    {
                        lastException = e;
                        operands.Reset();
                    }
                }

                throw lastException;
            }
        }
    }
}
