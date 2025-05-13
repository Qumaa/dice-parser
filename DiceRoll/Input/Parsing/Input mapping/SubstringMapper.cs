using System;

namespace DiceRoll.Input.Parsing
{
    public readonly struct SubstringMapper
    {
        private readonly string _source;
        
        public SubstringMapper(string source)
        {
            _source = source;
        }
        
        public Substring Apply<T>(in Mapped<T> mapped) =>
            Apply(mapped.Range);

        public Substring Apply(in Range mappedRange)
        {
            (int Offset, int Length) tuple = mappedRange.GetOffsetAndLength(_source.Length);

            return new Substring(_source, tuple.Offset, tuple.Length);
        }
    }
}
