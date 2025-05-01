using System.Collections.Generic;

namespace DiceRoll.Input.Parsing
{
    public readonly struct CompositionTokenDescriptor
    {
        public static readonly CompositionTokenDescriptor Summation = new(
            Params("s", "sum", "summation"),
            static (numeric, times) => Node.Value.Summation(numeric, times)
            );
        
        public static readonly CompositionTokenDescriptor Highest = new(
            Params("h", "highest"),
            static (numeric, times) => Node.Value.Highest(numeric, times)
            );
        
        public static readonly CompositionTokenDescriptor Lowest = new(
            Params("l", "lowest"),
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
