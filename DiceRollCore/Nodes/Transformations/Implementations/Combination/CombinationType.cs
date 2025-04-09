namespace DiceRoll
{
    /// <summary>
    /// Determines the behaviour of a <see cref="Combination"/> instance.
    /// </summary>
    /// <seealso cref="Add"/>
    /// <seealso cref="Subtract"/>
    public enum CombinationType
    {
        /// <summary>
        /// Tells the <see cref="Combination"/> instance to add one input node to another.
        /// </summary>
        Add = 0,
        /// <summary>
        /// Tells the <see cref="Combination"/> instance to subtract one input node from another.
        /// </summary>
        Subtract = 1,
        Multiply = 2,
        DivideRoundDownwards = 3,
        DivideRoundUpwards = 4
    }
}
