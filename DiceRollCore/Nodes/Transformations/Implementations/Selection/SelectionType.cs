namespace DiceRoll.Nodes
{
    /// <summary>
    /// Determines the behaviour of a <see cref="Selection"/> instance.
    /// </summary>
    /// <seealso cref="Highest"/>
    /// <seealso cref="Lowest"/>
    public enum SelectionType
    {
        /// <summary>
        /// Tells the <see cref="Selection"/> instance to select the highest result.
        /// </summary>
        Highest = 0,
        /// <summary>
        /// Tells the <see cref="Selection"/> instance to select the lowest result.
        /// </summary>
        Lowest = 1
    }
}
