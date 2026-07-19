using System;

namespace DiceRoll.Input.Parsing
{
    public sealed class EquationReducer
    {
        private readonly EquationParserState _state;
        private readonly LexemesReducingPipeline _pipeline;
        
        public EquationReducer(EquationParserState state, LexemesReducingPipeline pipeline)
        {
            ArgumentNullException.ThrowIfNull(state);
            ArgumentNullException.ThrowIfNull(pipeline);

            _state = state;
            _pipeline = pipeline;
        }

        public LinkedNode Reduce(UnknownLexemeSolver solver)
        {
            _pipeline.ExecuteAll(_state, solver);

            return GetResultOrThrow();
        }

        private LinkedNode GetResultOrThrow()
        {
            if (_state.Lexemes.Count is not 1)
                throw new Exception("More than 1 result has been produced by parser. Cannot proceed due to non deterministic output.");

            if (!_state.Lexemes.TryGetTyped(0, out Mapped<Operand> operand))
                throw new InvalidCastException();
                    
            return operand.ToLinkedNode();
        }
    }
}
