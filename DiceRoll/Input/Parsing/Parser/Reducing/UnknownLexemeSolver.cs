using System;
using System.Runtime.InteropServices;

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

    [StructLayout(LayoutKind.Auto)]
    public readonly struct UnknownLexemeSolvingResult
    {
        public readonly bool IsValid;
        private readonly Substring _substring;
        private readonly EquationParserState _parserState;
    }
    
    public sealed class ScopedUnknownLexemeSolver : UnknownLexemeSolver
    {
        
        
        public override bool TrySolve(in Substring substring, EquationParserState state) =>
            throw new System.NotImplementedException();
        
        

        public sealed class Scope : IDisposable
        {
            private readonly ScopedUnknownLexemeSolver _source;
            
            public Scope(ScopedUnknownLexemeSolver source)
            {
                _source = source;
            }

            public void Dispose() { }
        }
    }
}
