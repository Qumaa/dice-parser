using System.Runtime.InteropServices;

namespace DiceRoll.Input.Parsing
{
    public sealed class OperandsAccess
    {
        private readonly Mapped<LinkedNode>[] _operands;
        private readonly OperandCaster[] _casters;
        private readonly Signature _operatorSignature;

        internal OperandsAccess(Mapped<LinkedNode>[] operands, OperandCaster[] casters, Signature operatorSignature)
        {
            _operands = operands;
            _casters = casters;
            _operatorSignature = operatorSignature;
        }

        public OperandsAccess Get<T>(int operandIndex, out T operand) where T : INode
        {
            INode node = _operands[operandIndex].Value.Node;

            if ((_casters[operandIndex] is null && OperandCaster.Default<INode, T>().TryCast(node, out operand)) ||
                (_casters[operandIndex].CastsTo(out OperandCaster<T> caster) && caster.TryCast(node, out operand)))
                return this;

            throw OperatorInvocationException.InvalidOperandCast(operandIndex, _operatorSignature, typeof(T));
        }
    }

    public static class OperandsAccessExtensions
    {
        public static ChainedOperandsAccess Sequential(this OperandsAccess operandsAccess) =>
            new(operandsAccess);
    }

    [StructLayout(LayoutKind.Auto)]
    public readonly ref struct ChainedOperandsAccess
    {
        private readonly OperandsAccess _operandsAccess;
        private readonly int _currentOperandIndex;

        public ChainedOperandsAccess(OperandsAccess operandsAccess) : this(operandsAccess, 0) { }

        private ChainedOperandsAccess(OperandsAccess operandsAccess, int currentOperandIndex)
        {
            _operandsAccess = operandsAccess;
            _currentOperandIndex = currentOperandIndex;
        }

        public ChainedOperandsAccess Get<T>(out T operand) where T : INode
        {
            _operandsAccess.Get(_currentOperandIndex, out operand);

            return new ChainedOperandsAccess(_operandsAccess, _currentOperandIndex + 1);
        }
    }
}
