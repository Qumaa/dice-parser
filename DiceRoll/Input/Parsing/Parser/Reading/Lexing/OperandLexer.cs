using System;
using System.Collections.Generic;
using System.Linq;

namespace DiceRoll.Input.Parsing.Deprecated
{
    public class OperandLexer : Lexer
    {
        public const int DEFAULT_PRECEDENCE = 500;
        
        private readonly ShuntingYardState _state;
        private readonly OperandDefinition[] _definitions;

        public OperandLexer(int precedence, IEnumerable<OperandDefinition> definitions, ShuntingYardState state) : base()
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
            return true;
        }

        private bool StartsWithOperand(in Substring expression, out OperandDefinition definition,
            out Substring substring)
        {
            substring = expression.Empty();
            
            foreach (OperandDefinition operandDefinition in _definitions)
            {
                if (!operandDefinition.Token.MatchesStart(in expression, out Substring newMatch))
                {
                    LexerUtils.UpdateEarliestMatch(ref substring, in newMatch);
                    continue;
                }

                substring = newMatch;
                definition = operandDefinition;
                return true;
            }

            definition = null;
            return false;
        }
        
        private static Operand ParseOperand(OperandDefinition definition, in Substring substring)
        {
            INode parsedNode = definition.Parser.Parse(in substring);
            Type evaluationType = definition.EvaluationType;

            return new Operand(parsedNode, evaluationType);
        }
    }
}
