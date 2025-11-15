using System;
using System.Runtime.InteropServices;

namespace DiceRoll.Input.Parsing
{
    public sealed class OperatorInvocationHandler
    {
        private readonly ShuntingYardState _state;
        private readonly OperandCastingTable _castingTable;
        
        public OperatorInvocationHandler(ShuntingYardState state, OperandCastingTable castingTable)
        {
            _state = state;
            _castingTable = castingTable;
        }

        public void InvokeOperator(in Mapped<Operator> @operator)
        {
            OperatorInvocationBehaviour behaviour = @operator.Value.InvocationBehaviour;
            
            Mapped<LinkedNode>[] operands = PopOperands(behaviour.Arity);

            InvocationInfo info = GetInvocationInfo(operands, in @operator);

            Operand result = ExecuteInvocation(operands, in info);
            
            PushInvocationResult(in result, in @operator.Range, operands);
        }

        private Mapped<LinkedNode>[] PopOperands(int arity)
        {
            ThrowIfBadArity(arity);

            Mapped<LinkedNode>[] operands = new Mapped<LinkedNode>[arity];
                
            for (int i = arity - 1; i >= 0; i--)
                operands[i] = _state.Operands.Pop();
            
            return operands;
        }

        private void ThrowIfBadArity(int arity)
        {
            if (_state.Operands.Count < arity)
                throw OperatorInvocationException.BadArity(arity, _state.Operands.Count);
        }

        private InvocationInfo GetInvocationInfo(Mapped<LinkedNode>[] operands, in Mapped<Operator> @operator)
        {
            OperatorInvocationBehaviour behaviour = @operator.Value.InvocationBehaviour;
            OperandCaster[] casters = new OperandCaster[behaviour.Arity];
            
            foreach (OperatorInvoker invoker in behaviour.Invokers)
            {
                if (!InvokableWith(operands, invoker.Signature, casters))
                    continue;

                return new InvocationInfo(invoker, casters);
            }

            throw OperatorInvocationException.NoMatchingSignature(
                behaviour,
                operands,
                _state.Mapper.GetSubstringOf(in @operator)
                );
        }

        private bool InvokableWith(Mapped<LinkedNode>[] operands, Signature other, OperandCaster[] casters)
        {
            ReadOnlySpan<Type> targetTypes = other.GetOperandTypes();

            if (operands.Length != targetTypes.Length)
                return false;

            int length = operands.Length;

            for (int i = 0; i < length; i++)
            {
                Type targetType = targetTypes[i];
                Type type = operands[i].Value.EvaluationType;
                    
                if (targetType == type)
                    continue;
                    
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

        private static Operand ExecuteInvocation(Mapped<LinkedNode>[] operands, in InvocationInfo info)
        {
            Signature signature = info.Invoker.Signature;
            
            OperandsAccess access = new(operands, info.Casters, signature);
            
            INode invocationResult = info.Invoker.Invoke(access);

            return new Operand(invocationResult, signature.GetReturnType());
        }

        private void PushInvocationResult(in Operand operand, in Range operatorRange, Mapped<LinkedNode>[] operands)
        {
            ArgumentNullException.ThrowIfNull(operand.Node);

            LinkedNode linkedNode = new(operand.Node, operand.EvaluationType, operands);
            
            _state.Operands.Push(linkedNode, in operatorRange);
        }

        [StructLayout(LayoutKind.Auto)]
        private readonly struct InvocationInfo
        {
            public readonly OperatorInvoker Invoker;
            public readonly OperandCaster[] Casters;
            
            public InvocationInfo(OperatorInvoker invoker, OperandCaster[] casters)
            {
                Invoker = invoker;
                Casters = casters;
            }
        }
    }
}
