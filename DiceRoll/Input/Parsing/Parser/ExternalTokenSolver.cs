namespace DiceRoll.Input.Parsing
{
    public abstract class ExternalTokenSolver
    {
        public static readonly ExternalTokenSolver Inert = new InertSolver();
        
        public abstract bool TrySolve(in Substring substring, EquationParserState state);
        
        private sealed class InertSolver : ExternalTokenSolver
        {
            public override bool TrySolve(in Substring substring, EquationParserState state) =>
                false;
        }
    }
}
