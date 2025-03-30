using System;
using System.Linq;
using System.Runtime.CompilerServices;
using DiceRoll.Nodes;

namespace DiceRoll.Exceptions
{
    public sealed class ZeroDivisorException : DivideByZeroException
    {
        public ZeroDivisorException() { }
        public ZeroDivisorException(string message) : base(message) { }
        public ZeroDivisorException(string message, Exception innerException) : base(message, innerException) { }

        public static void ThrowIfAnyZero(RollProbabilityDistribution distribution,
            [CallerArgumentExpression("distribution")]
            string paramName = null)
        {
            if (distribution.Any(x => x.Outcome.Value is 0))
                throw new ZeroDivisorException("Possibility of dividing by zero.");
        }
    }
}
