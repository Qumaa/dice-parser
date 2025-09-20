using System;

namespace DiceRoll.Input.Parsing
{
    public sealed class OperandsAccess
    {
        private readonly Mapped<LinkedNode>[] _operands;
        private readonly OperandCaster[] _casters;
        private readonly Signature _operatorSignature;

        public OperandsAccess(Mapped<LinkedNode>[] operands, OperandCaster[] casters, Signature operatorSignature)
        {
            _operands = operands;
            _casters = casters;
            _operatorSignature = operatorSignature;
        }

        public OperandsAccess Get<T>(int operandIndex, out T operand) where T : INode
        {
            if (!_operands[operandIndex].Value.IsOperand(out INode node))
            {
                throw new Exception();
            }

            if ((_casters[operandIndex] is null && OperandCaster.Default<INode, T>().TryCast(node, out operand)) ||
                (_casters[operandIndex].CastsTo(out OperandCaster<T> caster) && caster.TryCast(node, out operand)))
                return this;

            // todo exceptions
            throw new Exception();
        }
    }
}
