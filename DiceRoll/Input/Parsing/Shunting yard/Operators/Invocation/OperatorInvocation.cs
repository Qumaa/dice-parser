using System;
using System.Runtime.InteropServices;

namespace DiceRoll.Input.Parsing
{
    [StructLayout(LayoutKind.Auto)]
    internal readonly struct OperatorInvocation
    {
        private readonly ShuntingYardState _state;
        private readonly OperandCastingTable _castingTable;
        private readonly OperatorInvocationBehaviour _behaviour;
        private readonly Range _operatorMappedRange;

        public OperatorInvocation(ShuntingYardState state, OperandCastingTable castingTable, OperatorInvocationBehaviour behaviour,
            in Range operatorMappedRange)
        {
            _state = state;
            _castingTable = castingTable;
            _behaviour = behaviour;
            _operatorMappedRange = operatorMappedRange;
        }

        public void Perform()
        {
            int arity = _behaviour.Arity;
            ThrowIfBadArity(arity);

            InvocationOperands operands = new(_state.Operands, arity);

            OperandsCast test = new(_castingTable, in operands);
            if (!test.CanBeCastedToBeInvokedBy(_behaviour, out OperatorInvoker invokable, out OperandCaster[] casters))
                ThrowNoMatchingSignature(in operands);

            OperandsAccess access = new(operands, casters, invokable.Signature);
            INode invocationResult = invokable.Invoke(access);

            Operand operand = new(invocationResult, invokable.Signature.GetReturnType());
            PushInvocationResult(in operand, operands);
        }

        private void ThrowNoMatchingSignature(in InvocationOperands operands) =>
            throw OperatorInvocationException.NoMatchingSignature(
                _behaviour,
                operands,
                _state.Mapper.GetSubstringOf(in _operatorMappedRange)
                );

        private void ThrowIfBadArity(int arity)
        {
            if (_state.Operands.Count < arity)
                throw OperatorInvocationException.BadArity(arity, _state.Operands.Count);
        }

        private void PushInvocationResult(in Operand operand, in InvocationOperands operands)
        {
            ArgumentNullException.ThrowIfNull(operand.Node);

            LinkedNode linkedNode = new(operand.Node, operand.EvaluationType, operands);
            
            _state.Operands.Push(linkedNode, in _operatorMappedRange);
        }

        [StructLayout(LayoutKind.Auto)]
        private readonly struct InvocationOperands
        {
            private readonly Mapped<LinkedNode>[] _operands;

            public Mapped<LinkedNode> this[int i] => _operands[i];

            public int Length => _operands.Length;
            
            public InvocationOperands(MappedStack<LinkedNode> operands, int arity)
            {
                _operands = new Mapped<LinkedNode>[arity];
                
                for (int i = arity - 1; i >= 0; i--)
                    _operands[i] = operands.Pop();
            }

            public static implicit operator Mapped<LinkedNode>[](in InvocationOperands operands) =>
                operands._operands;
        }

        [StructLayout(LayoutKind.Auto)]
        private readonly struct OperandsCast
        {
            private readonly OperandCastingTable _castingTable;
            private readonly InvocationOperands _operands;
            
            public OperandsCast(OperandCastingTable castingTable, in InvocationOperands operands)
            {
                _castingTable = castingTable;
                _operands = operands;
            }

            public bool CanBeCastedToBeInvokedBy(OperatorInvocationBehaviour behaviour, out OperatorInvoker invokable,
                out OperandCaster[] casters)
            {
                casters = new OperandCaster[behaviour.Arity];
            
                foreach (OperatorInvoker invoker in behaviour.Invokers)
                {
                    if (!InvokableWith(invoker.Signature, casters))
                        continue;

                    invokable = invoker;
                    return true;
                }

                casters = Array.Empty<OperandCaster>();
                invokable = null;
                return false;
            }

            private bool InvokableWith(Signature other, OperandCaster[] casters)
            {
                ReadOnlySpan<Type> targetTypes = other.GetOperandTypes();

                if (_operands.Length != targetTypes.Length)
                    return false;

                int length = _operands.Length;

                for (int i = 0; i < length; i++)
                {
                    Type targetType = targetTypes[i];
                    Type type = _operands[i].Value.EvaluationType;
                    
                    if (!(_castingTable.IsCasterDefined(
                            type,
                            targetType,
                            out OperandCaster caster
                            ) || targetType.IsAssignableFrom(type)))
                        return false;

                    casters[i] = caster;
                }

                return true;
            }
        }
    }
}
