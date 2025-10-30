using System.Collections.Generic;

namespace DiceRoll
{
    public interface IComposite : INumeric
    {
        IEnumerable<INumeric> SourceNodes { get; }
    }
}
