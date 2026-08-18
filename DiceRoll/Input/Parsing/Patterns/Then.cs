using System.Collections;
using System.Collections.Generic;

namespace DiceRoll.Input.Parsing
{
    public sealed class Then : IChainableGrammar
    {
        private readonly List<IGrammar> _grammars;

        public Then(IGrammar first, IGrammar second)
        {
            _grammars = new List<IGrammar>
            {
                first,
                second
            };
        }

        public GrammarProbe ProbeContext(ParseContext context) =>
            _grammars.ProbeAllSequentially(context);

        IEnumerator<IGrammar> IEnumerable<IGrammar>.GetEnumerator() =>
            _grammars.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            ((IEnumerable) _grammars).GetEnumerator();

        void IExtendableGrammar.Add(IGrammar item) =>
            _grammars.Add(item);
    }
}