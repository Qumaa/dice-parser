using System;

namespace DiceRoll.Input.Parsing.Deprecated
{
    internal sealed class InvocationInfoProvider
    {
        private readonly OperandCastingTable _castingTable;
        
        public InvocationInfoProvider(OperandCastingTable castingTable)
        {
            _castingTable = castingTable;
        }

        public bool TryGetInvocationInfo(in Mapped<Operator> @operator, out InvocationInfo info)
        {
            throw new NotImplementedException();
        }

        private bool InvokableWith(Mapped<LinkedNode>[] operands, Signature other, OperandCaster[] casters)
        {
            ReadOnlySpan<Type> targetTypes = other.OperandTypes;

            if (operands.Length != targetTypes.Length)
                return false;

            int length = operands.Length;

            for (int i = 0; i < length; i++)
            {
                Type targetType = targetTypes[i];
                Type operandType = operands[i].Value.EvaluationType;

                if (!CanBeCasted(operandType, targetType, out OperandCaster caster))
                    return false;

                casters[i] = caster;
            }

            return true;
        }

        private bool CanBeCasted(Type from, Type to, out OperandCaster caster)
        {
            if (from == to)
            {
                caster = null;
                return true;
            }

            if (_castingTable.IsCasterDefined(from, to, out caster))
                return true;

            caster = null;
            return to.IsAssignableFrom(from);
        }
    }
}
