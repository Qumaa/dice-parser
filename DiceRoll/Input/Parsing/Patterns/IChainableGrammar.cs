using System.Collections.Generic;

namespace DiceRoll.Input.Parsing
{
    /// <summary>
    /// An <see cref="IExtendableGrammar"/> that depends on all its probes (a and b and c ...)
    /// </summary>
    public interface IChainableGrammar : IExtendableGrammar, IEnumerable<IGrammar> { }
}
