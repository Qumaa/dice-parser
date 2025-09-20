using System;
using System.Runtime.InteropServices;

namespace DiceRoll.Input.Parsing
{
    [StructLayout(LayoutKind.Auto)]
    internal readonly struct OperatorInvocation
    {
        private readonly ShuntingYardState _state;
        private readonly OperandCastingTable _castingTable;
        private readonly OperatorInvocationBehaviour _invocationBehaviour;
        private readonly Range _operatorMappedRange;

        public OperatorInvocation(ShuntingYardState state, OperandCastingTable castingTable, OperatorInvocationBehaviour invocationBehaviour,
            in Range operatorMappedRange)
        {
            _state = state;
            _castingTable = castingTable;
            _invocationBehaviour = invocationBehaviour;
            _operatorMappedRange = operatorMappedRange;
        }

        public void Perform()
        {
            int arity = _invocationBehaviour.Arity;
            
            ThrowIfBadArity(arity);

            InvocationCapture capture = new(_state.Operands, arity, in _operatorMappedRange);

            InvocationAbilityTest test = new(_castingTable, in capture);
            
            if (!test.Perform(_invocationBehaviour, out OperatorInvoker invokable, out OperandCaster[] casters))
                throw new Exception(); // todo

            OperandsAccess access = new(capture.Operands, casters, invokable.Signature);
            
            INode invocationResult = invokable.Invoke(access);

            Operand operand = new(invocationResult, invokable.Signature.GetReturnType());
            
            PushInvocationResult(in operand, capture);
        }

        private void ThrowIfBadArity(int arity)
        {
            if (_state.Operands.Count < arity)
                throw new OperatorInvocationException(ParsingErrorMessages.OperandsExpected(arity, _state.Operands.Count));
        }

        private void PushInvocationResult(in Operand operand, in InvocationCapture capture)
        {
            ArgumentNullException.ThrowIfNull(operand.Node);

            LinkedNode linkedOperator = LinkedNode.Operator(capture.Operands, operand.EvaluationType);

            Mapped<LinkedNode> mappedOperator = new(linkedOperator, in _operatorMappedRange);
            LinkedNode linkedOperand = LinkedNode.Operand(in operand, mappedOperator);
            
            _state.Operands.Push(linkedOperand, in capture.MappedRange);
        }

        [StructLayout(LayoutKind.Auto)]
        private readonly struct InvocationCapture
        {
            public readonly Mapped<LinkedNode>[] Operands;
            public readonly Range MappedRange;
            
            public InvocationCapture(MappedStack<LinkedNode> operands, int arity, in Range operatorMappedRange)
            {
                Operands = new Mapped<LinkedNode>[arity];

                MappedRange = operatorMappedRange;

                for (int i = arity - 1; i >= 0; i--)
                {
                    Mapped<LinkedNode> operand = operands.Pop();
                    Operands[i] = operand;
                    MappedRange = MappedRange.And(operand.Range);
                }
            }
        }

        [StructLayout(LayoutKind.Auto)]
        private readonly struct InvocationAbilityTest
        {
            private readonly OperandCastingTable _castingTable;
            private readonly InvocationCapture _capture;
            
            public InvocationAbilityTest(OperandCastingTable castingTable, in InvocationCapture capture)
            {
                _castingTable = castingTable;
                _capture = capture;
            }

            public bool Perform(OperatorInvocationBehaviour behaviour, out OperatorInvoker invokable,
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

                if (_capture.Operands.Length != targetTypes.Length)
                    return false;

                int length = _capture.Operands.Length;

                for (int i = 0; i < length; i++)
                {
                    Type targetType = targetTypes[i];
                    Type type = _capture.Operands[i].Value.EvaluationType;
                    
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
