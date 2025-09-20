using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace DiceRoll.Input.Parsing
{
    public sealed class OperatorInvocationBehaviour
    {
        public readonly IEnumerable<OperatorInvoker> Invokers;
        public readonly int LeftArity;
        public readonly int RightArity;
        
        public int Arity => LeftArity + RightArity;

        private OperatorInvocationBehaviour(IEnumerable<OperatorInvoker> invokers, int leftArity, int rightArity)
        {
            Invokers = invokers;
            LeftArity = leftArity;
            RightArity = rightArity;
        }
        
        public static Builder WithOverloads(int leftArity, int rightArity) =>
            new(leftArity, rightArity);
        
        public static Builder WithOverloads(OperatorInvoker baseInvoker, int leftArity, int rightArity) =>
            new Builder(leftArity, rightArity).Overload(baseInvoker);

        public static OperatorInvocationBehaviour WithoutOverloads(OperatorInvoker invoker, int leftArity, int rightArity) =>
            WithOverloads(invoker, leftArity, rightArity).Build();

        [StructLayout(LayoutKind.Auto)]
        public readonly struct Builder
        {
            private readonly List<OperatorInvoker> _invokers;
            private readonly int _leftArity;
            private readonly int _rightArity;

            public Builder(int leftArity, int rightArity)
            {
                if (leftArity < 0 || rightArity < 0)
                    throw new Exception(); // todo: invalid arity
                
                _invokers = new List<OperatorInvoker>(2);
                _leftArity = leftArity;
                _rightArity = rightArity;
            }

            public Builder Overload(OperatorInvoker overload)
            {
                int arity = _leftArity + _rightArity;

                if (overload.Signature.OperandsNumber != arity)
                    throw new Exception(); // todo: signature doesn't match arity
                
                _invokers.Add(overload);

                return this;
            }

            public OperatorInvocationBehaviour Build()
            {
                if (_invokers.Count is 0)
                    throw new Exception(); // todo: no invokers defined at all
                
                return new OperatorInvocationBehaviour(_invokers, _leftArity, _rightArity);
            }
        }
    }
}
