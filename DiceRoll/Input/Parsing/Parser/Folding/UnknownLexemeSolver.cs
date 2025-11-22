namespace DiceRoll.Input.Parsing
{
    public abstract class UnknownLexemeSolver
    {
        public static readonly UnknownLexemeSolver Inert = new InertSolver();
        
        public abstract bool TrySolve(in Substring substring, EquationParserState state);
        
        private sealed class InertSolver : UnknownLexemeSolver
        {
            public override bool TrySolve(in Substring substring, EquationParserState state) =>
                false;
        }
    }
}
