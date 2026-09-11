using System.Runtime.InteropServices;

namespace DiceRoll.Input.Parsing
{
    [StructLayout(LayoutKind.Auto)]
    public readonly struct Tagged<T>
    {
        public readonly T Value;
        public readonly string Tag;
        
        public Tagged(string tag, T value)
        {
            Tag = tag;
            Value = value;
        }
    }
}
