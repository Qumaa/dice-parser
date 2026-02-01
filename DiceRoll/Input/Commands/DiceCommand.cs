using System;
using System.CommandLine;
using System.Text;

namespace DiceRoll
{
    internal sealed class DiceCommand : RootCommand
    {
        public DiceCommand(DiceCommandStrings strings) : base(strings.Description)
        {
            Console.OutputEncoding = Encoding.Unicode;
            
            DiceExpressionArgument argument = new(strings);
                
            AddCommand(new RollCommand(strings.Roll, argument));
            AddCommand(new AnalyzeCommand(strings.Analyze, argument, new DiscreteBarsBuilder()));
            AddCommand(new AliasCommand(strings.Alias));
            
            this.SetHandler(ctx => ctx.Console.WriteLine(DiceCommandStrings.WIP));
        }
    }
}
