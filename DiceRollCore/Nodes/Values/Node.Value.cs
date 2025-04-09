using System;
using System.Linq;

namespace DiceRoll
{
    public static partial class Node
    {
        private static readonly Random _allDiceRandom = new();

        /// <summary>
        /// Contains nodes that represent numeric values used in rolls - dice, modifiers, etc.
        /// </summary>
        public static class Value
        {
            /// <summary>
            /// Creates a <see cref="DiceRoll.Constant"/>.
            /// May represent a roll modifier, a check difficulty etc.
            /// </summary>
            /// <param name="value">The actual numeric value.</param>
            public static INumeric Constant(int value) =>
                new Constant(value);

            /// <summary>
            /// Creates a <see cref="DiceRoll.Dice"/>.
            /// All the dice created this way share their <see cref="Random"/> instance.
            /// </summary>
            /// <param name="faces">A number of faces the virtual dice will have.</param>
            public static INumeric Dice(int faces) =>
                new Dice(_allDiceRandom, faces);

            /// <summary>
            /// Creates a <see cref="DiceRoll.Composite"/> node that evaluates all composed nodes
            /// based on the passed <see cref="Composer"/> implementation instance.
            /// Only supports <see cref="INumeric">numerical nodes</see>.
            /// </summary>
            /// <param name="composer">A <see cref="Composer"/> implementation instance.</param>
            /// <param name="first">The first mandatory <see cref="INumeric">numerical node</see>.</param>
            /// <param name="second">The second mandatory <see cref="INumeric">numerical node</see>.</param>
            /// <param name="sequence">
            /// A sequence of <see cref="INumeric">numerical nodes</see> beyond the first mandatory two.
            /// </param>
            /// <returns>A <see cref="DiceRoll.Composite"/> node.</returns>
            public static INumeric Composite(Composer composer, INumeric first, INumeric second,
                params INumeric[] sequence) =>
                new Composite(sequence.Prepend(second).Prepend(first), composer);
            
            public static INumeric Composite(Composer composer, INumeric node, int repetitionCount) =>
                new Composite(node, repetitionCount, composer);

            /// <summary>
            /// <para>
            /// A variant of the <see cref="Composite">Composite</see> method that automatically creates a
            /// <see cref="Composer"/> instance of type <typeparamref name="T"/>,
            /// provided that it has a parameterless constructor. 
            /// </para>
            /// <para>
            /// Shortens the notation from <c>Composite(new Composer(), sequence)</c>
            /// to <c>Composite&lt;Composer&gt;(sequence)</c>.
            /// </para>
            /// </summary>
            /// <param name="first">The first mandatory <see cref="INumeric">numerical node</see>.</param>
            /// <param name="second">The second mandatory <see cref="INumeric">numerical node</see>.</param>
            /// <param name="sequence">
            /// A sequence of <see cref="INumeric">numerical nodes</see> beyond the first mandatory two.
            /// </param>
            /// <typeparam name="T">
            /// A <see cref="Composer"/> implementation type with a parameterless constructor.
            /// </typeparam>
            /// <returns>A <see cref="DiceRoll.Composite"/> node.</returns>
            public static INumeric Composite<T>(INumeric first, INumeric second, params INumeric[] sequence)
                where T : Composer, new() =>
                Composite(new T(), first, second, sequence);
            
            public static INumeric Composite<T>(INumeric node, int repetitionCount)
                where T : Composer, new() =>
                Composite(new T(), node, repetitionCount);

            /// <summary>
            /// <para>
            /// Creates a <see cref="DiceRoll.Composite"/> node that evaluates all composed nodes
            /// and <see cref="Summarize">summarizes</see> their <see cref="Outcome">Outcomes</see>.
            /// Only supports <see cref="INumeric">numerical nodes</see>.
            /// </para>
            /// <para>
            /// A shorthand for <see cref="Composite{T}">Composite&lt;Summarize&gt;</see>.
            /// </para>
            /// </summary>
            /// <param name="first">The first mandatory <see cref="INumeric">numerical node</see>.</param>
            /// <param name="second">The second mandatory <see cref="INumeric">numerical node</see>.</param>
            /// <param name="sequence">
            /// A sequence of <see cref="INumeric">numerical nodes</see> beyond the first mandatory two to summarize.
            /// </param>
            /// <returns>
            /// A <see cref="DiceRoll.Composite"/> node that evaluates to the total.
            /// </returns>
            public static INumeric Summation(INumeric first, INumeric second, params INumeric[] sequence) =>
                Composite<Summarize>(first, second, sequence);
            
            public static INumeric Summation(INumeric node, int repetitionCount) =>
                Composite<Summarize>(node, repetitionCount);

            /// <summary>
            /// <para>
            /// Creates a <see cref="DiceRoll.Composite"/> node that evaluates all composed nodes
            /// and <see cref="KeepHighest">selects the largest</see> <see cref="Outcome"/>
            /// from among them.
            /// Only supports <see cref="INumeric">numerical nodes</see>.
            /// </para>
            /// <para>
            /// A shorthand for <see cref="Composite{T}">Composite&lt;KeepHighest&gt;</see>.
            /// </para>
            /// </summary>
            /// <param name="first">The first mandatory <see cref="INumeric">numerical node</see>.</param>
            /// <param name="second">The second mandatory <see cref="INumeric">numerical node</see>.</param>
            /// <param name="sequence">
            /// A sequence of <see cref="INumeric">numerical nodes</see> beyond the first mandatory two to select the
            /// largest <see cref="Outcome"/> from among them.
            /// </param>
            /// <returns>
            /// A <see cref="DiceRoll.Composite"/> node that evaluates to the largest <see cref="Outcome"/>.
            /// </returns>
            public static INumeric Highest(INumeric first, INumeric second, params INumeric[] sequence) =>
                Composite<KeepHighest>(first, second, sequence);
            
            public static INumeric Highest(INumeric node, int repetitionCount) =>
                Composite<KeepHighest>(node, repetitionCount);

            /// <summary>
            /// <para>
            /// Creates a <see cref="DiceRoll.Composite"/> node that evaluates all composed nodes
            /// and <see cref="KeepLowest">selects the smallest</see> <see cref="Outcome"/>
            /// from among them.
            /// Only supports <see cref="INumeric">numerical nodes</see>.
            /// </para>
            /// <para>
            /// A shorthand for <see cref="Composite{T}">Composite&lt;KeepLowest&gt;</see>.
            /// </para>
            /// </summary>
            /// <param name="first">The first mandatory <see cref="INumeric">numerical node</see>.</param>
            /// <param name="second">The second mandatory <see cref="INumeric">numerical node</see>.</param>
            /// <param name="sequence">
            /// A sequence of <see cref="INumeric">numerical nodes</see> beyond the first mandatory two to select the
            /// smallest <see cref="Outcome"/> from among them.
            /// </param>
            /// <returns>
            /// A <see cref="DiceRoll.Composite"/> node that evaluates to the smallest <see cref="Outcome"/>.
            /// </returns>
            public static INumeric Lowest(INumeric first, INumeric second, params INumeric[] sequence) =>
                Composite<KeepLowest>(first, second, sequence);
            
            public static INumeric Lowest(INumeric node, int repetitionCount) =>
                Composite<KeepLowest>(node, repetitionCount);

            public static IOperation Conditional(INumeric value, IAssertion condition) =>
                new Conditional(condition, value);
        }
    }
}
