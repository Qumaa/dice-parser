using System;
using System.Collections.Generic;
using System.Linq;

namespace DiceRoll
{
    public abstract class AnalyzeBarsBuilder
    {
        public string[] CreatePaddedBarStrings(IEnumerable<Probability> probabilities, Probability max, int barWidth)
        {
            Probability[] array = probabilities as Probability[] ?? probabilities.ToArray();

            if (array.Length is 0)
                return Array.Empty<string>();
            
            string[] bars = new string[array.Length];
            
            for (int i = 0; i < bars.Length; i++)
                bars[i] = CreatePaddedBarString(array[i], max, barWidth);

            return bars;
        }

        public abstract string CreatePaddedBarString(Probability probability, Probability maxProbability, int barWidth);
    }
}
