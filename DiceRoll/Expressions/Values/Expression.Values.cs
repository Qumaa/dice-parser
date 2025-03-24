using System;

namespace DiceRoll.Expressions
{
    public static partial class Expression
    {
        public static partial class Values
        {
            public static Constant Constant(int value) =>
                new(value);

            public static Dice Dice(int faces) =>
                new(new Random(), faces);

            public static Composite Composite(Composer composer, params IAnalyzable[] sequence) =>
                new(sequence, composer);

            public static Composite Composite<T>(params IAnalyzable[] sequence) where T : Composer, new() =>
                Composite(new T(), sequence);
        }
    }
}
