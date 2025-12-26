using System;
using System.Runtime.InteropServices;

namespace DiceRoll.Input.Parsing
{
    [StructLayout(LayoutKind.Explicit)]
    public readonly struct Arity
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
    }

    public static class ArityExtensions
    {
        public static bool IsInfix(this Arity arity) =>
            arity is { Left: > 0, Right: > 0 };
        
        public static bool IsPostfix(this Arity arity) =>
            arity is { Left: > 0, Right: 0 };
        
        public static bool IsPrefix(this Arity arity) =>
            arity is { Left: 0, Right: > 0 };
        
    }
}
