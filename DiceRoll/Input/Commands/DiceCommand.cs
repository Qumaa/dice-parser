using System.CommandLine;

namespace DiceRoll
{
    internal sealed class DiceCommand : RootCommand
    {
        public DiceCommand(DiceCommandStrings strings) : base(strings.Description)
        {
            DiceExpressionArgument argument = new(strings);
                
            AddCommand(new RollCommand(strings.Roll, argument));
            AddCommand(new AnalyzeCommand(strings.Analyze, argument));
            
            this.SetHandler(ctx => ctx.Console.WriteLine(DiceCommandStrings.WIP)); // todo
        }
    }
}
