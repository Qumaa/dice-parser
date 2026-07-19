using System;
using System.Collections.Generic;
using System.Linq;

namespace DiceRoll.Input.Parsing
{
    public sealed class LexingPipeline
    {
        private readonly Lexer[] _lexers;

        public LexingPipeline(IEnumerable<Lexer> lexers)
        {
            ArgumentNullException.ThrowIfNull(lexers);
            
            _lexers = lexers.ToArray();
        }

        public bool TryExecuteAll(in Substring substring, Cursor stateCursor, out Substring match)
        {
            Substring earliestMatch = substring.Empty();
            
            foreach (Lexer lexer in _lexers)
                if (lexer.TryExecute(in substring, stateCursor, out Substring newMatch))
                {
                    match = newMatch;
                    return true;
                }
                else
                    LexerUtils.UpdateEarliestMatch(ref earliestMatch, in newMatch);

            match = earliestMatch.IsEmpty ? substring : substring.SetEnd(earliestMatch.Start).TrimEnd();
            return false;
        }
    }
}
