namespace DiceRoll
{
    public sealed class DiscreteBarsBuilder : AnalyzeBarsBuilder
    {
        public override string CreatePaddedBarString(Probability probability, Probability maxProbability, int barWidth)
        {
            int allUnits = barWidth * Boxes.UNITS_PER_BOX;
            int normalizedUnits = (int) (allUnits * (probability / maxProbability).Value);

            int fullBlocks = normalizedUnits / 8;
            int remainingUnits = normalizedUnits % 8;

            char[] chars = new char[barWidth];

            for (int i = 0; i < fullBlocks; i++)
                chars[i] = Boxes.B8;

            for (int i = fullBlocks; i < chars.Length; i++)
                chars[i] = ' ';

            if (remainingUnits is not 0)
                chars[fullBlocks] = GetBarTip(remainingUnits);

            return new string(chars, 0, chars.Length);
        }

        private static char GetBarTip(int units) =>
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
