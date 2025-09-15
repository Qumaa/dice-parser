using System;

namespace DiceRoll.Input.Parsing
{
    internal sealed class ShuntingYardOperators
    {
        private readonly ShuntingYardState _state;

        public ShuntingYardOperators(ShuntingYardState state)
        {
            _state = state;
        }

        public void Push(in Operator @operator, in Substring context)
        {
            if (@operator.IsOpenParenthesis)
            {
                _state.Operators.MapAndPush(in @operator, context);
                return;
            }

            Mapped<Operator> mapped = _state.Mapper.Map(in @operator, in context);

            OperatorInvoker invoker = @operator.Invoker;

            if (invoker.RightArity is 0)
            {
                // invoke immediately using existing operands
                InvokeOperatorOrThrow(in mapped);
                return;
            }

            if (invoker is { RightArity: 1, LeftArity: > 0 })
            {
                // resolve using default shunting-yard mechanism
                _state.Operators.Push(in mapped);
                return;
            }

            // delay until more operands are pushed
            _state.DelayedOperators.MapAndPush(
                new DelayedOperator(@operator.Invoker, _state.ParenthesisLevel, _state.Operands.Count),
                in context
                );
        }

        public bool TryPeek(out Operator @operator) =>
            _state.Operators.TryPeek(out @operator);
        
        public bool TryPeek(out Mapped<DelayedOperator> operatorToken) =>
            _state.DelayedOperators.TryPeek(out operatorToken);

        public bool TryPop(out Mapped<Operator> operatorToken) =>
            _state.Operators.TryPop(out operatorToken);

        public Mapped<Operator> Pop() =>
            _state.Operators.Pop();

        public void InvokeOperatorOrThrow(in Mapped<Operator> operatorToken)
        {
            try
            {
                InvokeOperator(operatorToken.Value.Invoker, in operatorToken.Range);
            }
            catch (Exception e)
            {
                throw _state.MapException(in operatorToken, e);
            }
        }

        public void TryInvokeDelayedOperators()
        {
            while (_state.DelayedOperators.TryPeek(out DelayedOperator token) &&
                   token.CapturedParenthesisLevel >= _state.ParenthesisLevel &&
                   token.CapturedOperands + token.Invoker.RightArity <= _state.Operands.Count)
                InvokeOperatorOrThrow(_state.DelayedOperators.Pop());
        }

        public void InvokeAfterDelayedOperators(in Mapped<Operator> invoker)
        {
            TryInvokeDelayedOperators();
            InvokeOperatorOrThrow(in invoker);
        }

        private void InvokeOperatorOrThrow(in Mapped<DelayedOperator> operatorToken)
        {
            try
            {
                InvokeOperator(operatorToken.Value.Invoker, in operatorToken.Range);
            }
            catch (Exception e)
            {
                throw _state.MapException(in operatorToken, e);
            }
        }

        private void InvokeOperator(OperatorInvoker invoker, in Range operatorRange)
        {
            int arity = invoker.Arity;
            
            if (_state.Operands.Count < arity)
                throw new OperatorInvocationException(ParsingErrorMessages.OperandsExpected(arity, _state.Operands.Count));

            OperandsStackAccess access = new(_state.Operands, arity, in operatorRange);
            
            INode result = invoker.Invoke(access);
            
            access.PushResult(result);
        }
    }
}
