using System.Collections.Generic;
using System.Linq;

namespace DiceRoll.Input.Parsing
{
    public static class DiceOperand
    {
        public static readonly Operand Default = BuildDefault();

        public static DiceOperandBuilder StartBuilding(IEnumerable<string> defaultDelimiters,
            in CompositionTokenDescriptor descriptor) =>
            new(defaultDelimiters, in descriptor);
        public static DiceOperandBuilder StartBuilding(string defaultDelimiter,
            in CompositionTokenDescriptor descriptor) =>
            new(defaultDelimiter, in descriptor);

        public static DiceOperandBuilder StartBuilding(IEnumerable<string> defaultDelimiters, IEnumerable<string> defaultComposition,
            CompositionHandler compositionHandler) =>
            StartBuilding(defaultDelimiters, new CompositionTokenDescriptor(defaultComposition, compositionHandler));
        public static DiceOperandBuilder StartBuilding(IEnumerable<string> defaultDelimiters, string defaultComposition,
            CompositionHandler compositionHandler) =>
            StartBuilding(defaultDelimiters, Params(defaultComposition), compositionHandler);
        
        public static DiceOperandBuilder StartBuilding(string defaultDelimiter, IEnumerable<string> defaultComposition,
            CompositionHandler compositionHandler) =>
            StartBuilding(Params(defaultDelimiter), new CompositionTokenDescriptor(defaultComposition, compositionHandler));
        public static DiceOperandBuilder StartBuilding(string defaultDelimiter, string defaultComposition,
            CompositionHandler compositionHandler) =>
            StartBuilding(Params(defaultDelimiter), Params(defaultComposition), compositionHandler);
        
        public static DiceOperandBuilder StartBuilding(char defaultDelimiter, string defaultComposition,
            CompositionHandler compositionHandler) =>
            StartBuilding(char.ToString(defaultDelimiter), defaultComposition, compositionHandler);
        public static DiceOperandBuilder StartBuilding(char defaultDelimiter, IEnumerable<string> defaultComposition,
            CompositionHandler compositionHandler) =>
            StartBuilding(char.ToString(defaultDelimiter), defaultComposition, compositionHandler);
        public static DiceOperandBuilder StartBuilding(IEnumerable<char> defaultDelimiters, string defaultComposition,
            CompositionHandler compositionHandler) =>
            StartBuilding(defaultDelimiters.Select(static x => char.ToString(x)), defaultComposition, compositionHandler);
        public static DiceOperandBuilder StartBuilding(IEnumerable<char> defaultDelimiters, IEnumerable<string> defaultComposition,
            CompositionHandler compositionHandler) =>
            StartBuilding(defaultDelimiters.Select(static x => char.ToString(x)), defaultComposition, compositionHandler);

        private static Operand BuildDefault() =>
            new DiceOperandBuilder("d", in CompositionTokenDescriptor.Summation)
                .AddComposition(in CompositionTokenDescriptor.Highest)
                .AddComposition(in CompositionTokenDescriptor.Lowest)
                .Build();

        private static T[] Params<T>(params T[] args) =>
            args;
    }
}
