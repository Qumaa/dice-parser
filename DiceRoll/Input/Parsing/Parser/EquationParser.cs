using System;

namespace DiceRoll.Input.Parsing
{
    public sealed class EquationParser
    {
        private readonly EquationReader _reader;
        private readonly EquationFolder _folder;

        public EquationParser(EquationParserState state, LexingPipeline lexingPipeline, FoldingPipeline foldingPipeline)
        {
            ArgumentNullException.ThrowIfNull(state);
            ArgumentNullException.ThrowIfNull(lexingPipeline);
            ArgumentNullException.ThrowIfNull(foldingPipeline);

            _reader = new EquationReader(state, lexingPipeline);
            _folder = new EquationFolder(state, foldingPipeline);
        }

        public void Read(string equation) =>
            _reader.Read(equation);

        public NodeTree Fold(UnknownLexemeSolver solver) =>
            _folder.Fold(solver);
    }
}
