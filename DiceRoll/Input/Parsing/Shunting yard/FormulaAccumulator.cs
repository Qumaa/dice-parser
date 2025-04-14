using System;
using System.Collections.Generic;

namespace DiceRoll.Input
{
    public sealed class FormulaAccumulator
    {
        private readonly List<string> _accumulatedFormula = new();
        private int _formulaLength = 0;
        private int _previousLength = 0;

        public FormulaToken<T> Tokenize<T>(in T element, in Substring token) =>
            Tokenize(in element, token.Start, token.Length);
            
        public FormulaToken<T> Tokenize<T>(in T element, int start, int length) =>
            new(in element, _previousLength + start, length);

        public void Append(string formulaPiece)
        {
            if (_accumulatedFormula.Count > 0 && !char.IsWhiteSpace(_accumulatedFormula[^1][^1]))
                _accumulatedFormula.Add(" ");
                
            _accumulatedFormula.Add(formulaPiece);
            _previousLength = _formulaLength;
            _formulaLength += formulaPiece.Length + 1;
        }

        public Substring AppendAndGetSubstring(in Substring substring)
        {
            int start = substring.Start + _previousLength;
                
            Range range = new(new Index(start), new Index(start + substring.Length));
                
            Append(substring.Source);

            return GetFormulaSubstring(range);
        }

        public Substring GetFormulaSubstring(in Range tokenRange)
        {
            string source = ConcatenateFormula();
            (int Offset, int Length) tuple = tokenRange.GetOffsetAndLength(source.Length);

            return new Substring(source, tuple.Offset, tuple.Length);
        }

        private string ConcatenateFormula()
        {
            if (_formulaLength is 0)
                return string.Empty;
            
            char[] chars = new char[_formulaLength - 1];

            int i = 0;
            foreach (string piece in _accumulatedFormula)
            foreach (char c in piece)
                chars[i++] = c;
            
            return new string(chars);
        }

        public Substring GetFormulaSubstring<T>(in FormulaToken<T> token) =>
            GetFormulaSubstring(token.Range);

        public void Clear()
        {
            _accumulatedFormula.Clear();
            _formulaLength = 0;
            _previousLength = 0;
        }

        public FormulaTokensStack<T> CreateStack<T>() =>
            new(this);
    }
}
