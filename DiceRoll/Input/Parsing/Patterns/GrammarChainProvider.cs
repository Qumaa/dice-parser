using System;
using System.Collections.Generic;

namespace DiceRoll.Input.Parsing
{
    public sealed class GrammarChainProvider
    {
        private readonly Tagged<IGrammar[]>[] _chains;
        
        public GrammarChainProvider(Tagged<IGrammar[]>[] chains)
        {
            _chains = chains;
        }

        public Tagged<GrammarChain>[] GetChainsThatStartWith(ParseContext context, GrammarChainCollection collection, out int shortestRecognitionLength)
        {
            Tagged<IGrammar[]> single = default;
            GrammarProbe singleProbe = null;
            List<Tagged<IGrammar[]>> many = null;
            List<GrammarProbe> manyProbes = null;

            int shortestRecognition = -1;

            for (int i = 0; i < _chains.Length; i++)
            {
                if (collection.HasBeenDetected(i))
                    continue;
                
                Tagged<IGrammar[]> chain = _chains[i];
                GrammarProbe probe = chain.Value[0].ProbeContext(context);

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

            void _AddChain(Tagged<IGrammar[]> grammars, GrammarProbe probe)
            {
                if (single.Value is null)
                {
                    single = grammars;
                    singleProbe = probe;
                    return;
                }

                if (many is null)
                {
                    many = new List<Tagged<IGrammar[]>>(capacity: _chains.Length)
                    {
                        single,
                        grammars
                    };
                    manyProbes = new List<GrammarProbe>(capacity: _chains.Length)
                    {
                        singleProbe,
                        probe
                    };
                    return;
                }
                
                many.Add(grammars);
                manyProbes!.Add(probe);
            }

            Tagged<GrammarChain>[] _ToArray()
            {
                if (many is { Count: > 0 })
                {
                    Tagged<GrammarChain>[] chains = new Tagged<GrammarChain>[many.Count];

                    for (int i = 0; i < many.Count; i++)
                        chains[i] = new Tagged<GrammarChain>(many[i].Tag, new GrammarChain(many[i].Value, manyProbes![i]));

                    return chains;
                }

                if (single.Value is not null)
                    return new[] { new Tagged<GrammarChain>(single.Tag, new GrammarChain(single.Value, singleProbe)) };
                
                return Array.Empty<Tagged<GrammarChain>>();
            }
        }
    }
}
