using System.Collections.Generic;

namespace DiceRoll.Input
{
    public sealed class FormulaSubstringsStack<T>
    {
        private readonly Stack<FormulaSubstring<T>> _stack;
        private readonly FormulaAccumulator _formulaAccumulator;
            
        public FormulaSubstringsStack(FormulaAccumulator formulaAccumulator)
        {
            _formulaAccumulator = formulaAccumulator;

            _stack = new Stack<FormulaSubstring<T>>();
        }

        public int Count => _stack.Count;

        public void Push(in T value, in Substring context) =>
            Push(_formulaAccumulator.Wrap(in value, in context));
        public void Push(in T value, int start, int length) =>
            Push(_formulaAccumulator.Wrap(in value, start, length));

        public void PushWithoutContext(in T value) =>
            _stack.Push(FormulaSubstring<T>.Inexpressable(value));

        public void Push(in FormulaSubstring<T> context) =>
            _stack.Push(context);
            
        public FormulaSubstring<T> Pop() =>
            _stack.Pop();

        public T PopValue() =>
            Pop().Value;

        public FormulaSubstring<T> Peek() =>
            _stack.Peek();
        public T PeekValue() =>
            Peek().Value;

        public bool TryPeek(out FormulaSubstring<T> context) =>
            _stack.TryPeek(out context);

        public bool TryPeek(out T value)
        {
            if (TryPeek(out FormulaSubstring<T> context))
            {
                value = context.Value;
                return true;
            }

            value = default;
            return false;
        }

        public bool TryPop(out FormulaSubstring<T> context) =>
            _stack.TryPop(out context);

        public bool TryPop(out T value)
        {
            if (TryPop(out FormulaSubstring<T> context))
            {
                value = context.Value;
                return true;
            }

            value = default;
            return false;
        }
    }
}
