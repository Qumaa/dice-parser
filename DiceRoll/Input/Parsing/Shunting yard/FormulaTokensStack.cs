using System.Collections.Generic;

namespace DiceRoll.Input
{
    public sealed class FormulaTokensStack<T>
    {
        private readonly Stack<FormulaToken<T>> _stack;
        private readonly FormulaAccumulator _formulaAccumulator;
            
        public FormulaTokensStack(FormulaAccumulator formulaAccumulator)
        {
            _formulaAccumulator = formulaAccumulator;

            _stack = new Stack<FormulaToken<T>>();
        }

        public int Count => _stack.Count;

        public void Push(in T value, in Substring context) =>
            Push(_formulaAccumulator.Tokenize(in value, in context));
        public void Push(in T value, int start, int length) =>
            Push(new FormulaToken<T>(in value, start, length));

        public void PushWithoutToken(in T value) =>
            _stack.Push(FormulaToken<T>.NotTokenized(value));

        public void Push(in FormulaToken<T> context) =>
            _stack.Push(context);
            
        public FormulaToken<T> Pop() =>
            _stack.Pop();

        public T PopValue() =>
            Pop().Value;

        public FormulaToken<T> Peek() =>
            _stack.Peek();
        public T PeekValue() =>
            Peek().Value;

        public bool TryPeek(out FormulaToken<T> context) =>
            _stack.TryPeek(out context);

        public bool TryPeek(out T value)
        {
            if (TryPeek(out FormulaToken<T> context))
            {
                value = context.Value;
                return true;
            }

            value = default;
            return false;
        }

        public bool TryPop(out FormulaToken<T> context) =>
            _stack.TryPop(out context);

        public bool TryPop(out T value)
        {
            if (TryPop(out FormulaToken<T> context))
            {
                value = context.Value;
                return true;
            }

            value = default;
            return false;
        }
    }
}
