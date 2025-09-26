using System;

namespace DiceRoll.Input.Parsing
{
    public sealed class SubstringMapper
    {
        public readonly string Source;

        internal SubstringMapper(string source)
        {
            Source = source;
        }
        
        public Substring GetSubstringOf<T>(in Mapped<T> mapped) =>
            GetSubstringOf(mapped.Range);

        public Substring GetSubstringOf(in Range mappedRange)
        {
            (int offset, int length) = mappedRange.GetOffsetAndLength(Source.Length);

            return new Substring(Source, offset, length);
        }
    }
}
