using System;
using System.Collections.Generic;
using System.Linq;

namespace DiceRoll.Input.Parsing
{
    internal class OperatorGroup : TokenGroup
    {
        private readonly ShuntingYardOperators _operators;
        private readonly OperatorDefinition[] _definitions;
        
        public OperatorGroup(int precedence, IEnumerable<OperatorDefinition> definitions, ShuntingYardOperators operators) : base(precedence)
        {
            ArgumentNullException.ThrowIfNull(operators);
            ArgumentNullException.ThrowIfNull(definitions);
            
            _operators = operators;
            _definitions = definitions.ToArray();
        }
        
        public override bool TryMatch(in Substring substring, out Substring match)
        {
            if (!StartsWithOperator(in substring, out OperatorDefinition definition, out match))
                return false;
            
            while (_operators.TryPeek(out Operator lastOperator) &&
                   !lastOperator.IsOpenParenthesis &&
                   definition.Precedence <= lastOperator.Precedence)
                _operators.InvokeAfterDelayedOperators(_operators.Pop());

            Operator @operator = new(definition);
            
            _operators.Process(in @operator, in substring);
            
            return true;
        }

        private bool StartsWithOperator(in Substring expression, out OperatorDefinition definition, 
            out Substring substring)
        {
            foreach (OperatorDefinition operatorDefinition in _definitions)
            {
                if (!MatchesCurrentUsageForm(operatorDefinition.InvocationBehaviour))
                    continue;
                
                if (!operatorDefinition.Token.MatchesStart(in expression, out substring))
                    continue;

                definition = operatorDefinition;
                return true;
            }

            substring = default;
            definition = null;
            return false;
        }
        
        private bool MatchesCurrentUsageForm(OperatorInvocationBehaviour invocationBehaviour) =>
            invocationBehaviour is { LeftArity: 0, RightArity: > 0 } ==
            _operators.CurrentUsageForm is OperatorUsageForm.Prefix;
    }
}
