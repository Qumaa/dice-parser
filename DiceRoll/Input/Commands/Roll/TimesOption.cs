using System.CommandLine;
using System.CommandLine.Parsing;

namespace DiceRoll
{
    internal sealed class TimesOption : Option<int>
    {
        private const int _DEFAULT_MAX_TIMES = 25;

        public TimesOption(RollCommandStrings strings, int maxTimes = _DEFAULT_MAX_TIMES) : base(
            "--times",
            () => 1,
            strings.TimesOptionDescription
            )
        {
            AddAlias("-x");
            Arity = ArgumentArity.ExactlyOne;
            AllowMultipleArgumentsPerToken = false;
            AddValidator(result => Validate(result, strings, maxTimes));
        }

        private static void Validate(OptionResult result, RollCommandStrings strings, int maxTimes)
        {
            if (result.IsImplicit)
                return;

            int times = result.GetValueOrDefault<int>();

            if (times < 1 || times > maxTimes)
                result.ErrorMessage = strings.TimesOutOfRange(times, maxTimes);
        }
    }
}
