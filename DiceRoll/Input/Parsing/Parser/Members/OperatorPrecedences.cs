using System.Collections;
using System.Collections.Generic;

namespace DiceRoll.Input.Parsing
{
    public sealed class OperatorPrecedences : IEnumerable<int>
    {
        private readonly List<int> _precedences = new();

        public void Add(int precedence)
        {
            if (TryInsertEmpty(precedence))
                return;
            
            InsertSorted(precedence);
        }

        public void Clear() =>
            _precedences.Clear();

        private bool TryInsertEmpty(int precedence)
        {
            if (_precedences.Count is not 0)
                return false;

            _precedences.Add(precedence);
            return true;

        }

        private void InsertSorted(int precedence)
        {
            // insert in descending order. Highest first, lowest last
            for (int i = 0; i < _precedences.Count; i++)
            {
                if (precedence == _precedences[i])
                    return;
                
                if (precedence < _precedences[i])
                    continue;
                
                _precedences.Insert(i, precedence);
                return;
            }
        }

        public List<int>.Enumerator GetEnumerator() =>
            _precedences.GetEnumerator();
        
        IEnumerator<int> IEnumerable<int>.GetEnumerator() =>
            GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();
    }
}
