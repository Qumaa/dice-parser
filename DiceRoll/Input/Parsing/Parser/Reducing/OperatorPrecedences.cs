using System.Collections;
using System.Collections.Generic;

namespace DiceRoll.Input.Parsing
{
    public sealed class OperatorPrecedences : IEnumerable<PrecedenceLevel>
    {
        private readonly List<PrecedenceLevel> _precedences = new();

        public void Add(int precedence, Associativity associativity)
        {
            if (TryInsertEmpty(precedence, associativity))
                return;
            
            InsertSorted(precedence, associativity);
        }

        private bool TryInsertEmpty(int precedence, Associativity associativity)
        {
            if (_precedences.Count is not 0)
                return false;

            _precedences.Add(new PrecedenceLevel(precedence, associativity));
            return true;

        }

        private void InsertSorted(int precedence, Associativity associativity)
        {
            // insert in descending order. Highest first, lowest last
            for (int i = 0; i < _precedences.Count; i++)
            {
                if (precedence == _precedences[i].Value)
                {
                    PrecedenceLevel level = _precedences[i];
                    bool left = associativity is Associativity.Left || level.HasAssociativity(Associativity.Left);
                    bool right = associativity is Associativity.Right || level.HasAssociativity(Associativity.Right);
                    _precedences[i] = new PrecedenceLevel(level.Value, left, right);
                    return;
                }

                if (precedence < _precedences[i].Value)
                    continue;
                
                _precedences.Insert(i, new PrecedenceLevel(precedence, associativity));
                return;
            }
        }

        public List<PrecedenceLevel>.Enumerator GetEnumerator() =>
            _precedences.GetEnumerator();
        
        IEnumerator<PrecedenceLevel> IEnumerable<PrecedenceLevel>.GetEnumerator() =>
            GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();
    }
}
