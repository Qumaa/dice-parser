using System.Collections.Generic;

namespace DiceRoll.Input.Parsing
{
    public readonly struct CompositionTokenDescriptor
    {
        public static readonly CompositionTokenDescriptor Summation = new(
            Params("summation", "sum", "s"),
            static (numeric, times) => Node.Value.Summation(numeric, times)
            );
        
        public static readonly CompositionTokenDescriptor Highest = new(
            Params("highest", "h"),
            static (numeric, times) => Node.Value.Highest(numeric, times)
            );
        
        public static readonly CompositionTokenDescriptor Lowest = new(
            Params("lowest", "l"),
            static (numeric, times) => Node.Value.Lowest(numeric, times)
            );

        public readonly IEnumerable<string> Tokens;

        public readonly CompositionHandler CompositionHandler;

        public CompositionTokenDescriptor(IEnumerable<string> tokens, CompositionHandler compositionHandler)
        {
            Tokens = tokens;
            CompositionHandler = compositionHandler;
        }

        public CompositionToken Convert() =>
            new(RegexToken.ExactIgnoreCase(Tokens), CompositionHandler);

        private static T[] Params<T>(params T[] args) =>
            args;
    }
}
