using System.Runtime.InteropServices;

namespace DiceRoll
{
    // todo: make value retrievable even if exists is set to false (make fields public?)
    [StructLayout(LayoutKind.Auto)]
    public readonly struct Optional<T>
    {
        public readonly bool Exists;
        public readonly T Value;

        public static Optional<T> Empty => new();

        public Optional(T value)
        {
            Value = value;
            Exists = true;
        }
        
        public bool GetIfExists(out T value)
        {
            value = Value;
            return Exists;
        }

        public Binary AsBinary() =>
            new(Exists);

        public override string ToString() =>
            ToString(false.ToString());
        
        public string ToString(string noValue) =>
            Exists ? Value.ToString() : noValue;
    }
}
