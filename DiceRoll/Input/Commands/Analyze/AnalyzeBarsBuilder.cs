using System.Collections.Generic;

namespace DiceRoll
{
    public abstract class AnalyzeBarsBuilder
    {
        public abstract string[] CreatePaddedBarStrings(IEnumerable<Probability> probabilities, int barStringLength);
    }
}
