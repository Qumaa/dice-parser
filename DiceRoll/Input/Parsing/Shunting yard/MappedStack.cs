using System.Collections.Generic;

namespace DiceRoll.Input
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

        public void Push(in T value, in Substring context) =>
            Push(_inputMapper.Map(in value, in context));
        
        public void Push(in T value, int start, int length) =>
            Push(new Mapped<T>(in value, start, length));

        public void Push(in Mapped<T> context) =>
            _stack.Push(context);
            
        public Mapped<T> Pop() =>
            _stack.Pop();

        public T PopValue() =>
            Pop().Value;

        public Mapped<T> Peek() =>
            _stack.Peek();
        public T PeekValue() =>
            Peek().Value;

        public bool TryPeek(out Mapped<T> context) =>
            _stack.TryPeek(out context);

        public bool TryPeek(out T value)
        {
            if (TryPeek(out Mapped<T> context))
            {
                value = context.Value;
                return true;
            }

            value = default;
            return false;
        }

        public bool TryPop(out Mapped<T> context) =>
            _stack.TryPop(out context);

        public bool TryPop(out T value)
        {
            if (TryPop(out Mapped<T> context))
            {
                value = context.Value;
                return true;
            }

            value = default;
            return false;
        }
    }
}
