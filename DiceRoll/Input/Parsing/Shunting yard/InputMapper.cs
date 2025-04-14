using System;
using System.Collections.Generic;

namespace DiceRoll.Input
{
    public sealed class InputMapper
    {
        private readonly List<string> _accumulatedInput = new();
        private int _formulaLength = 0;
        private int _previousLength = 0;

        public Mapped<T> Map<T>(in T element, in Substring token) =>
            Map(in element, token.Start, token.Length);
            
        public Mapped<T> Map<T>(in T element, int start, int length) =>
            new(in element, _previousLength + start, length);

        public void Append(string input)
        {
            if (_accumulatedInput.Count > 0 && !char.IsWhiteSpace(_accumulatedInput[^1][^1]))
                _accumulatedInput.Add(" ");
                
            _accumulatedInput.Add(input);
            _previousLength = _formulaLength;
            _formulaLength += input.Length + 1;
        }

        public Substring AppendAndGetSubstringOf(in Substring substring)
        {
            int start = substring.Start + _previousLength;
                
            Range range = new(new Index(start), new Index(start + substring.Length));
                
            Append(substring.Source);

            return SubstringFromRange(range);
        }

        public Substring GetSubstringOf<T>(in Mapped<T> token) =>
            SubstringFromRange(token.Range);

        public void Clear()
        {
            _accumulatedInput.Clear();
            _formulaLength = 0;
            _previousLength = 0;
        }

        private Substring SubstringFromRange(in Range mappedRange)
        {
            string source = ConcatenateFormula();
            (int Offset, int Length) tuple = mappedRange.GetOffsetAndLength(source.Length);

            return new Substring(source, tuple.Offset, tuple.Length);
        }

        public MappedStack<T> CreateLinkedStack<T>() =>
            new(this);

        private string ConcatenateFormula()
        {
            if (_formulaLength is 0)
                return string.Empty;
            
            char[] chars = new char[_formulaLength - 1];

            int i = 0;
            foreach (string piece in _accumulatedInput)
            foreach (char c in piece)
                chars[i++] = c;
            
            return new string(chars);
        }
    }
}
