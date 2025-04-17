using System.Collections.Generic;
using System.CommandLine;

namespace DiceRoll
{
    internal sealed class DiceExpressionArgument : Argument<IEnumerable<string>>
    {
        public DiceExpressionArgument(DiceCommandStrings strings) : base(
            strings.DiceExpressionName,
            strings.DiceExpressionDescription
            ) { }
    }
}
