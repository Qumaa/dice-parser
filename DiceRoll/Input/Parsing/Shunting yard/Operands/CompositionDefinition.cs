namespace DiceRoll.Input.Parsing
{
    public sealed class CompositionDefinition
    {
        public static readonly CompositionDefinition Summation = new(
            Tokenize("summation", "sum", "s"),
            static (numeric, times) => Node.Value.Summation(numeric, times)
            );
        
        public static readonly CompositionDefinition Highest = new(
            Tokenize("highest", "h"),
            static (numeric, times) => Node.Value.Highest(numeric, times)
            );
        
        public static readonly CompositionDefinition Lowest = new(
            Tokenize("lowest", "l"),
            static (numeric, times) => Node.Value.Lowest(numeric, times)
            );
        
        public readonly IToken Token;
        public readonly CompositionHandler CompositionHandler;

        public CompositionDefinition(IToken token, CompositionHandler compositionHandler)
        {
            Token = token;
            CompositionHandler = compositionHandler;
        }
        
        private static StringBasedToken Tokenize(params string[] args) =>
            StringBasedToken.CaseInsensitive(args);
    }
}
