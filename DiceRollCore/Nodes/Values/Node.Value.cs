using System;
using System.Linq;

namespace DiceRoll
{
    public static partial class Node
    {
        private static readonly Random _allDiceRandom = new();

        public static class Value
        {
            public static INumeric Constant(int value) =>
                new Constant(value);

            public static INumeric Dice(int faces) =>
                new Dice(_allDiceRandom, faces);
            public static INumeric Dice(int faces, int dice) =>
                Dice<Summarize>(faces, dice);
            public static INumeric Dice<T>(int faces, int dice) where T : Composer, new()
            {
                INumeric die = Dice(faces);

                if (dice > 1)
                    die = Composite<T>(die, dice);
                
                return die;
            }

            public static INumeric Composite(Composer composer, INumeric first, INumeric second,
                params INumeric[] sequence) =>
                new Composite(sequence.Prepend(second).Prepend(first), composer);
            
            public static INumeric Composite(Composer composer, INumeric node, int repetitionCount) =>
                new Composite(node, repetitionCount, composer);

            public static INumeric Composite<T>(INumeric first, INumeric second, params INumeric[] sequence)
                where T : Composer, new() =>
                Composite(new T(), first, second, sequence);
            
            public static INumeric Composite<T>(INumeric node, int repetitionCount)
                where T : Composer, new() =>
                Composite(new T(), node, repetitionCount);

            public static INumeric Summation(INumeric first, INumeric second, params INumeric[] sequence) =>
                Composite<Summarize>(first, second, sequence);
            
            public static INumeric Summation(INumeric node, int repetitionCount) =>
                Composite<Summarize>(node, repetitionCount);

            public static INumeric Highest(INumeric first, INumeric second, params INumeric[] sequence) =>
                Composite<KeepHighest>(first, second, sequence);
            
            public static INumeric Highest(INumeric node, int repetitionCount) =>
                Composite<KeepHighest>(node, repetitionCount);

            public static INumeric Lowest(INumeric first, INumeric second, params INumeric[] sequence) =>
                Composite<KeepLowest>(first, second, sequence);
            
            public static INumeric Lowest(INumeric node, int repetitionCount) =>
                Composite<KeepLowest>(node, repetitionCount);

            // wip
            // public static IOperation Conditional(INumeric value, IAssertion condition) =>
            //     new Conditional(condition, value);
        }
    }
}
