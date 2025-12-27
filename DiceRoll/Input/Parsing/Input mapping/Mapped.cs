using System;
using System.Runtime.InteropServices;

namespace DiceRoll.Input.Parsing
{
    [StructLayout(LayoutKind.Auto)]
    public readonly struct Mapped<T>
    {
        public readonly Range Range;
        public readonly T Value;

        internal Mapped(in T value, int substringStart, int substringLength) : this(
            in value,
            new Range(new Index(substringStart), new Index(substringStart + substringLength))
            ) { }

        internal Mapped(in T value, in Range range)
        {
            Value = value;
            Range = range;
        }
    }

    public static class MappedExtensions
    {
        public static bool TryCastValue<TSource, TResult>(this Mapped<TSource> source, out Mapped<TResult> result)
            where TSource : class where TResult : TSource
        {
            if (source.Value is TResult castedValue)
            {
                result = new Mapped<TResult>(castedValue, in source.Range);
                return true;
            }

            result = default;
            return false;
        }

        public static Mapped<TResult> CastValueOrThrow<TSource, TResult>(this Mapped<TSource> source)
            where TSource : class where TResult : TSource =>
            new((TResult) source.Value, in source.Range);

        public static Mapped<T> WithValue<T>(this Mapped<T> mapped, in T value) =>
            new(in value, in mapped.Range);
    }
}
