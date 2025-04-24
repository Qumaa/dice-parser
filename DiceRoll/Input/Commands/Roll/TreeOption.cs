using System.CommandLine;

namespace DiceRoll
{
    internal sealed class TreeOption : Option<bool>
    {
        public TreeOption(RollCommandStrings strings) : base(
            "--tree",
            () => false,
            strings.TreeOptionDescription
            )
        {
            AddAlias("-t");
            Arity = ArgumentArity.ZeroOrOne;
            AllowMultipleArgumentsPerToken = false;
        }
    }
}
