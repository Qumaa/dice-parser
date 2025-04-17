using System.CommandLine;

namespace DiceRoll
{
    internal sealed class DiceCommand : RootCommand
    {
        public DiceCommand(DiceCommandStrings strings) : base(strings.RootDescription)
        {
            DiceExpressionArgument argument = new(strings);
                
            AddCommand(new RollCommand(strings, argument));
            AddCommand(new AnalyzeCommand(strings, argument));
        }
    }
}
