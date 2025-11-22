using System;

namespace DiceRoll.Input.Parsing
{
    public sealed class EquationFolder
    {
        private readonly EquationParserState _state;
        private readonly FoldingPipeline _pipeline;
        
        public EquationFolder(EquationParserState state, FoldingPipeline pipeline)
        {
            _state = state;
            _pipeline = pipeline;
        }

        public NodeTree Fold(UnknownLexemeSolver solver)
        {
            SubstringMapper mapper = _state.Mapper.BuildSubstringMapper();
            Mapped<LinkedNode> root = FoldAndCollapse(solver);
            
            return new NodeTree(mapper, root);
        }

        private Mapped<LinkedNode> FoldAndCollapse(UnknownLexemeSolver solver)
        {
            _pipeline.ExecuteAll(_state, solver);

            return GetResultOrThrow();
        }

        private Mapped<LinkedNode> GetResultOrThrow()
        {
            if (_state.Lexemes.Count is not 1)
                throw new Exception(); // todo

            return _state.Lexemes.GetTypedOrThrow<Operand>(0).ToLinkedNode();
        }
    }
}
