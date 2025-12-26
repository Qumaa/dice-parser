using System;

namespace DiceRoll.Input.Parsing
{
    public sealed class EquationFolder
    {
        private readonly LexemesList _lexemes;
        private readonly FoldingPipeline _pipeline;
        
        public EquationFolder(LexemesList lexemes, FoldingPipeline pipeline)
        {
            ArgumentNullException.ThrowIfNull(lexemes);
            ArgumentNullException.ThrowIfNull(pipeline);

            _lexemes = lexemes;
            _pipeline = pipeline;
        }

        public Mapped<LinkedNode> Fold(UnknownLexemeSolver solver)
        {
            _pipeline.ExecuteAll(_lexemes, solver);

            return GetResultOrThrow();
        }

        private Mapped<LinkedNode> GetResultOrThrow()
        {
            if (_lexemes.Count is not 1)
                throw new Exception(); // todo

            return _lexemes.GetTypedOrThrow<Operand>(0).ToLinkedNode();
        }
    }
}
