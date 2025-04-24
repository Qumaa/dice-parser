using System.CommandLine;
using System.CommandLine.Invocation;

namespace DiceRoll
{
    internal sealed class AliasCommand : Command
    {
        public AliasCommand(AliasCommandStrings strings) : base("alias", strings.Description)
        {
            AddAlias("a");
            
            this.SetHandler(context => CommandHandler(context));
        }

        private static void CommandHandler(InvocationContext context) =>
            context.Console.WriteLine(DiceCommandStrings.WIP);
    }
}
