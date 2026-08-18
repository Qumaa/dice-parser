using System;

namespace DiceRoll.Input.Parsing
{
    public class GrammarChain : IGrammar
    {
        private readonly IGrammar[] _chain;
        private readonly GrammarProbe[] _probes;
        private int _state;

        public int Links => _chain.Length;
        public int Progress => _state;
        public bool IsComplete => _state >= _chain.Length;

        public GrammarChain(IGrammar[] chain)
        {
            _chain = chain;
            _probes = new GrammarProbe[chain.Length];
        }
        
        public GrammarChain(IGrammar[] chain, GrammarProbe probe) : this(chain)
        {
            TryAdvanceWithProbe(probe);
        }

        public bool TryAdvance(ParseContext context, out GrammarProbe probe)
        {
            if (_state >= _chain.Length)
            {
                probe = null;
                return false;
            }

            IGrammar current = _chain[_state];
            int recognizedOffset = GetRecognizedOffset(context.GetUnrecognized());
            
            context.PushRecognized(recognizedOffset);
            probe = current.ProbeContext(context);
            context.PopRecognized(recognizedOffset);
            
            return TryAdvanceWithProbe(probe);
        }

        private int GetRecognizedOffset(Substring unrecognized)
        {
            if (_state <= 0)
                return 0;

            int end = _probes[0].RecognizedStart + _probes[0].RecognizedLength;

            for (int i = 1; i < _state; i++)
            {
                int end2 = _probes[i].RecognizedStart + _probes[i].RecognizedLength;

                end = Math.Max(end, end2);
            }

            return Math.Max(0, end - unrecognized.Start);
        }

        private bool TryAdvanceWithProbe(GrammarProbe probe)
        {
            if (!probe.IsSuccessful)
                return false;

            _probes[_state++] = probe;
            return true;
        }

        GrammarProbe IGrammar.ProbeContext(ParseContext context) =>
            _chain.ProbeAllSequentially(context);
    }
}
