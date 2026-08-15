using System.Collections.Generic;

namespace DiceRoll.Input.Parsing
{
    public sealed class Or : ISelectiveGrammar
    {
        private readonly List<IGrammar> _grammars;
        
        public Or(IGrammar first, IGrammar second)
        {
            _grammars = new List<IGrammar>
            {
                first,
                second
            };
        }
        
        public GrammarProbe ProbeContext(ParseContext context)
        {
            foreach (IGrammar grammar in _grammars)
            {
                GrammarProbe probe = grammar.ProbeContext(context);

                if (probe.IsSuccessful)
                    return probe;
            }
            
            return GrammarProbe.Failed;
        }

        void IExtendableGrammar.Add(IGrammar grammar) =>
            _grammars.Add(grammar);
    }
}