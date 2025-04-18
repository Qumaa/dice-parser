using System.CommandLine;

namespace DiceRoll
{
    internal sealed class StyleOption : Option<AnalyzeOutputStyle>
    {
        public StyleOption(AnalyzeCommandStrings strings) : base(
            "--style",
            () => AnalyzeOutputStyle.Full,
            strings.StyleOptionDescription
            )
        {
            AddAlias("-s");
            Arity = ArgumentArity.ExactlyOne;
            AllowMultipleArgumentsPerToken = false;
        }
    }
}
