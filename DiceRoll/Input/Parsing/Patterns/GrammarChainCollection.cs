using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace DiceRoll.Input.Parsing
{
    public sealed class GrammarChainCollection
    {
        private readonly List<Tagged<GrammarChain>> _active = new();
        private readonly HashSet<int> _previouslyDetectedIds = new();
        private readonly List<CompletedChain> _completed = new();
        
        public void AddNewChains(IEnumerable<Tagged<GrammarChain>> chains) =>
            _active.AddRange(chains);

        public void MarkChainStartAsDetected(int id) =>
            _previouslyDetectedIds.Add(id);

        public bool HasBeenDetected(int id) =>
            _previouslyDetectedIds.Contains(id);

        public void FlushDetectedChainStarts() =>
            _previouslyDetectedIds.Clear();

        public Enumerable EnumerateActiveChains() =>
            new(this);

        [StructLayout(LayoutKind.Auto)]
        public readonly struct GrammarChainHandle
        {
            private readonly GrammarChainCollection _chainCollection;
            private readonly int _index;

            public GrammarChainHandle(GrammarChainCollection chainCollection, int index)
            {
                _chainCollection = chainCollection;
                _index = index;
            }

            public int TryAdvanceOrRemove(ParseContext context)
            {
                Tagged<GrammarChain> chain = _chainCollection._active[_index];

                if (!chain.Value.TryAdvance(context, out GrammarProbe probe))
                {
                    RemoveReferencedActiveChain();
                    return -1;
                }

                if (chain.Value.TryConvertToCompleteChain(chain.Tag, out CompletedChain completedChain))
                {
                    RemoveReferencedActiveChain();
                    _chainCollection._completed.Add(completedChain);
                }
                    
                return context.GetRecognizedOffset(probe);
            }

            private void RemoveReferencedActiveChain() =>
                _chainCollection._active.RemoveAt(_index);
        }

        public readonly struct Enumerable : IEnumerable<GrammarChainHandle>
        {
            private readonly GrammarChainCollection _chainCollection;
            
            public Enumerable(GrammarChainCollection chainCollection)
            {
                _chainCollection = chainCollection;
            }

            public Enumerator GetEnumerator() =>
                new(_chainCollection);

            IEnumerator<GrammarChainHandle> IEnumerable<GrammarChainHandle>.GetEnumerator() =>
                GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() =>
                GetEnumerator();
        }

        public struct Enumerator : IEnumerator<GrammarChainHandle>
        {
            private readonly GrammarChainCollection _chainCollection;
            private int _index;
            
            public Enumerator(GrammarChainCollection chainCollection)
            {
                _chainCollection = chainCollection;
                _index = chainCollection._active.Count;
            }

            public bool MoveNext() =>
                --_index >= 0;

            public void Reset() =>
                throw new System.NotSupportedException();

            public GrammarChainHandle Current => new(_chainCollection, _index);
            object IEnumerator.Current => Current;

            public void Dispose() { }
        }

        public CompletedChain[] GetCompletedChains() =>
            _completed.ToArray();
    }
}
