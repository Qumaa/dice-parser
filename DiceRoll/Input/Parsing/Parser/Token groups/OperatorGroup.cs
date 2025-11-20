using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace DiceRoll.Input.Parsing
{
    public class OperatorGroup : TokenGroup
    {
        public const int DEFAULT_PRECEDENCE = 0;
        
        private readonly EquationParserState _state;
        private readonly OperatorDefinition[] _definitions;
        
        public OperatorGroup(int precedence, IEnumerable<OperatorDefinition> definitions, EquationParserState state) : base(precedence)
        {
            ArgumentNullException.ThrowIfNull(state);
            ArgumentNullException.ThrowIfNull(definitions);
            
            _state = state;
            _definitions = definitions.OrderByDescending(x => x.Precedence).ToArray();
        }
        
        public override bool TryExecute(in Substring substring, out Substring match)
        {
            if (!StartsWithOperator(in substring, out OperatorDefinition[] definitions, out match))
                return false;

            Operator @operator = new(definitions);
            _state.Members.Push(@operator, in match);
            return true;
        }

        private bool StartsWithOperator(in Substring expression, out OperatorDefinition[] definitions, 
            out Substring substring)
        {
            substring = expression.Empty();
            DefinitionsCollection collection = new();
            
            foreach (OperatorDefinition definition in _definitions)
            {
                if (!definition.Token.MatchesStart(in expression, out Substring newMatch))
                {
                    if (collection.IsEmpty)
                        TokenGroupUtils.UpdateEarliestMatch(ref substring, in newMatch);
                    
                    continue;
                }
                
                collection.Add(definition);
                substring = newMatch;
            }

            definitions = collection.ToArray();
            return definitions is { Length: > 0 };
        }

        [StructLayout(LayoutKind.Auto)]
        private struct DefinitionsCollection
        {
            private OperatorDefinition _single;
            private List<OperatorDefinition> _many;

            public bool IsEmpty => _single is null;

            public void Add(OperatorDefinition definition)
            {
                if (_many is not null)
                {
                    _many.Add(definition);
                    return;
                }
                
                if (_single is null)
                {
                    _single = definition;
                    return;
                }

                _many = new List<OperatorDefinition> { _single, definition };
            }

            public OperatorDefinition[] ToArray()
            {
                if (_many is not null)
                    return _many.ToArray();

                if (_single is not null)
                    return new[] { _single };

                return Array.Empty<OperatorDefinition>();
            }
        }
    }
}
