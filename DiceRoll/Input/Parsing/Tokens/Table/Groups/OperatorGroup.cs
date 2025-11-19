using System;
using System.Collections.Generic;
using System.Linq;

namespace DiceRoll.Input.Parsing
{
    public class OperatorGroup : TokenGroup
    {
        public const int DEFAULT_PRECEDENCE = 0;
        
        private readonly ShuntingYardState _state;
        private readonly OperatorDefinition[] _definitions;
        
        public OperatorGroup(int precedence, IEnumerable<OperatorDefinition> definitions, ShuntingYardState state) : base(precedence)
        {
            ArgumentNullException.ThrowIfNull(state);
            ArgumentNullException.ThrowIfNull(definitions);
            
            _state = state;
            _definitions = definitions.OrderByDescending(x => x.Precedence).ToArray();
        }
        
        public override bool TryExecute(in Substring substring, out Substring match)
        {
            if (!StartsWithOperator(in substring, out OperatorDefinition definition, out match))
                return false;

            HandleOperator(definition, in match);
            
            return true;
        }

        private bool StartsWithOperator(in Substring expression, out OperatorDefinition definition, 
            out Substring substring)
        {
            substring = expression.Empty();
            
            foreach (OperatorDefinition operatorDefinition in _definitions)
            {
                if (!operatorDefinition.Token.MatchesStart(in expression, out Substring newMatch))
                {
                    TokenGroupUtils.UpdateEarliestMatch(ref substring, in newMatch);
                    continue;
                }
                
                if (!MatchesCurrentUsageForm(operatorDefinition.InvocationBehaviour))
                    continue;

                substring = newMatch;
                definition = operatorDefinition;
                return true;
            }

            definition = null;
            return false;
        }
        
        private bool MatchesCurrentUsageForm(OperatorInvocationBehaviour invocationBehaviour)
        {
            foreach (OperatorInvoker invoker in invocationBehaviour.Invokers)
            {
                if (MatchesUsageForm(invoker))
                    return true;
            }

            return false;
        }
        
        private bool MatchesUsageForm(OperatorInvoker invoker) =>
            invoker is { LeftArity: 0, RightArity: > 0 } == GetCurrentUsageForm() is OperatorUsageForm.Prefix;

        private OperatorUsageForm GetCurrentUsageForm() =>
            _state.PrecedingTokenKind is TokenKind.Operand ?
                OperatorUsageForm.Infix :
                OperatorUsageForm.Prefix;

        private void HandleOperator(OperatorDefinition definition, in Substring substring)
        {
            InvokeHigherOrEqualPrecedenceOperators(definition.Precedence);

            Mapped<Operator> mapped = MapOperator(definition, in substring);
            
            // if (invoker.RightArity is 0)
            // {
            //     // invoke immediately using existing operands
            //     _state.InvocationHandler.InvokeOperator(in mapped);
            //     _state.Annotate().OperandProcessing();
            //     return;
            // }
            
            // resolve using default shunting-yard mechanism
            _state.Operators.Push(in mapped);
            _state.Annotate().OperatorProcessing();
        }

        private void InvokeHigherOrEqualPrecedenceOperators(int precedence)
        {
            while (_state.Operators.TryPeek(out Operator lastOperator))
            {
                if (lastOperator.IsOpenParenthesis)
                    break;
                
                if (lastOperator.Precedence < precedence)
                    break;

                _state.InvocationHandler.InvokeOperator(_state.Operators.Pop());
            }
        }

        private Mapped<Operator> MapOperator(OperatorDefinition definition, in Substring substring)
        {
            Operator @operator = new(definition.InvocationBehaviour, definition.Precedence, _state.Operands.Count);

            return _state.Mapper.Map(in @operator, in substring);
        }
    }
}
