using System;

namespace DiceRoll.Input.Parsing.Deprecated
{
    internal sealed class InvocationOperandsProvider
    {
        private readonly ShuntingYardState _state;
        
        public InvocationOperandsProvider(ShuntingYardState state)
        {
            _state = state;
        }

        public Mapped<LinkedNode>[] LiftExcessiveOperandsIfAny(int operatorPosition, int rightArity)
        {
            int operands = _state.Operands.Count;
            int rightOperands = operands - operatorPosition;

            if (rightOperands == rightArity)
                return Array.Empty<Mapped<LinkedNode>>();

            int excessiveOperands = rightOperands - rightArity;
            return PopOperandsDirect(excessiveOperands);
        }
        
        public Mapped<LinkedNode>[] PopOperands(int arity)
        {
            ThrowIfCannotPop(arity);
            
            return PopOperandsDirect(arity);
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

        private void ThrowIfCannotPop(int arity)
        {
            int operands = _state.Operands.Count;
            if (operands < arity)
                throw OperatorInvocationException.NotEnoughOperands(arity, operands);
        }
    } 
}
