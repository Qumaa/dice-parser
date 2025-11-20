using System;

namespace DiceRoll.Input.Parsing.Deprecated
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
            InvocationInfo info = GetInvocationInfo(in @operator);
            
            OperatorInvoker invoker = info.Invoker;

            Mapped<LinkedNode>[] excessive =
                _operandsProvider.LiftExcessiveOperandsIfAny(@operator.Value.Position, invoker.RightArity);
            
            Mapped<LinkedNode>[] operands = _operandsProvider.PopOperands(invoker.Arity);
            
            Operand result = Invoke(operands, in info);
            
            PushInvocationResult(in result, in @operator.Range, operands);
            
            _operandsProvider.RestoreExcessiveOperands(excessive);
        }

        private InvocationInfo GetInvocationInfo(in Mapped<Operator> @operator)
        {
            if (_infoProvider.TryGetInvocationInfo(in @operator, out InvocationInfo info))
                return info;
            
            throw OperatorInvocationException.NoMatchingSignature(
                @operator.Value.InvocationBehaviour,
                _state.Mapper.GetSubstringOf(in @operator)
                );
        }

        private static Operand Invoke(Mapped<LinkedNode>[] operands, in InvocationInfo info)
        {
            Signature signature = info.Invoker.Signature;
            
            OperandsAccess access = new(operands, signature);
            
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
