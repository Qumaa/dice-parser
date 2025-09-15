using System.Collections.Generic;
using System.Linq;

namespace DiceRoll.Input.Parsing
{
    public class DiceOperandBuilder
    {
        private readonly List<IToken> _delimiters = new();
        private readonly List<CompositionDefinition> _compositionDefinitions = new();

        public DiceOperandBuilder Delimiter(IToken token)
        {
            _delimiters.Add(token);
            return this;
        }

        public DiceOperandBuilder Composition(in CompositionDefinition definition)
        {
            _compositionDefinitions.Add(definition);
            return this;
        }

        public DiceOperandBuilder DefaultComposition(in CompositionDefinition definition)
        {
            if (_compositionDefinitions.Count is 0)
                _compositionDefinitions.Add(definition);
            else
                _compositionDefinitions.Insert(0, definition);

            return this;
        }

        public OperandDefinition Build()
        {
            IToken delimiterToken = _delimiters.ToCompositeToken();
            
            DiceParser parser = new(delimiterToken, _compositionDefinitions.ToArray());
            return new OperandDefinition(CreateDiceToken(delimiterToken), diceExpression => parser.Parse(diceExpression));
        }

        private DiceToken CreateDiceToken(IToken delimiterToken) =>
            new(delimiterToken, _compositionDefinitions.Select(x => x.Token).ToCompositeToken());
    }

    public static class DiceOperandBuilderExtensions
    {
        public static DiceOperandBuilder Delimiter(this DiceOperandBuilder builder, IEnumerable<IToken> tokens)
        {
            foreach (IToken token in tokens)
                builder.Delimiter(token);
            
            return builder;
        }

        public static DiceOperandBuilder Composition(this DiceOperandBuilder builder,
            IEnumerable<CompositionDefinition> definitions)
        {
            foreach (CompositionDefinition definition in definitions)
                builder.Composition(in definition);

            return builder;
        }
    }
}
