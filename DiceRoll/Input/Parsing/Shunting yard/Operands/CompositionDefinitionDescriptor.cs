using System.Collections.Generic;
using System.Linq;

namespace DiceRoll.Input.Parsing
{
    public readonly struct CompositionDefinitionDescriptor
    {
        public readonly IEnumerable<string> Tokens;

        public readonly CompositionHandler CompositionHandler;

        public CompositionDefinitionDescriptor(IEnumerable<string> tokens, CompositionHandler compositionHandler)
        {
            Tokens = tokens;
            CompositionHandler = compositionHandler;
        }

        public CompositionDefinition Convert() =>
            new(ComparisonToken.CaseInsensitive(Tokens as string[] ?? Tokens.ToArray()), CompositionHandler);
    }
}
