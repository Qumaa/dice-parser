using System;
using System.Collections.Generic;

namespace DiceRoll.Input.Parsing
{
    public sealed class InputMapper
    {
        private readonly List<string> _accumulatedInput = new();
        private int _inputLength;
        private int _previousLength;

        public int InputLength => _inputLength;
            
        public Range Map(int start, int length)
        {
            int rangeStart = _previousLength + start;
            int rangeEnd = rangeStart + length;
            
            return rangeStart..rangeEnd;
        }

        public void Append(string input)
        {
            AddSpaceIfNeeded();
                
            _accumulatedInput.Add(input);
            _previousLength = _inputLength;
            _inputLength += input.Length + 1;
        }

        private void AddSpaceIfNeeded()
        {
            if (_accumulatedInput.Count > 0 && !char.IsWhiteSpace(_accumulatedInput[^1][^1]))
                _accumulatedInput.Add(" ");
        }

        public void Clear()
        {
            _accumulatedInput.Clear();
            _inputLength = 0;
            _previousLength = 0;
        }

        public SubstringMapper BuildSubstringMapper() =>
            new(BuildSourceString());

        private string BuildSourceString()
        {
            if (_inputLength is 0)
                return string.Empty;
            
            char[] chars = new char[_inputLength - 1];

            int i = 0;
            foreach (string piece in _accumulatedInput)
            {
                piece.CopyTo(0, chars, i, piece.Length);
                i += piece.Length;
            }

            return new string(chars);
        }
    }

    internal static class InputMapperExtensions
    {
        public static Mapped<T> Map<T>(this InputMapper mapper, in T element, in Substring token) =>
            mapper.Map(in element, token.Start, token.Length);
        
        public static Mapped<T> Map<T>(this InputMapper mapper, in T element, string token) =>
            mapper.Map(in element, 0, token.Length);
            
        public static Mapped<T> Map<T>(this InputMapper mapper, in T element, int start, int length) =>
            new(in element, mapper.Map(start, length));
        
        public static Range Map(this InputMapper mapper, in Substring token) =>
            mapper.Map(token.Start, token.Length);
        
        public static Range Map(this InputMapper mapper, string token) =>
            mapper.Map(0, token.Length);
        
        public static Substring GetSubstringOf<T>(this InputMapper mapper, in Mapped<T> mapped) =>
            mapper.BuildSubstringMapper().GetSubstringOf(mapped);
        
        public static Substring GetSubstringOf(this InputMapper mapper, in Range mapped) =>
            mapper.BuildSubstringMapper().GetSubstring(mapped);

        public static Substring MapAndGetSubstringOf(this InputMapper mapper, in Substring substring) =>
            mapper.GetSubstringOf(mapper.Map(in substring));
    }
}
