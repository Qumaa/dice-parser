using System;

namespace DiceRoll
{
    public sealed class ComposerContext
    {
        private readonly Outcome[] _array;
        private int _count;

        public ComposerContext(int capacity)
        {
            _array = new Outcome[capacity];
            _count = 0;
        }
            
        public void Write(in Outcome outcome) =>
            _array[_count++] = outcome;

        public void Reset() =>
            _count = 0;

        public Outcome[] Read() =>
            CopyArray();

        public ComposerWrapper Include(INumeric numeric) =>
            new(numeric, this);

        private Outcome[] CopyArray()
        {
            if (_count is 0)
                return Array.Empty<Outcome>();
                
            Outcome[] copy = new Outcome[_count];
                
            Array.Copy(_array, copy, _count);

            return copy;
        }
    }
}
