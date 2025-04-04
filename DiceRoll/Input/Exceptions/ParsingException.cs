using System;

namespace DiceRoll.Input
{
    public sealed class ParsingException : Exception
    {
        public ParsingException(Exception innerException) : 
            base($"Parsing failed. {innerException.GetType().Name}: {innerException.Message}", innerException) { }
    }
}
