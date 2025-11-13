using System;
using System.Collections.Generic;
using System.Linq;

namespace DiceRoll.Input.Parsing
{
    internal class OperandGroup : TokenGroup
    {
        private readonly ShuntingYardOperands _operands;
        private readonly OperandDefinition[] _definitions;

        public OperandGroup(int precedence, IEnumerable<OperandDefinition> definitions, ShuntingYardOperands operands) : base(precedence)
        {
            ArgumentNullException.ThrowIfNull(operands);
            ArgumentNullException.ThrowIfNull(definitions);

            _operands = operands;
            _definitions = definitions.ToArray();
        }

        public override bool TryMatch(in Substring substring, out Substring match)
        {
            if (!StartsWithOperand(in substring, out OperandDefinition definition, out match))
                return false;
            
            Operand operand = ParseOperand(definition, in match);
            _operands.Push(in operand, in match);
            // _operators.TryInvokeDelayedOperators(); todo remove
            return true;
        }

        private bool StartsWithOperand(in Substring expression, out OperandDefinition definition,
            out Substring substring)
        {
            foreach (OperandDefinition operandDefinition in _definitions)
            {
                if (!operandDefinition.Token.MatchesStart(in expression, out substring))
                    continue;

                definition = operandDefinition;
                return true;
            }

            substring = default;
            definition = null;
            return false;
        }
        
        private static Operand ParseOperand(OperandDefinition definition, in Substring substring)
        {
            // todo remove
            INode parsedNode = ((FlatOperandParser) definition.Parser).Parse(in substring);

            return new Operand(parsedNode, definition.EvaluationType);
        }
    }
}
