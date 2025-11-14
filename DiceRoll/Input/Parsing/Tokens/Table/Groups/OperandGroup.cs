using System;
using System.Collections.Generic;
using System.Linq;

namespace DiceRoll.Input.Parsing
{
    internal class OperandGroup : TokenGroup
    {
        public const int DEFAULT_PRECEDENCE = 500;
        
        private readonly ShuntingYardState _state;
        private readonly OperandDefinition[] _definitions;

        public OperandGroup(int precedence, IEnumerable<OperandDefinition> definitions, ShuntingYardState state) : base(precedence)
        {
            ArgumentNullException.ThrowIfNull(state);
            ArgumentNullException.ThrowIfNull(definitions);

            _state = state;
            _definitions = definitions.ToArray();
        }

        public override bool TryExecute(in Substring substring, out Substring match)
        {
            if (!StartsWithOperand(in substring, out OperandDefinition definition, out match))
                return false;
            
            Operand operand = ParseOperand(definition, in match);
            _state.Operands.MapAndPush(new LinkedNode(operand.Node, operand.EvaluationType), in match);
            _state.Annotate().OperandProcessing();
            _state.InvocationHandler.TryInvokeDelayedOperators(); // todo remove
            return true;
        }

        private bool StartsWithOperand(in Substring expression, out OperandDefinition definition,
            out Substring substring)
        {
            substring = expression.Empty();
            
            foreach (OperandDefinition operandDefinition in _definitions)
            {
                if (!operandDefinition.Token.MatchesStart(in expression, out substring))
                    continue;

                definition = operandDefinition;
                return true;
            }

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
