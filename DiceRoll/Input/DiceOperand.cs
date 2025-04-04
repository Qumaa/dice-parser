using System;
using System.Text.RegularExpressions;

namespace DiceRoll.Input
{
    public static class DiceOperand
    {
        public static TokenizedOperand Default => new(
            new RegexToken(new Regex(@"(?:(\d+)d(\d+)|d(\d+))\s*(sum|adv|dis)?", RegexOptions.IgnoreCase)),
            static x => ParseDice(x)
            );
        
        // todo: refactor
        private static INumeric ParseDice(string diceExpr)
        {
            // XdY
            int diceCount = 1;
            int delimiterIndex = diceExpr.IndexOf('d');

            if (delimiterIndex is not 0)
                diceCount = int.Parse(diceExpr.AsSpan(0, delimiterIndex));
            
            // dY
            int expressionEnd = diceExpr.Length;
            for (int i = delimiterIndex + 1; i < expressionEnd; i++)
                if (!char.IsDigit(diceExpr, i))
                    expressionEnd = i;

            int facesCount = int.Parse(diceExpr.AsSpan(delimiterIndex + 1, expressionEnd - delimiterIndex - 1));

            INumeric dice = Node.Value.Dice(facesCount);

            if (diceCount is 1)
                return dice;
            
            // sum/adv/dis
            if (expressionEnd != diceExpr.Length)
            {
                ReadOnlySpan<char> compositionExpr = diceExpr.AsSpan(expressionEnd).Trim(' ');
                
                if (compositionExpr.Equals("adv".AsSpan(), StringComparison.OrdinalIgnoreCase))
                    return Node.Value.Highest(dice, diceCount);
                
                if (compositionExpr.Equals("dis".AsSpan(), StringComparison.OrdinalIgnoreCase))
                    return Node.Value.Lowest(dice, diceCount);
            }
            
            return Node.Value.Summation(dice, diceCount);
        }

    }
}
