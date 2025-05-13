using System.Collections;
using System.Collections.Generic;

namespace DiceRoll
{
    public sealed class CompositeEvaluation : IEnumerable<Outcome>
    {
        public readonly Outcome Outcome;
        private readonly Outcome[] _source;
        
        public CompositeEvaluation(Outcome outcome, Outcome[] source)
        {
            _source = source;
            Outcome = outcome;
        }
        
        public IEnumerator<Outcome> GetEnumerator() =>
            (_source as IEnumerable<Outcome>).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();
    }
}
