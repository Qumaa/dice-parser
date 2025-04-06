using System.Runtime.InteropServices;

namespace DiceRoll
{
    [StructLayout(LayoutKind.Auto)]
    public readonly struct Optional<T>
    {
        private readonly bool _exists;
        private readonly T _value;

        public static Optional<T> Empty => new();

        public Optional(T value)
        {
            _value = value;
            _exists = true;
        }
        
        public bool Exists(out T value)
        {
            value = _value;
            return _exists;
        }

        public override string ToString() =>
            ToString("-");
        
        public string ToString(string noValue) =>
            _exists ? _value.ToString() : noValue;
    }
}
