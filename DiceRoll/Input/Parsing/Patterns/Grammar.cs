using System;

namespace DiceRoll.Input.Parsing
{
    public static class Grammar
    {
        public static StringComparison DefaultLiteralComparison = StringComparison.Ordinal;
        
        public static IGrammar UnsignedInteger() =>
            new TokenDrivenGrammar(new NumericToken());

        public static IGrammar AsOptional(IGrammar grammar, string defaultValue = "") =>
            new Optional(grammar);

        public static IGrammar Literal(string value) =>
            Literal(value, DefaultLiteralComparison);

        public static IGrammar Literal(string value, StringComparison comparison) =>
            new TokenDrivenGrammar(new StringComparisonToken(value, comparison));

        public static IGrammar Literal(string[] tags) =>
            Literal(tags, DefaultLiteralComparison);

        public static IGrammar Literal(string[] tags, StringComparison comparison) =>
            new TokenDrivenGrammar(new StringComparisonToken(tags, comparison));

        public static IGrammar Reference(string tag) =>
            new Reference(tag);
    }
}
