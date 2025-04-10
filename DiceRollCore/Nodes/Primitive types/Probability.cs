using System;
using System.Collections.Generic;
using System.Globalization;

namespace DiceRoll
{
    /// <summary>
    /// A <see cref="double">double-precision</see> number wrapper,
    /// explicitly stating the intent to treat it as a likelihood of something.
    /// </summary>
    public readonly struct Probability : IEquatable<Probability>, IComparable<Probability>, IComparable
    {
        private const string _FORMAT = "P";
        private const double _MACHINE_EPSILON = 2.220446049250313E-16d;
        private const double _CONSTRUCTOR_TOLERANCE = -8.326672684688674E-16d;
        public readonly double Value;
        
        public static Probability Hundred => new(1d);

        public static Probability Zero => new(0d);

        /// <param name="probability">A <see cref="double">double-precision</see> number to represent,
        /// where 0 = 0% and 1 = 100%.</param>
        /// <exception cref="NegativeProbabilityException">When <paramref name="probability"/> is below 0.</exception>
        public Probability(double probability) 
        {
            if (probability < 0)
            {
                if (probability < _CONSTRUCTOR_TOLERANCE)
                    throw new NegativeProbabilityException(probability);

                Value = 0;
            }
            
            Value = probability;
        }

        public Probability Inversed() =>
            new(1d - Value);
        
        public Probability Normalized() =>
            new(double.Min(Value, 1d));

        private static bool Approximates(double left, double right)
        {
            if (left.Equals(right))
                return true;

            double tolerance = Math.Abs(left) * _MACHINE_EPSILON + Math.Abs(right) * _MACHINE_EPSILON;
            double difference = left - right;

            return -tolerance < difference && tolerance > difference;
        }

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
            Equals(other) ? 0 : Value.CompareTo(other.Value);

        public int CompareTo(object obj)
        {
            if (obj is null)
                return 1;

            return obj is Probability other ?
                CompareTo(other) :
                throw new ArgumentException($"Object must be of type {nameof(Probability)}");
        }

        public bool Equals(Probability other) =>
            Approximates(Value, other.Value);

        public override bool Equals(object obj) =>
            obj is Probability other && Equals(other);
        
        public override int GetHashCode() =>
            Value.GetHashCode();

        public override string ToString() =>
            ToString(_FORMAT);

        public string ToString(string format) =>
            ToString(format, CultureInfo.CurrentCulture);
        
        public string ToString(IFormatProvider formatProvider) =>
            ToString(_FORMAT, formatProvider);
        
        public string ToString(string format, IFormatProvider formatProvider) =>
            Value.ToString(format, formatProvider);

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
            left.Equals(right) || left.Value > right.Value;
        public static bool operator <(Probability left, Probability right) =>
            left.Value < right.Value;
        public static bool operator <=(Probability left, Probability right) =>
            left.Equals(right) || left.Value < right.Value;
        public static bool operator ==(Probability left, Probability right) =>
            left.Equals(right);
        public static bool operator !=(Probability left, Probability right) =>
            !(left == right);

    #endregion
    }
}
