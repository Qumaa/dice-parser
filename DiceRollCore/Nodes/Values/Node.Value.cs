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

            public static INumeric Die(int faces) =>
                new Die(_sharedRandom, faces);
            
            public static IComposite Dice(int faces, int dice) =>
                Dice<Total>(faces, dice);
            public static IComposite Dice<T>(int faces, int dice) where T : Composer, new() =>
                Dice(new T(), faces, dice);
            public static IComposite Dice(Composer composer, int faces, int dice) =>
                Composite(composer, Die(faces), dice);

            public static ISequence<T> Sequence<T>(IEnumerable<T> sourceNodes) where T : INode =>
                new Sequence<T>(sourceNodes);
            public static ISequence<T> Sequence<T>(T first, T second, params T[] sequence) where T : INode =>
                new Sequence<T>(sequence.Prepend(second).Prepend(first));
            public static ISequence<T> Sequence<T>(T node, int repetitionCount) where T : INode =>
                new Sequence<T>(node, repetitionCount);

            
            public static IComposite Composite(Composer composer, IEnumerable<INumeric> sourceNodes) =>
                new Composite(composer, sourceNodes);
            public static IComposite Composite(Composer composer, INumeric first, INumeric second,
                params INumeric[] sequence) =>
                Composite(composer, sequence.Prepend(second).Prepend(first));
            public static IComposite Composite(Composer composer, INumeric numeric, int repetitionCount) =>
                new Composite(composer, numeric, repetitionCount);

            public static IComposite Composite<T>(IEnumerable<INumeric> sourceNodes) where T : Composer, new() =>
                Composite(new T(), sourceNodes);
            public static IComposite Composite<T>(INumeric first, INumeric second, params INumeric[] sequence)
                where T : Composer, new() =>
                Composite(new T(), first, second, sequence);
            public static IComposite Composite<T>(INumeric numeric, int repetitionCount)
                where T : Composer, new() =>
                Composite(new T(), numeric, repetitionCount);

            public static IComposite Total(IEnumerable<INumeric> sourceNodes) =>
                Composite<Total>(sourceNodes);
            public static IComposite Total(INumeric first, INumeric second, params INumeric[] sequence) =>
                Composite<Total>(first, second, sequence);
            public static IComposite Total(INumeric numeric, int repetitionCount) =>
                Composite<Total>(numeric, repetitionCount);

            public static IComposite Highest(IEnumerable<INumeric> sourceNodes) =>
                Composite<KeepHighest>(sourceNodes);
            public static IComposite Highest(INumeric first, INumeric second, params INumeric[] sequence) =>
                Composite<KeepHighest>(first, second, sequence);
            public static IComposite Highest(INumeric numeric, int repetitionCount) =>
                Composite<KeepHighest>(numeric, repetitionCount);

            public static IComposite Lowest(IEnumerable<INumeric> sourceNodes) =>
                Composite<KeepLowest>(sourceNodes);
            public static IComposite Lowest(INumeric first, INumeric second, params INumeric[] sequence) =>
                Composite<KeepLowest>(first, second, sequence);
            public static IComposite Lowest(INumeric numeric, int repetitionCount) =>
                Composite<KeepLowest>(numeric, repetitionCount);

            // todo wip
            // public static IOperation Conditional(INumeric value, IAssertion condition) =>
            //     new Conditional(condition, value);
        }
    }
}
