using System;
using System.Runtime.InteropServices;

namespace DiceRoll.Input.Parsing
{
    [StructLayout(LayoutKind.Explicit)]
    public readonly struct Arity : IComparable<Arity>, IComparable
    {
        [FieldOffset(0)] private readonly ushort _left;
        [FieldOffset(2)] private readonly ushort _right;
        
        public int Left => _left;
        public int Right => _right;
        public int Total => Left + Right;

        public Arity(int left, int right)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(left);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(left, ushort.MaxValue);
            
            ArgumentOutOfRangeException.ThrowIfNegative(right);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(right, ushort.MaxValue);
            
            ArgumentOutOfRangeException.ThrowIfZero(left + right, "arity");

            _left = unchecked((ushort) left);
            _right = unchecked((ushort) right);
        }

        public static implicit operator int(Arity arity) =>
            arity.Total;

        public int CompareTo(Arity other) =>
            Total.CompareTo(other.Total);

        public int CompareTo(object obj)
        {
            if (obj is null)
                return 1;

            return obj is Arity other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(Arity)}");
        }
    }

    public static class ArityExtensions
    {
        public static bool IsInfix(this Arity arity) =>
            arity is { Left: > 0, Right: > 0 };
        
        public static bool IsPostfix(this Arity arity) =>
            arity is { Left: > 0, Right: 0 };
        
        public static bool IsPrefix(this Arity arity) =>
            arity is { Left: 0, Right: > 0 };
        
        public static Range ToRange(this Arity arity, int position) =>
            (position - arity.Left)..(position + arity.Right + 1);
    }
}
