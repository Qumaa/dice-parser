using System;

namespace DiceRoll.Input.Parsing
{
    public sealed class GrammarGraph
    {
        private readonly GrammarChainProvider _chainProvider;
        private readonly GrammarCollection _collection;
        
        public GrammarGraph(GrammarChainProvider chainProvider, GrammarCollection collection)
        {
            _chainProvider = chainProvider;
            _collection = collection;
        }

        public CompletedChain[] Parse(string input)
        {
            ParseContext context = new(input, _collection);
            GrammarChainCollection chainCollection = new();

            do
            {
                int recognizedInActive = AdvanceDetectedChains(context, chainCollection);
                int recognizedInNew = DetectAndStartNewChains(context, chainCollection);
                int recognized = Math.Max(recognizedInActive, recognizedInNew);

                context.PushRecognized(recognized);

                if (recognized > 0)
                    chainCollection.FlushDetectedChainStarts();
            } while (context.HasUnrecognizedInput);

            return chainCollection.GetCompletedChains();
        }

        // todo: re-read grammars that return 0 length probes (means an optional grammar)
        // letting it into the next loop will cut off the rest of chains
        private int AdvanceDetectedChains(ParseContext context, GrammarChainCollection chainCollection)
        {
            int shortestRecognitionLength = -1;
            
            foreach (var handle in chainCollection.EnumerateActiveChains())
            {
                int recognizedLength = handle.TryAdvanceOrRemove(context);
                
                if (recognizedLength < 0)
                    continue;

                shortestRecognitionLength = shortestRecognitionLength < 0 ?
                    recognizedLength :
                    Math.Min(shortestRecognitionLength, recognizedLength);
            }

            return shortestRecognitionLength < 0 ? 0 : shortestRecognitionLength;
        }

        private int DetectAndStartNewChains(
            ParseContext context,
            GrammarChainCollection collection
            )
        {
            Tagged<GrammarChain>[] chains = _chainProvider.GetChainsThatStartWith(context, collection, out int shortestRecognitionLength);
            collection.AddNewChains(chains);
            return shortestRecognitionLength;
        }
    }
}
