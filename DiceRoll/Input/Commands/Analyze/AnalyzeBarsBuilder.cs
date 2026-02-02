using System;
using System.Collections.Generic;
using System.Linq;

namespace DiceRoll
{
    public abstract class AnalyzeBarsBuilder
    {
        public string[] CreatePaddedBarStrings(IEnumerable<Probability> probabilities, int barWidth)
        {
            Probability[] array = probabilities.ToArray();

            if (array.Length is 0)
                return Array.Empty<string>();
            
            string[] bars = new string[array.Length];

            Probability max = GetHighestProbability(array);

            for (int i = 0; i < bars.Length; i++)
                bars[i] = CreatePaddedBarString(array[i], max, barWidth);

            return bars;
        }

        public abstract string CreatePaddedBarString(Probability probability, Probability maxProbability, int barWidth);
        
        private static Probability GetHighestProbability(Probability[] array)
        {
            Probability max = array[0];

            for (int i = 1; i < array.Length; i++)
                if (array[i] > max)
                    max = array[i];
            
            return max;
        }
    }
}
