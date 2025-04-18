namespace DiceRoll
{
    public sealed class DiceCommandStrings
    {
        public const string WIP = "This functionality is under development.";
        
        public readonly AnalyzeCommandStrings Analyze = new();
        public readonly RollCommandStrings Roll = new();
        
        public readonly string Description = "Enter the perpetual mode or evaluate the passed expression";
        public readonly string DiceExpressionName = "Dice expression";
        public readonly string DiceExpressionDescription = "The expression to be evaluated";
    }
}
