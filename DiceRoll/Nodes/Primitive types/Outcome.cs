using System;
using System.Collections.Generic;

namespace DiceRoll.Nodes
{
    /// <summary>
    /// An <see cref="int">integer</see> number wrapper.
    /// Use this type as the target evaluation type for numeric <see cref="INode{T}">nodes</see>.
    /// </summary>
    public readonly struct Outcome : IEquatable<Outcome>, IComparable<Outcome>, IComparable
    {
        public readonly int Value;
        
        /// <param name="value">Any <see cref="int">integer</see> number.</param>
        public Outcome(int value)
        {
            Value = value;
        }

        public static Outcome Max(Outcome left, Outcome right) =>
            left > right ? left : right;

        public static Outcome Min(Outcome left, Outcome right) =>
            left < right ? left : right;

    #region Members and operators

        public static IEqualityComparer<Outcome> EqualityComparer { get; } = new OutcomeEqualityComparer();

        private sealed class OutcomeEqualityComparer : IEqualityComparer<Outcome>
        {
            public bool Equals(Outcome left, Outcome right) =>
                left.Equals(right);

            public int GetHashCode(Outcome probability) =>
                probability.GetHashCode();
        }

        public static IComparer<Outcome> RelationalComparer { get; } = new OutcomeRelationalComparer();

        private sealed class OutcomeRelationalComparer : IComparer<Outcome>
        {
            public int Compare(Outcome left, Outcome right) =>
                left.CompareTo(right);
        }

        public int CompareTo(Outcome other) =>
            Value.CompareTo(other.Value);

        public int CompareTo(object obj)
        {
            if (obj is null)
                return 1;

            return obj is Outcome other ?
                CompareTo(other) :
                throw new ArgumentException($"Object must be of type {nameof(Outcome)}");
        }

        public bool Equals(Outcome other) =>
            Value.Equals(other.Value);
        
        public override bool Equals(object obj) =>
            obj is Outcome other && Equals(other);
        
        public override int GetHashCode() =>
            Value.GetHashCode();

        public static Outcome operator ++(Outcome self) =>
            new(self.Value + 1);
        public static Outcome operator +(Outcome left, Outcome right) =>
            new(left.Value + right.Value);
        public static Outcome operator +(int left, Outcome right) =>
            new(left + right.Value);
        public static Outcome operator +(Outcome left, int right) =>
            new(left.Value + right);

        public static Outcome operator -(Outcome self) =>
            new(-self.Value);
        public static Outcome operator --(Outcome self) =>
            new(self.Value - 1);
        public static Outcome operator -(Outcome left, Outcome right) =>
            new(left.Value - right.Value);
        public static Outcome operator -(int left, Outcome right) =>
            new(left - right.Value);
        public static Outcome operator -(Outcome left, int right) =>
            new(left.Value - right);

        public static Outcome operator *(Outcome left, Outcome right) =>
            new(left.Value * right.Value);
        public static Outcome operator *(int left, Outcome right) =>
            new(left * right.Value);
        public static Outcome operator *(Outcome left, int right) =>
            new(left.Value * right);
        
        public static Outcome operator /(Outcome left, Outcome right) =>
            new(left.Value / right.Value);
        public static Outcome operator /(int left, Outcome right) =>
            new(left / right.Value);
        public static Outcome operator /(Outcome left, int right) =>
            new(left.Value / right);

        public static bool operator >(Outcome left, Outcome right) =>
            left.Value > right.Value;
        public static bool operator >=(Outcome left, Outcome right) =>
            left.Value >= right.Value;
        public static bool operator <(Outcome left, Outcome right) =>
            left.Value < right.Value;
        public static bool operator <=(Outcome left, Outcome right) =>
            left.Value <= right.Value;
        public static bool operator ==(Outcome left, Outcome right) =>
            left.Value == right.Value;
        public static bool operator !=(Outcome left, Outcome right) =>
            !(left == right);

    #endregion
    }
}
