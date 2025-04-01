namespace DiceRoll.Nodes
{
    /// <summary>
    /// A <see cref="bool">boolean</see> value wrapper.
    /// Use this type as the target evaluation type for <see cref="IOperation">binary operation nodes</see>.
    /// </summary>
    public readonly struct Binary
    {
        public readonly bool Value;
        
        public Binary(bool value)
        {
            Value = value;
        }

        public static Binary operator !(Binary self) =>
            new(!self.Value);

        public static Binary operator &(Binary left, Binary right) =>
            new(left.Value & right.Value);
        public static Binary operator |(Binary left, Binary right) =>
            new(left.Value | right.Value);

        public static bool operator true(Binary self) =>
            self.Value;
        public static bool operator false(Binary self) =>
            !self.Value;
    }
}
