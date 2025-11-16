using System;

namespace DiceRoll.Input.Parsing
{
    public sealed class OperatorInvocationHandler
    {
        private readonly ShuntingYardState _state;
        private readonly InvocationOperandsProvider _operandsProvider;
        private readonly InvocationInfoProvider _infoProvider;
        
        public OperatorInvocationHandler(ShuntingYardState state, OperandCastingTable castingTable)
        {
            ArgumentNullException.ThrowIfNull(state);
            ArgumentNullException.ThrowIfNull(castingTable);

            _state = state;
            _operandsProvider = new InvocationOperandsProvider(state);
            _infoProvider = new InvocationInfoProvider(castingTable);
        }

        public void InvokeOperator(in Mapped<Operator> @operator)
        {
            Mapped<LinkedNode>[] excessive = _operandsProvider.PopExcessiveOperandsIfAny(in @operator.Value);
            
            Mapped<LinkedNode>[] operands = _operandsProvider.PopOperandsFor(in @operator.Value);

            InvocationInfo info = GetInvocationInfo(operands, in @operator);

            Operand result = Invoke(operands, in info);
            
            PushInvocationResult(in result, in @operator.Range, operands);
            
            _operandsProvider.RestoreExcessiveOperands(excessive);
        }

        private InvocationInfo GetInvocationInfo(Mapped<LinkedNode>[] operands, in Mapped<Operator> @operator)
        {
            if (_infoProvider.TryGetInvocationInfo(operands, in @operator, out InvocationInfo info))
                return info;
            
            throw OperatorInvocationException.NoMatchingSignature(
                @operator.Value.InvocationBehaviour,
                operands,
                _state.Mapper.GetSubstringOf(in @operator)
                );
        }

        private static Operand Invoke(Mapped<LinkedNode>[] operands, in InvocationInfo info)
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
    }
}
