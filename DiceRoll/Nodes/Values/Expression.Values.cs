using System;

namespace DiceRoll.Nodes
{
    public static partial class Node
    {
        private static readonly Random _allDiceRandom = new();
        
        /// <summary>
        /// Contains nodes that represent numeric values used in rolls - dice, modifiers, etc.
        /// </summary>
        public static partial class Value
        {
            /// <summary>
            /// Creates a <see cref="DiceRoll.Nodes.Constant"/>.
            /// May represent a roll modifier, a check difficulty etc.
            /// </summary>
            /// <param name="value">The actual numeric value.</param>
            public static Constant Constant(int value) =>
                new(value);

            /// <summary>
            /// Creates a <see cref="DiceRoll.Nodes.Dice"/>.
            /// All the dice created this way share their <see cref="Random"/> instance.
            /// </summary>
            /// <param name="faces">A number of faces the virtual dice will have.</param>
            public static Dice Dice(int faces) =>
                new(_allDiceRandom, faces);

            /// <summary>
            /// <para>
            /// Creates a <see cref="DiceRoll.Nodes.Composite"/> node that evaluates all composed nodes
            /// and <see cref="DiceRoll.Nodes.Summarize">summarizes</see> their <see cref="Outcome">Outcomes</see>.
            /// Only supports <see cref="IAnalyzable">numerical nodes</see>.
            /// </para>
            /// <para>
            /// A shorthand for <see cref="Composite{T}">Composite&lt;Summarize&gt;</see>.
            /// </para>
            /// </summary>
            /// <param name="sequence">
            /// A sequence of <see cref="IAnalyzable">numerical nodes</see> to summarize.
            /// </param>
            /// <returns>
            /// A <see cref="DiceRoll.Nodes.Composite"/> node that evaluates to the total.
            /// </returns>
            public static Composite Summarize(params IAnalyzable[] sequence) =>
                Composite<Summarize>(sequence);
            
            /// <summary>
            /// <para>
            /// Creates a <see cref="DiceRoll.Nodes.Composite"/> node that evaluates all composed nodes
            /// and <see cref="DiceRoll.Nodes.KeepHighest">selects the largest</see> <see cref="Outcome"/>
            /// from among them.
            /// Only supports <see cref="IAnalyzable">numerical nodes</see>.
            /// </para>
            /// <para>
            /// A shorthand for <see cref="Composite{T}">Composite&lt;KeepHighest&gt;</see>.
            /// </para>
            /// </summary>
            /// <param name="sequence">
            /// A sequence of <see cref="IAnalyzable">numerical nodes</see> to select the largest
            /// <see cref="Outcome"/> from among them.
            /// </param>
            /// <returns>
            /// A <see cref="DiceRoll.Nodes.Composite"/> node that evaluates to the largest
            /// <see cref="Outcome"/>.
            /// </returns>
            public static Composite KeepHighest(params IAnalyzable[] sequence) =>
                Composite<KeepHighest>(sequence);
            
            /// <summary>
            /// <para>
            /// Creates a <see cref="DiceRoll.Nodes.Composite"/> node that evaluates all composed nodes
            /// and <see cref="DiceRoll.Nodes.KeepLowest">selects the smallest</see> <see cref="Outcome"/>
            /// from among them.
            /// Only supports <see cref="IAnalyzable">numerical nodes</see>.
            /// </para>
            /// <para>
            /// A shorthand for <see cref="Composite{T}">Composite&lt;KeepLowest&gt;</see>.
            /// </para>
            /// </summary>
            /// <param name="sequence">
            /// A sequence of <see cref="IAnalyzable">numerical nodes</see> to select the smallest
            /// <see cref="Outcome"/> from among them.
            /// </param>
            /// <returns>
            /// A <see cref="DiceRoll.Nodes.Composite"/> node that evaluates to the smallest
            /// <see cref="Outcome"/>.
            /// </returns>
            public static Composite KeepLowest(params IAnalyzable[] sequence) =>
                Composite<KeepLowest>(sequence);

            /// <summary>
            /// Creates a <see cref="DiceRoll.Nodes.Composite"/> node that evaluates all composed nodes
            /// based on the passed <see cref="Composer"/> implementation instance.
            /// Only supports <see cref="IAnalyzable">numerical nodes</see>.
            /// </summary>
            /// <param name="composer">A <see cref="Composer"/> implementation instance.</param>
            /// <param name="sequence">A sequence of <see cref="IAnalyzable">numerical nodes</see>.</param>
            /// <returns>A <see cref="DiceRoll.Nodes.Composite"/> node.</returns>
            public static Composite Composite(Composer composer, params IAnalyzable[] sequence) =>
                new(sequence, composer);

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
            /// <param name="sequence">A sequence of <see cref="IAnalyzable">numerical nodes</see>.</param>
            /// <typeparam name="T">A <see cref="Composer"/> implementation type.</typeparam>
            /// <returns>A <see cref="DiceRoll.Nodes.Composite"/> node.</returns>
            public static Composite Composite<T>(params IAnalyzable[] sequence) where T : Composer, new() =>
                Composite(new T(), sequence);
        }
    }
}
