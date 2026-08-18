using System;
using System.Collections.Generic;

namespace DiceRoll.Input.Parsing
{
    public sealed class GrammarChainProvider
    {
        private readonly IGrammar[][] _chains;
        
        public GrammarChainProvider(IGrammar[][] chains)
        {
            _chains = chains;
        }

        public GrammarChain[] GetChainsThatStartWith(ParseContext context, GrammarChainCollection collection, out int shortestRecognitionLength)
        {
            IGrammar[] single = null;
            GrammarProbe singleProbe = null;
            List<IGrammar[]> many = null;
            List<GrammarProbe> manyProbes = null;

            int shortestRecognition = -1;

            for (int i = 0; i < _chains.Length; i++)
            {
                if (collection.HasBeenDetected(i))
                    continue;
                
                IGrammar[] chain = _chains[i];
                GrammarProbe probe = chain[0].ProbeContext(context);

                if (!probe.IsSuccessful)
                    continue;

                int recognizedOffset = context.GetRecognizedOffset(probe);

                shortestRecognition = shortestRecognition < 0 ?
                    recognizedOffset :
                    Math.Min(shortestRecognition, recognizedOffset);

                _AddChain(chain, probe);
                collection.MarkChainStartAsDetected(i);
            }

            shortestRecognitionLength = shortestRecognition;
            return _ToArray();

            void _AddChain(IGrammar[] grammars, GrammarProbe probe)
            {
                if (single is null)
                {
                    single = grammars;
                    singleProbe = probe;
                    return;
                }

                if (many is null)
                {
                    many = new List<IGrammar[]>(capacity: _chains.Length)
                    {
                        single,
                        grammars
                    };
                    manyProbes = new List<GrammarProbe>(capacity: _chains.Length)
                    {
                        singleProbe,
                        probe
                    };
                }
                
                many.Add(grammars);
                manyProbes!.Add(probe);
            }

            GrammarChain[] _ToArray()
            {
                if (many is { Count: > 0 })
                {
                    GrammarChain[] chains = new GrammarChain[many.Count];

                    for (int i = 0; i < many.Count; i++)
                        chains[i] = new GrammarChain(many[i], manyProbes![i]);

                    return chains;
                }

                if (single is not null)
                    return new[] { new GrammarChain(single, singleProbe) };
                
                return Array.Empty<GrammarChain>();
            }
        }
    }
}
