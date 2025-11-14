using System;
using System.Collections.Generic;
using System.Linq;

namespace DiceRoll.Input.Parsing
{
    internal class OperatorGroup : TokenGroup
    {
        public const int DEFAULT_PRECEDENCE = 0;
        
        private readonly ShuntingYardState _state;
        private readonly OperatorDefinition[] _definitions;
        
        public OperatorGroup(int precedence, IEnumerable<OperatorDefinition> definitions, ShuntingYardState state) : base(precedence)
        {
            ArgumentNullException.ThrowIfNull(state);
            ArgumentNullException.ThrowIfNull(definitions);
            
            _state = state;
            _definitions = definitions.ToArray();
        }
        
        public override bool TryExecute(in Substring substring, out Substring match)
        {
            if (!StartsWithOperator(in substring, out OperatorDefinition definition, out match))
                return false;

            Process(definition, in substring);
            
            return true;
        }

        private bool StartsWithOperator(in Substring expression, out OperatorDefinition definition, 
            out Substring substring)
        {
            substring = expression.Empty();
            
            foreach (OperatorDefinition operatorDefinition in _definitions)
            {
                if (!MatchesCurrentUsageForm(operatorDefinition.InvocationBehaviour))
                    continue;
                
                if (!operatorDefinition.Token.MatchesStart(in expression, out substring))
                    continue;

                definition = operatorDefinition;
                return true;
            }

            definition = null;
            return false;
        }
        
        private bool MatchesCurrentUsageForm(OperatorInvocationBehaviour invocationBehaviour) =>
            invocationBehaviour is { LeftArity: 0, RightArity: > 0 } ==
            GetCurrentUsageForm() is OperatorUsageForm.Prefix;

        private OperatorUsageForm GetCurrentUsageForm() =>
            _state.PrecedingTokenKind is TokenKind.Operand ?
                OperatorUsageForm.Infix :
                OperatorUsageForm.Prefix;

        private void Process(OperatorDefinition definition, in Substring substring)
        {
            Operator @operator = new(definition);
            
            InvokeHigherPrecedenceOperators(in @operator);
            
            Mapped<Operator> mapped = _state.Mapper.Map(in @operator, in substring);

            OperatorInvocationBehaviour invocationBehaviour = @operator.InvocationBehaviour;

            if (invocationBehaviour.RightArity is 0)
            {
                // invoke immediately using existing operands
                _state.InvocationHandler.InvokeOperator(in mapped);
                _state.Annotate().OperandProcessing();
                return;
            }
            
            _state.Annotate().OperatorProcessing();

            if (invocationBehaviour is { RightArity: 1, LeftArity: > 0 })
            {
                // resolve using default shunting-yard mechanism
                _state.Operators.Push(in mapped);
                return;
            }

            // delay until more operands are pushed
            _state.DelayedOperators.MapAndPush(
                new DelayedOperator(@operator.InvocationBehaviour, _state.ParenthesisLevel, _state.Operands.Count),
                in substring
                );
        }
        
        private void InvokeHigherPrecedenceOperators(in Operator @operator)
        {
            while (_state.Operators.TryPeek(out Operator lastOperator))
            {
                if (lastOperator.IsOpenParenthesis)
                    break;
                
                if (lastOperator.Precedence > @operator.Precedence)
                    break;
                
                _state.InvocationHandler.InvokeAfterDelayedOperators(_state.Operators.Pop());
            }
        }
    }
}
