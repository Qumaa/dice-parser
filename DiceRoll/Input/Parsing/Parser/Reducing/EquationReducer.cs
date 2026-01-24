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

        public LinkedNode Reduce(Cursor cursor, UnknownLexemeSolver solver)
        {
            _pipeline.ExecuteAll(_lexemes, cursor, solver);

            return GetResultOrThrow();
        }

        private LinkedNode GetResultOrThrow()
        {
            if (_lexemes.Count is not 1)
                throw new Exception("More than 1 result has been produced by parser. Cannot proceed due to non deterministic output.");

            if (!_lexemes.TryGetTyped(0, out Mapped<Operand> operand))
                throw new InvalidCastException();
                    
            return operand.ToLinkedNode();
        }
    }
}
