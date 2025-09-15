using System;

namespace DiceRoll.Input.Parsing
{
    public sealed class ComparisonToken : IToken
    {
        private readonly string[] _values;
        private readonly StringComparison _comparison;
        
        public ComparisonToken(string[] values, StringComparison comparison)
        {
            ConstructorException.ThrowIfParamsArrayIsEmpty(values);
            
            _values = values;
            _comparison = comparison;
        }

        public ComparisonToken(string value, StringComparison comparison) : this(new[] { value }, comparison) { }

        public bool Matches(in Substring input, out Substring match)
        {
            foreach (string value in _values)
            {
                int i = input.AsSpan().IndexOf(value, _comparison);
                
                if (i < 0)
                    continue;
                
                match = new Substring(in input, i, value.Length);
                return true;
            }

            match = default;
            return false;
        }

        public static ComparisonToken CaseInsensitive(params string[] values) =>
            new(values, StringComparison.OrdinalIgnoreCase);
        
        public static ComparisonToken CaseInsensitive(string value) =>
            new(value, StringComparison.OrdinalIgnoreCase);
        
        public static ComparisonToken CaseSensitive(params string[] values) =>
            new(values, StringComparison.Ordinal);
        
        public static ComparisonToken CaseSensitive(string value) =>
            new(value, StringComparison.Ordinal);
    }
}
