using System;
using System.Collections.Generic;
using System.Linq;

namespace DiceRoll.Input
{
    public static class DiceOperand
    {
        public static readonly TokenizedOperand Default = BuildDefault();

        public static DiceOperandBuilder StartBuilding(string defaultDelimiter, IToken defaultComposition,
            DiceCompositionHandler compositionHandler) =>
            new(defaultDelimiter, defaultComposition, compositionHandler);
        public static DiceOperandBuilder StartBuilding(string defaultDelimiter, string defaultComposition,
            DiceCompositionHandler compositionHandler) =>
            new(defaultDelimiter, defaultComposition, compositionHandler);
        public static DiceOperandBuilder StartBuilding(string defaultDelimiter, IEnumerable<string> defaultComposition,
            DiceCompositionHandler compositionHandler) =>
            new(defaultDelimiter, defaultComposition, compositionHandler);
        public static DiceOperandBuilder StartBuilding(IEnumerable<string> defaultDelimiters, IToken defaultComposition,
            DiceCompositionHandler compositionHandler) =>
            new(defaultDelimiters, defaultComposition, compositionHandler);
        public static DiceOperandBuilder StartBuilding(IEnumerable<string> defaultDelimiters, string defaultComposition,
            DiceCompositionHandler compositionHandler) =>
            new(defaultDelimiters, defaultComposition, compositionHandler);
        public static DiceOperandBuilder StartBuilding(IEnumerable<string> defaultDelimiters, IEnumerable<string> defaultComposition,
            DiceCompositionHandler compositionHandler) =>
            new(defaultDelimiters, defaultComposition, compositionHandler);
        
        public static DiceOperandBuilder StartBuilding(char defaultDelimiter, string defaultComposition,
            DiceCompositionHandler compositionHandler) =>
            StartBuilding(char.ToString(defaultDelimiter), defaultComposition, compositionHandler);
        public static DiceOperandBuilder StartBuilding(char defaultDelimiter, IToken defaultComposition,
            DiceCompositionHandler compositionHandler) =>
            StartBuilding(char.ToString(defaultDelimiter), defaultComposition, compositionHandler);
        public static DiceOperandBuilder StartBuilding(char defaultDelimiter, IEnumerable<string> defaultComposition,
            DiceCompositionHandler compositionHandler) =>
            StartBuilding(char.ToString(defaultDelimiter), defaultComposition, compositionHandler);
        public static DiceOperandBuilder StartBuilding(IEnumerable<char> defaultDelimiters, string defaultComposition,
            DiceCompositionHandler compositionHandler) =>
            StartBuilding(defaultDelimiters.Select(static x => char.ToString(x)), defaultComposition, compositionHandler);
        public static DiceOperandBuilder StartBuilding(IEnumerable<char> defaultDelimiters, IToken defaultComposition,
            DiceCompositionHandler compositionHandler) =>
            StartBuilding(defaultDelimiters.Select(static x => char.ToString(x)), defaultComposition, compositionHandler);
        public static DiceOperandBuilder StartBuilding(IEnumerable<char> defaultDelimiters, IEnumerable<string> defaultComposition,
            DiceCompositionHandler compositionHandler) =>
            StartBuilding(defaultDelimiters.Select(static x => char.ToString(x)), defaultComposition, compositionHandler);

        public static TokenizedOperand DefaultWithInjection(Action<DiceOperandBuilder> injector) =>
            BuildDefault(injector);

        private static TokenizedOperand BuildDefault(Action<DiceOperandBuilder> injector = null)
        {
            DiceOperandBuilder builder =
                StartBuilding('d', Params("sum", "summation"), static (dice, count) => Node.Value.Summation(dice, count));
            
            builder.AddComposition(Params("adv", "advantage", "highest"), static (dice, count) => Node.Value.Highest(dice, count));
            builder.AddComposition(Params("dis", "disadvantage", "lowest"), static (dice, count) => Node.Value.Lowest(dice, count));

            injector?.Invoke(builder);
            
            return builder.Build();
        }

        private static T[] Params<T>(params T[] args) =>
            args;
    }
}
