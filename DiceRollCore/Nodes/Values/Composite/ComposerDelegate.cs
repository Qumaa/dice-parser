using System.Collections.Generic;

namespace DiceRoll
{
    public delegate INumeric ComposerDelegate(IEnumerable<INumeric> source);
}
