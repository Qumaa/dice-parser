using System;
using System.Collections.Generic;

namespace DiceRoll.Input
{
    public sealed class FormulaAccumulator
    {
        private readonly List<string> _accumulatedFormula = new();
        private int _formulaLength = 0;

        public FormulaSubstring<T> Wrap<T>(in T element, in Substring token) =>
            Wrap(in element, token.Start, token.Length);
            
        public FormulaSubstring<T> Wrap<T>(in T element, int start, int length) =>
            new(in element, _formulaLength + start, length);

        public void Accumulate(string formulaPiece)
        {
            if (_accumulatedFormula.Count > 0 && !char.IsWhiteSpace(_accumulatedFormula[^1][^1]))
                _accumulatedFormula.Add(" ");
                
            _accumulatedFormula.Add(formulaPiece);
            _formulaLength += formulaPiece.Length + 1;
        }

        public Substring AccumulateAndBuild(in Substring substring)
        {
            int start = substring.Start + _formulaLength;
                
            Range range = new(new Index(start), new Index(start + substring.Length));
                
            Accumulate(substring.Source);

            return AccumulatedFormulaSubstring(range);
        }

        public Substring AccumulatedFormulaSubstring(in Range tokenRange)
        {
            string source = string.Concat(_accumulatedFormula);
            (int Offset, int Length) tuple = tokenRange.GetOffsetAndLength(source.Length);

            return new Substring(source, tuple.Offset, tuple.Length);
        }
            
        public Substring AccumulatedFormulaSubstring<T>(in FormulaSubstring<T> token) =>
            AccumulatedFormulaSubstring(token.Range);

        public void Clear()
        {
            _accumulatedFormula.Clear();
            _formulaLength = 0;
        }

        public FormulaSubstringsStack<T> CreateStack<T>() =>
            new(this);
    }
}
