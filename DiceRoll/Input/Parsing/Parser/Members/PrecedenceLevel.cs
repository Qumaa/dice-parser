using System.Runtime.InteropServices;

namespace DiceRoll.Input.Parsing
{
    [StructLayout(LayoutKind.Auto)]
    public readonly struct PrecedenceLevel
    {
        public readonly int Value;
        private readonly byte _associativityValue; // 1st bit = left, 2nd bit = right

        public PrecedenceLevel(int value, Associativity associativity) : this(
            value,
            associativity is Associativity.Left,
            associativity is Associativity.Right
            ) { }
        
        public PrecedenceLevel(int value, bool leftAssociativity, bool rightAssociativity)
        {
            Value = value;
            _associativityValue = EncodeAssociativity(leftAssociativity, rightAssociativity);
        }

        public bool HasAssociativity(Associativity associativity) =>
            associativity switch
            {
                Associativity.Left => (_associativityValue & 1) is 1,
                Associativity.Right => (_associativityValue & 2) is 2,
                _ => false
            };

        private static byte EncodeAssociativity(bool left, bool right)
        {
            int value = left ? 1 : 0;
            value |= right ? 2 : 0;

            return unchecked((byte) value);
        }
    }
}
