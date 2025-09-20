using System;

namespace DiceRoll.Input.Parsing
{
    public sealed class SubstringMapper
    {
        private readonly string _source;

        internal SubstringMapper(string source)
        {
            _source = source;
        }
        
        public Substring Apply<T>(in Mapped<T> mapped) =>
            Apply(mapped.Range);

        public Substring Apply(in Range mappedRange)
        {
            (int offset, int length) = mappedRange.GetOffsetAndLength(_source.Length);

            return new Substring(_source, offset, length);
        }
    }
}
