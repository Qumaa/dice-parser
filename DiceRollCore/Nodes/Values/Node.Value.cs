using System;
using System.Collections.Generic;
using System.Linq;

namespace DiceRoll
{
    public static partial class Node
    {
        private static readonly Random _sharedRandom = new();

        public static class Value
        {
            public static INumeric Constant(int value) =>
                new NumericConstant(value);
            
            public static IAssertion Constant(bool value) =>
                new BinaryConstant(value);

            public static INumeric Dice(int faces) =>
                faces is 1 ? Constant(1) : new Dice(_sharedRandom, faces);
            public static INumeric Dice(int faces, int dice) =>
                Dice<Summarize>(faces, dice);
            public static INumeric Dice<T>(int faces, int dice) where T : Composer, new() =>
                dice <= 1 ? Dice(faces) : Dice(new T(), faces, dice);
            public static INumeric Dice(Composer composer, int faces, int dice) =>
                dice <= 1 ? Dice(faces) : Composite(composer, Dice(faces), dice);

            
            public static INumeric Composite(Composer composer, IEnumerable<INumeric> sourceNodes) =>
                new Composite(composer, sourceNodes);
            public static INumeric Composite(Composer composer, INumeric first, INumeric second,
                params INumeric[] sequence) =>
                Composite(composer, sequence.Prepend(second).Prepend(first));
            public static INumeric Composite(Composer composer, INumeric numeric, int repetitionCount) =>
                new Composite(composer, numeric, repetitionCount);

            public static INumeric Composite<T>(IEnumerable<INumeric> sourceNodes) where T : Composer, new() =>
                Composite(new T(), sourceNodes);
            public static INumeric Composite<T>(INumeric first, INumeric second, params INumeric[] sequence)
                where T : Composer, new() =>
                Composite(new T(), first, second, sequence);
            public static INumeric Composite<T>(INumeric numeric, int repetitionCount)
                where T : Composer, new() =>
                Composite(new T(), numeric, repetitionCount);

            public static INumeric Summation(IEnumerable<INumeric> sourceNodes) =>
                Composite<Summarize>(sourceNodes);
            public static INumeric Summation(INumeric first, INumeric second, params INumeric[] sequence) =>
                Composite<Summarize>(first, second, sequence);
            public static INumeric Summation(INumeric numeric, int repetitionCount) =>
                Composite<Summarize>(numeric, repetitionCount);

            public static INumeric Highest(IEnumerable<INumeric> sourceNodes) =>
                Composite<KeepHighest>(sourceNodes);
            public static INumeric Highest(INumeric first, INumeric second, params INumeric[] sequence) =>
                Composite<KeepHighest>(first, second, sequence);
            public static INumeric Highest(INumeric numeric, int repetitionCount) =>
                Composite<KeepHighest>(numeric, repetitionCount);

            public static INumeric Lowest(IEnumerable<INumeric> sourceNodes) =>
                Composite<KeepLowest>(sourceNodes);
            public static INumeric Lowest(INumeric first, INumeric second, params INumeric[] sequence) =>
                Composite<KeepLowest>(first, second, sequence);
            public static INumeric Lowest(INumeric numeric, int repetitionCount) =>
                Composite<KeepLowest>(numeric, repetitionCount);

            // wip
            // public static IOperation Conditional(INumeric value, IAssertion condition) =>
            //     new Conditional(condition, value);
        }
    }
}
