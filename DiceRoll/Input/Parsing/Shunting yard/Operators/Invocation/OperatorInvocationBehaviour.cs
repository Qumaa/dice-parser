using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace DiceRoll.Input.Parsing
{
    public sealed class OperatorInvocationBehaviour
    {
        private const int _DEFAULT_OVERLOADS_HINT = 2;
        
        public readonly OperatorInvoker[] Invokers;

        private OperatorInvocationBehaviour(OperatorInvoker[] invokers)
        {
            Invokers = invokers;
        }
        
        public static Builder WithOverloads(int overloadsHint = _DEFAULT_OVERLOADS_HINT) =>
            new(overloadsHint);
        
        public static Builder WithOverloads(OperatorInvoker baseInvoker, int overloadsHint = _DEFAULT_OVERLOADS_HINT) =>
            WithOverloads().Overload(baseInvoker);

        public static OperatorInvocationBehaviour WithoutOverloads(OperatorInvoker invoker) =>
            WithOverloads(invoker).Build();

        [StructLayout(LayoutKind.Auto)]
        public readonly struct Builder
        {
            private readonly List<OperatorInvoker> _invokers;
            
            public Builder(int overloadsHint)
            {
                _invokers = new List<OperatorInvoker>(overloadsHint);
            }

            public Builder Overload(OperatorInvoker overload)
            {
                _invokers.Add(overload);

                return this;
            }

            public OperatorInvocationBehaviour Build()
            {
                if (_invokers.Count is 0)
                    throw new ArgumentException("Constructing an invocation behaviour with not at least one invoker is not valid.");
                
                return new OperatorInvocationBehaviour(_invokers.ToArray());
            }
        }
    }
}
