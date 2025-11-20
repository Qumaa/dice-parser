using System;

namespace DiceRoll.Input.Parsing
{
    public sealed class EquationParser
    {
        private readonly EquationReader _reader;
        private readonly EquationFolder _folder;

        public EquationParser(EquationParserState state, TokenGroupChain chain)
        {
            ArgumentNullException.ThrowIfNull(state);
            ArgumentNullException.ThrowIfNull(chain);

            _reader = new EquationReader(state, chain);
            _folder = new EquationFolder();
        }

        public void Read(string equation) =>
            _reader.Read(equation);

        public NodeTree Fold(ExternalTokenSolver solver) =>
            _folder.Fold(solver);
    }
}
