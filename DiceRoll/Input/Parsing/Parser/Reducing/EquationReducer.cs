using System;

namespace DiceRoll.Input.Parsing
{
    public sealed class EquationReducer
    {
        private readonly LexemesList _lexemes;
        private readonly LexemesReducingPipeline _pipeline;
        
        public EquationReducer(LexemesList lexemes, LexemesReducingPipeline pipeline)
        {
            ArgumentNullException.ThrowIfNull(lexemes);
            ArgumentNullException.ThrowIfNull(pipeline);

            _lexemes = lexemes;
            _pipeline = pipeline;
        }

        public Mapped<LinkedNode> Reduce(Cursor cursor, UnknownLexemeSolver solver)
        {
            _pipeline.ExecuteAll(_lexemes, cursor, solver);

            return GetResultOrThrow();
        }

        private Mapped<LinkedNode> GetResultOrThrow()
        {
            if (_lexemes.Count is not 1)
                throw new Exception("More than 1 result has been produced by parser. Cannot proceed due to non deterministic output.");

            return _lexemes.GetTypedOrThrow<Operand>(0).ToLinkedNode();
        }
    }
}
