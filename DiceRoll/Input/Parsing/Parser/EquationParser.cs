using System;

namespace DiceRoll.Input.Parsing
{
    public sealed class EquationParser : IEquationParser
    {
        private readonly EquationParserState _state;
        private readonly EquationReader _reader;
        private readonly EquationReducer _reducer;

        public EquationParser(EquationParserState state, LexingPipeline lexingPipeline, LexemesReducingPipeline reducingPipeline)
        {
            ArgumentNullException.ThrowIfNull(state);
            ArgumentNullException.ThrowIfNull(lexingPipeline);
            ArgumentNullException.ThrowIfNull(reducingPipeline);

            _state = state;
            _reader = new EquationReader(state, lexingPipeline);
            _reducer = new EquationReducer(state, reducingPipeline);
        }

        public void AccumulateInput(string input)
        {
            try
            {
                _reader.Read(input);
            }
            catch (Exception)
            {
                _state.Reset();
                throw;
            }
        }

        public NodeTree ParseAccumulatedInput(UnknownLexemeSolver solver)
        {
            SubstringMapper mapper = _state.Mapper.BuildSubstringMapper();
            
            try
            {
                LinkedNode root = _reducer.Reduce(solver);
                
                _state.Reset();
            
                return new NodeTree(mapper, root);
            }
            catch (Exception e)
            {
                Substring cause = _state.Cursor.GetSubstringOfCurrent(mapper);

                _state.Reset();

                throw new ParsingException(in cause, e);
            }
        }
    }
}
