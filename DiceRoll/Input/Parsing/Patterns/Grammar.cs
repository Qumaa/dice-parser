using System;

namespace DiceRoll.Input.Parsing
{
    public static class Grammar
    {
        public static StringComparison DefaultLiteralComparison = StringComparison.Ordinal;
        
        public static IGrammar UnsignedInteger()
        {
            throw new NotImplementedException();
        }

        public static IGrammar AsOptional(IGrammar grammar, string defaultValue = "")
        {
            throw new NotImplementedException();
        }

        public static IGrammar Literal(string value) =>
            Literal(value, DefaultLiteralComparison);

        public static IGrammar Literal(string value, StringComparison comparison)
        {
            throw new NotImplementedException();
        }

        public static IGrammar Literal(string[] tags) =>
            Literal(tags, DefaultLiteralComparison);

        public static IGrammar Literal(string[] tags, StringComparison comparison)
        {
            throw new NotImplementedException();
        }

        public static IGrammar Reference(string tag)
        {
            throw new NotImplementedException();
        }
    }
}
