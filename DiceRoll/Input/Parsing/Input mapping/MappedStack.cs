using System;
using System.Collections.Generic;

namespace DiceRoll.Input.Parsing
{
    public sealed class MappedStack<T>
    {
        private readonly Stack<Mapped<T>> _stack;
        private readonly InputMapper _inputMapper;
            
        public MappedStack(InputMapper inputMapper)
        {
            _inputMapper = inputMapper;

            _stack = new Stack<Mapped<T>>();
        }

        public int Count => _stack.Count;

        public void MapAndPush(in T value, in Substring substring) =>
            Push(_inputMapper.Map(in value, in substring));
        
        public void Push(in T value, in Range range) =>
            Push(new Mapped<T>(in value, range));

        public void Push(in Mapped<T> value) =>
            _stack.Push(value);
            
        public Mapped<T> Pop() =>
            _stack.Pop();

        public T PopValue() =>
            Pop().Value;

        public Mapped<T> Peek() =>
            _stack.Peek();
        public T PeekValue() =>
            Peek().Value;

        public bool TryPeek(out Mapped<T> result) =>
            _stack.TryPeek(out result);

        public bool TryPeek(out T result)
        {
            if (TryPeek(out Mapped<T> context))
            {
                result = context.Value;
                return true;
            }

            result = default;
            return false;
        }

        public bool TryPop(out Mapped<T> result) =>
            _stack.TryPop(out result);

        public bool TryPop(out T result)
        {
            if (TryPop(out Mapped<T> context))
            {
                result = context.Value;
                return true;
            }

            result = default;
            return false;
        }
    }
}
