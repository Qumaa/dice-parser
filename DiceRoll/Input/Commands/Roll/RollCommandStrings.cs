namespace DiceRoll
{
    public sealed class RollCommandStrings
    {
        public readonly string Description = "Evaluate the passed expression to one result";
        public readonly string FailedToPass = "Failed to pass";
        public readonly string TreeOptionDescription = "Display every step of the evaluation process as a tree";
        public readonly string TimesOptionDescription = "Roll the passed expression specified amount of times";

        private readonly string _timesOutOfRangeFormat = "The roll may not be repeated {0} times. The valid range is from 1 to {1}.";

        public string TimesOutOfRange(int times, int maxTimes) =>
            string.Format(_timesOutOfRangeFormat, times.ToString(), maxTimes.ToString());
    }
}
