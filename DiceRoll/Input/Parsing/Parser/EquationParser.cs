using System;

namespace DiceRoll.Input.Parsing
{
    public sealed class EquationParser
    {
        private readonly EquationParserState _state;
        private readonly EquationReader _reader;
        private readonly EquationFolder _folder;

        public EquationParser(EquationParserState state, LexingPipeline lexingPipeline, FoldingPipeline foldingPipeline)
        {
            ArgumentNullException.ThrowIfNull(state);
            ArgumentNullException.ThrowIfNull(lexingPipeline);
            ArgumentNullException.ThrowIfNull(foldingPipeline);

            _state = state;
            _reader = new EquationReader(state.Mapper, state.Lexemes, lexingPipeline);
            _folder = new EquationFolder(state.Lexemes, foldingPipeline);
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

        public NodeTree Fold(UnknownLexemeSolver solver)
        {
            try
            {
                SubstringMapper mapper = _state.Mapper.BuildSubstringMapper();
                Mapped<LinkedNode> root = _folder.Fold(solver);
            
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
