using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DiceRoll.Exceptions;

namespace DiceRoll.Input.Parsing
{
    public sealed class OperatorInvocationBehaviour
    {
        private const int _DEFAULT_OVERLOADS_HINT = 4;
        
        public readonly OperatorInvoker[] Invokers;
        public readonly int Precedence;
        public readonly Associativity Associativity;

        private OperatorInvocationBehaviour(OperatorInvoker[] invokers, int precedence, Associativity associativity)
        {
            ArgumentNullException.ThrowIfNull(invokers);
            ArgumentOutOfRangeException.ThrowIfZero(invokers.Length);
            EnumValueNotDefinedException.ThrowIfValueNotDefined(Associativity);
            
            Invokers = invokers;
            Precedence = precedence;
            Associativity = associativity;
        }
        
        public static Builder WithOverloads(int precedence, Associativity associativity, int overloadsHint = _DEFAULT_OVERLOADS_HINT) =>
            new(overloadsHint, precedence, associativity);
        
        public static Builder WithOverloads(int precedence, Associativity associativity, OperatorInvoker baseInvoker, int overloadsHint = _DEFAULT_OVERLOADS_HINT) =>
            WithOverloads(precedence, associativity, overloadsHint).Overload(baseInvoker);

        public static OperatorInvocationBehaviour WithoutOverloads(int precedence, Associativity associativity, OperatorInvoker invoker) =>
            WithOverloads(precedence, associativity, invoker, 1).Build();

        [StructLayout(LayoutKind.Auto)]
        public readonly struct Builder
        {
            private readonly List<OperatorInvoker> _invokers;
            private readonly int _precedence;
            private readonly Associativity _associativity;
            
            public Builder(int overloadsHint, int precedence, Associativity associativity)
            {
                EnumValueNotDefinedException.ThrowIfValueNotDefined(associativity);
                
                _precedence = precedence;
                _associativity = associativity;
                
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
                
                return new OperatorInvocationBehaviour(_invokers.ToArray(), _precedence, _associativity);
            }
        }
    }
}
