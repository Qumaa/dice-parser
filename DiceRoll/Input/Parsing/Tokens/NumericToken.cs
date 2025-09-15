namespace DiceRoll.Input.Parsing
{
    public sealed class NumericToken : IToken
    {
        public static readonly NumericToken Shared = new();
        
        public bool Matches(in Substring input, out Substring match)
        {
            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];

                if (!char.IsNumber(c))
                    continue;

                match = ExtractNumberSubstring(input.MoveStart(i));
                return true;
            }

            match = default;
            return false;
        }

        private static Substring ExtractNumberSubstring(in Substring input)
        {
            int count = 0;

            foreach (char c in input)
            {
                if (!char.IsNumber(c))
                    break;

                count++;
            }

            return input.SetLength(count);
        }
    }
}
