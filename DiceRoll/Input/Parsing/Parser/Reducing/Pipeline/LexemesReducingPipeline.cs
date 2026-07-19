using System;
using System.Collections.Generic;
using System.Linq;

namespace DiceRoll.Input.Parsing
{
    public sealed class LexemesReducingPipeline
    {
        // todo builder and stuff

        private readonly LexemesReducer[] _reducers;

        public LexemesReducingPipeline(IEnumerable<LexemesReducer> reducers)
        {
            ArgumentNullException.ThrowIfNull(reducers);
            
            _reducers = reducers.ToArray();
        }

        public void ExecuteAll(EquationParserState state, UnknownLexemeSolver solver)
        {
            foreach (LexemesReducer reducer in _reducers)
                reducer.Execute(state, solver);
        }

        public static LexemesReducingPipeline CreateDefault(OperandCastingTable castingTable)
        {
            OperatorReducingHandler operatorHandler = new(castingTable);
            SequenceReducingHandler operandsHandler = new();
            
            return new LexemesReducingPipeline(new LexemesReducer[]
            {
                new ParenthesisReducer(operatorHandler, operandsHandler),
                new OperatorReducer(operatorHandler),
                new ExcessiveOperandsReducer(operandsHandler)
            });
        }
    }
}
