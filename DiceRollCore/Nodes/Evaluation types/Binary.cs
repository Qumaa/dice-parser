using System;

namespace DiceRoll
{
    public readonly struct Binary : IEquatable<Binary>
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

        public static Binary operator ==(Binary left, Binary right) =>
            new(left.Value == right.Value);

        public static Binary operator !=(Binary left, Binary right) =>
            new(left.Value != right.Value);

        public static bool operator true(Binary self) =>
            self.Value;

        public static bool operator false(Binary self) =>
            !self.Value;

        public override string ToString() =>
            Value.ToString();

        public string ToString(IFormatProvider formatProvider) =>
            Value.ToString(formatProvider);

        public bool Equals(Binary other) =>
            Value == other.Value;

        public override bool Equals(object obj) =>
            obj is Binary other && Equals(other);

        public override int GetHashCode() =>
            Value.GetHashCode();
    }
}
