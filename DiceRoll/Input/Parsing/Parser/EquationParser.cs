using System;

namespace DiceRoll.Input.Parsing
{
    public sealed class EquationParser
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
            _reader = new EquationReader(state.Mapper, state.Lexemes, lexingPipeline);
            _reducer = new EquationReducer(state.Lexemes, reducingPipeline);
        }

        public void Read(string equation)
        {
            try
            {
                _reader.Read(equation);
            }
            catch (Exception)
            {
                _state.Reset();
                throw;
            }
        }

        public NodeTree Collapse(UnknownLexemeSolver solver)
        {
            // todo error message formatting. A way to reference a lexeme that has caused an exception
            try
            {
                SubstringMapper mapper = _state.Mapper.BuildSubstringMapper();
                Mapped<LinkedNode> root = _reducer.Reduce(solver);
                
                _state.Reset();
            
                return new NodeTree(mapper, root);
            }
            catch (Exception)
            {
                _state.Reset();
                throw;
            }
        }
    }
}
