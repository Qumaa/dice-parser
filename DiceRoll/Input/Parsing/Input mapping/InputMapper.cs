using System;
using System.Collections.Generic;

namespace DiceRoll.Input.Parsing
{
    public sealed class InputMapper
    {
        private readonly List<string> _accumulatedInput = new();
        private int _inputLength = 0;
        private int _previousLength = 0;

        public Mapped<T> Map<T>(in T element, in Substring token) =>
            Map(in element, token.Start, token.Length);
        
        public Mapped<T> Map<T>(in T element, string token) =>
            Map(in element, 0, token.Length);
            
        public Mapped<T> Map<T>(in T element, int start, int length) =>
            new(in element, Map(start, length));
        
        public Range Map(in Substring token) =>
            Map(token.Start, token.Length);
        
        public Range Map(string token) =>
            Map(0, token.Length);
            
        public Range Map(int start, int length)
        {
            int s = _previousLength + start;
            int e = s + length;
            
            return new Range(new Index(s), new Index(e));
        }

        public void Append(string input)
        {
            if (_accumulatedInput.Count > 0 && !char.IsWhiteSpace(_accumulatedInput[^1][^1]))
                _accumulatedInput.Add(" ");
                
            _accumulatedInput.Add(input);
            _previousLength = _inputLength;
            _inputLength += input.Length + 1;
        }

        public Substring MapAndGetSubstringOf(in Substring substring) =>
            GetSubstringOf(Map(in substring));

        public Substring GetSubstringOf<T>(in Mapped<T> token) =>
            GetSubstringOf(token.Range);

        public Substring GetSubstringOf(in Range mappedRange)
        {
            string source = ConcatenateInput();
            (int Offset, int Length) tuple = mappedRange.GetOffsetAndLength(source.Length);

            return new Substring(source, tuple.Offset, tuple.Length);
        }

        public void Clear()
        {
            _accumulatedInput.Clear();
            _inputLength = 0;
            _previousLength = 0;
        }

        public MappedStack<T> CreateLinkedStack<T>() =>
            new(this);

        private string ConcatenateInput()
        {
            if (_inputLength is 0)
                return string.Empty;
            
            char[] chars = new char[_inputLength - 1];

            int i = 0;
            foreach (string piece in _accumulatedInput)
            foreach (char c in piece)
                chars[i++] = c;
            
            return new string(chars);
        }
    }
}
