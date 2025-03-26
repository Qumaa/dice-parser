using System;
using System.Collections.Generic;

namespace DiceRoll.Expressions
{
    public readonly struct Probability : IEquatable<Probability>, IComparable<Probability>, IComparable
    {
        public readonly double Value;
        
        public static Probability Hundred => new(1d);

        public static Probability Zero => new(0d);

        public Probability(double probability) 
        {
            Value = probability;
        }

        public Probability Inversed() =>
            new(1d - Value);

    #region Members and operators

        public static IEqualityComparer<Probability> EqualityComparer { get; } = new ProbabilityEqualityComparer();

        private sealed class ProbabilityEqualityComparer : IEqualityComparer<Probability>
        {
            public bool Equals(Probability left, Probability right) =>
                left.Equals(right);

            public int GetHashCode(Probability probability) =>
                probability.GetHashCode();
        }

        public static IComparer<Probability> RelationalComparer { get; } = new ProbabilityRelationalComparer();

        private sealed class ProbabilityRelationalComparer : IComparer<Probability>
        {
            public int Compare(Probability left, Probability right) =>
                left.CompareTo(right);
        }

        public int CompareTo(Probability other) =>
            Value.CompareTo(other.Value);

        public int CompareTo(object obj)
        {
            if (obj is null)
                return 1;

            return obj is Probability other ?
                CompareTo(other) :
                throw new ArgumentException($"Object must be of type {nameof(Probability)}");
        }

        public bool Equals(Probability other) =>
            Value.Equals(other.Value);
        
        public override bool Equals(object obj) =>
            obj is Probability other && Equals(other);
        
        public override int GetHashCode() =>
            Value.GetHashCode();

        public static Probability operator +(Probability left, Probability right) =>
            new(left.Value + right.Value);
        public static Probability operator +(double left, Probability right) =>
            new(left + right.Value);
        public static Probability operator +(Probability left, double right) =>
            new(left.Value + right);
        
        public static Probability operator -(Probability self) =>
            new(-self.Value);
        public static Probability operator -(Probability left, Probability right) =>
            new(left.Value - right.Value);
        public static Probability operator -(double left, Probability right) =>
            new(left - right.Value);
        public static Probability operator -(Probability left, double right) =>
            new(left.Value - right);

        public static Probability operator *(Probability left, Probability right) =>
            new(left.Value * right.Value);
        public static Probability operator *(double left, Probability right) =>
            new(left * right.Value);
        public static Probability operator *(Probability left, double right) =>
            new(left.Value * right);
        
        public static Probability operator /(Probability left, Probability right) =>
            new(left.Value / right.Value);
        public static Probability operator /(double left, Probability right) =>
            new(left / right.Value);
        public static Probability operator /(Probability left, double right) =>
            new(left.Value / right);

        public static bool operator >(Probability left, Probability right) =>
            left.Value > right.Value;
        public static bool operator >=(Probability left, Probability right) =>
            left.Value >= right.Value;
        public static bool operator <(Probability left, Probability right) =>
            left.Value < right.Value;
        public static bool operator <=(Probability left, Probability right) =>
            left.Value <= right.Value;
        public static bool operator ==(Probability left, Probability right) =>
            Math.Abs(left.Value - right.Value) <= double.Epsilon * 10;
        public static bool operator !=(Probability left, Probability right) =>
            !(left == right);

    #endregion
    }
}
