using System;

namespace DiceRoll.Input.Parsing
{
    public sealed class StringComparisonToken : IToken
    {
        private readonly string[] _values;
        private readonly StringComparison _comparison;
        
        public StringComparisonToken(string[] values, StringComparison comparison)
        {
            CommonException.ThrowIfParamsArrayIsEmpty(values);
            
            _values = values;
            Array.Sort(_values, (x, y) => y.Length.CompareTo(x.Length));
            _comparison = comparison;
        }

        public StringComparisonToken(string value, StringComparison comparison) :
            this(Syntax.Params(value), comparison) { }

        public bool Matches(in Substring input, out Substring firstMatch)
        {
            firstMatch = input.Empty();
            
            foreach (string value in _values)
            {
                int i = input.IndexOf(value, _comparison);
                
                if (i < 0) // no match
                    continue;

                int matchStart = input.Start + i;
                int matchLength = value.Length;

                if (MatchUtils.ShouldUpdateMatch(in firstMatch, matchStart, matchLength))
                    firstMatch = new Substring(in input, i, value.Length);
            }
            
            return !firstMatch.IsEmpty;
        }

        public static StringComparisonToken CaseInsensitive(params string[] values) =>
            new(values, StringComparison.OrdinalIgnoreCase);
        
        public static StringComparisonToken CaseInsensitive(string value) =>
            new(value, StringComparison.OrdinalIgnoreCase);
        
        public static StringComparisonToken CaseSensitive(params string[] values) =>
            new(values, StringComparison.Ordinal);
        
        public static StringComparisonToken CaseSensitive(string value) =>
            new(value, StringComparison.Ordinal);
    }
}
