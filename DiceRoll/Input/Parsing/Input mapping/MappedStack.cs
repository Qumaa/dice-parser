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

        public Mapped<T> Peek() =>
            _stack.Peek();

        public bool TryPeek(out Mapped<T> result) =>
            _stack.TryPeek(out result);

        public bool TryPop(out Mapped<T> result) =>
            _stack.TryPop(out result);
    }

    public static class MappedStackExtensions
    {
        public static Mapped<T>[] PopAll<T>(this MappedStack<T> stack) =>
            stack.PopMany(stack.Count);

        public static Mapped<T>[] PopMany<T>(this MappedStack<T> stack, int count)
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(count, stack.Count);
            
            Mapped<T>[] popped = new Mapped<T>[count];

            for (int i = count - 1; i >= 0; i--)
                popped[i] = stack.Pop();

            return popped;
        }
        
        public static T PopValue<T>(this MappedStack<T> stack) =>
            stack.Pop().Value;
        
        public static T PeekValue<T>(this MappedStack<T> stack) =>
            stack.Peek().Value;
        
        public static bool TryPeek<T>(this MappedStack<T> stack, out T result)
        {
            if (stack.TryPeek(out Mapped<T> context))
            {
                result = context.Value;
                return true;
            }

            result = default;
            return false;
        }
        
        public static bool TryPop<T>(this MappedStack<T> stack, out T result)
        {
            if (stack.TryPop(out Mapped<T> context))
            {
                result = context.Value;
                return true;
            }

            result = default;
            return false;
        }
    }
}
