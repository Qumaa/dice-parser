using System.CommandLine;

namespace DiceRoll
{
    internal sealed class NormalizePlotBarsOption : Option<bool>
    {
        public NormalizePlotBarsOption(AnalyzeCommandStrings strings, bool defaultValue) : base(
            "--normalize",
            () => defaultValue,
            strings.NormalizeOptionDescription
            )
        {
            AddAlias("-n");
            Arity = ArgumentArity.ZeroOrOne;
            AllowMultipleArgumentsPerToken = false;
        }
    }
}
