namespace DiceRoll.Input.Parsing
{
    public enum ArgumentsLayout
    {
        /// <summary>
        /// <para>
        /// Grabs the first argument from the right and the rest from the left.
        /// </para>
        /// <para>
        /// ... 4 3 2 x 1
        /// </para>
        /// </summary>
        Left,
        /// <summary>
        /// <para>
        /// Grabs the first argument from the left and the rest from the right.
        /// </para>
        /// <para>
        /// 1 x 2 3 4 ...
        /// </para>
        /// </summary>
        Right,
        /// <summary>
        /// <para>
        /// Grabs all arguments from the right.
        /// </para>
        /// <para>
        /// x 1 2 3 ...
        /// </para>
        /// </summary>
        FullRight,
        /// <summary>
        /// <para>
        /// Grabs all arguments from the left.
        /// </para>
        /// <para>
        /// ... 3 2 1 x
        /// </para>
        /// </summary>
        FullLeft
    }
}
