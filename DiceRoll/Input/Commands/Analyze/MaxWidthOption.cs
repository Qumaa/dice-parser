using System.CommandLine;

namespace DiceRoll
{
    internal sealed class MaxWidthOption : Option<int>
    {
        public MaxWidthOption(AnalyzeCommandStrings strings, int defaultMaxWidth) : base(
            "--max-width",
            () => defaultMaxWidth,
            strings.MaxWidthOptionDescription
            )
        {
            AddAlias("-w");
            Arity = ArgumentArity.ZeroOrOne;
            AllowMultipleArgumentsPerToken = false;
        }
    }
}
