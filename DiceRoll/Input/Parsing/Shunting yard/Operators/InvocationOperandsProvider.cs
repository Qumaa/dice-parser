using System;

namespace DiceRoll.Input.Parsing
{
    internal sealed class InvocationOperandsProvider
    {
        private readonly ShuntingYardState _state;
        
        public InvocationOperandsProvider(ShuntingYardState state)
        {
            _state = state;
        }

        public Mapped<LinkedNode>[] PopExcessiveOperandsIfAny(in Operator @operator)
        {
            OperatorInvocationBehaviour behaviour = @operator.InvocationBehaviour;
            int operands = _state.Operands.Count;
            int rightOperands = operands - @operator.OperandsPosition;

            if (rightOperands == behaviour.RightArity)
                return Array.Empty<Mapped<LinkedNode>>();

            int excessiveOperands = rightOperands - behaviour.RightArity;
            return PopOperandsDirect(excessiveOperands);
        }
        
        public Mapped<LinkedNode>[] PopOperandsFor(in Operator @operator)
        {
            ThrowIfCannotPop(@operator.InvocationBehaviour);
            
            return PopOperandsDirect(@operator.InvocationBehaviour.Arity);
        }

        public void RestoreExcessiveOperands(Mapped<LinkedNode>[] excessiveOperands)
        {
            foreach (Mapped<LinkedNode> operand in excessiveOperands)
                _state.Operands.Push(in operand);
        }

        private Mapped<LinkedNode>[] PopOperandsDirect(int count)
        {
            Mapped<LinkedNode>[] operands = new Mapped<LinkedNode>[count];
                
            for (int i = count - 1; i >= 0; i--)
                operands[i] = _state.Operands.Pop();
            
            return operands;
        }

        private void ThrowIfCannotPop(OperatorInvocationBehaviour behaviour)
        {
            int operands = _state.Operands.Count;
            if (operands < behaviour.Arity)
                throw OperatorInvocationException.NotEnoughOperands(behaviour.Arity, operands);
        }
    } 
}
