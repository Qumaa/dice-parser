using System;
using System.Collections.Generic;
using System.Linq;

namespace DiceRoll.Input.Parsing
{
    public class OperandLexer : Lexer
    {
        private readonly LexemesList _lexemes;
        private readonly OperandDefinition[] _definitions;

        public OperandLexer(IEnumerable<OperandDefinition> definitions, LexemesList lexemes)
        {
            ArgumentNullException.ThrowIfNull(lexemes);
            ArgumentNullException.ThrowIfNull(definitions);

            _lexemes = lexemes;
            _definitions = definitions.ToArray();
        }

        public override bool TryExecute(in Substring substring, out Substring match)
        {
            if (!StartsWithOperand(in substring, out OperandDefinition definition, out match))
                return false;
            
            Operand operand = ParseOperand(definition, in match);
            _lexemes.Push(operand, in match);
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
