using System;
using System.Collections.Generic;
using System.Linq;

namespace DiceRoll
{
    public sealed class DiscreteBarsBuilder : AnalyzeBarsBuilder
    {
        public override string[] CreatePaddedBarStrings(IEnumerable<Probability> probabilities, int barStringLength)
        {
            Probability[] array = probabilities.ToArray();

            if (array.Length is 0)
                return Array.Empty<string>();
            
            string[] bars = new string[array.Length];

            Probability max = GetHighestProbability(array);

            for (int i = 0; i < bars.Length; i++)
                bars[i] = BuildBar(array[i], max, barStringLength);

            return bars;
        }

        private static Probability GetHighestProbability(Probability[] array)
        {
            Probability max = array[0];

            for (int i = 1; i < array.Length; i++)
                if (array[i] > max)
                    max = array[i];
            
            return max;
        }

        private string BuildBar(Probability current, Probability max, int stringLength)
        {
            int allUnits = stringLength * Boxes.UNITS_PER_BOX;
            int normalizedUnits = (int) (allUnits * (current / max).Value);

            int fullBlocks = normalizedUnits / 8;
            int remainingUnits = normalizedUnits % 8;

            char[] chars = new char[stringLength];

            for (int i = 0; i < fullBlocks; i++)
                chars[i] = Boxes.B8;

            for (int i = fullBlocks; i < chars.Length; i++)
                chars[i] = ' ';

            if (remainingUnits is not 0)
                chars[fullBlocks] = GetBarTip(remainingUnits);

            return new string(chars, 0, chars.Length);
        }

        private char GetBarTip(int units) =>
            units switch
            {
                8 => Boxes.B8,
                7 => Boxes.B7,
                6 => Boxes.B6,
                5 => Boxes.B5,
                4 => Boxes.B4,
                3 => Boxes.B3,
                2 => Boxes.B2,
                1 => Boxes.B1,
                _ => ' '
            };

        private static class Boxes
        {
            public const int UNITS_PER_BOX = 8;
            
            public const char B8 = '█';
            public const char B7 = '▉';
            public const char B6 = '▊';
            public const char B5 = '▋';
            public const char B4 = '▌';
            public const char B3 = '▍';
            public const char B2 = '▎';
            public const char B1 = '▏';
        }
    }
}
