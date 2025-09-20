using System;
using System.Linq;

namespace DiceRoll.Input.Parsing
{
    public sealed class BadSignatureException : Exception
    {
        public readonly Type[] ExpectedTypes;
        
        public BadSignatureException(Type[] expectedTypes)
        {
            ExpectedTypes = expectedTypes;
        }

        public static BadSignatureException Combine(BadSignatureException[] exceptions) =>
            new(exceptions.SelectMany(x => x.ExpectedTypes).ToArray());
    }
}
