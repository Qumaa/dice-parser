using System;

namespace DiceRoll.Input
{
    public sealed class ParsingExceptionInfo
    {
        public readonly string Source;
        public readonly int CauseStart;
        public readonly int CauseEnd;
        public readonly Exception Exception;
        
        public ParsingExceptionInfo(string source, int causeStart, int causeEnd, Exception exception)
        {
            Source = source;
            CauseStart = causeStart;
            CauseEnd = causeEnd;
            Exception = exception;
        }
    }
}
