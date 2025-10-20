using System.Collections.Generic;

namespace DiceRoll
{
    public sealed class Summarize : Composer
    {
        public override INumeric Compose(IEnumerable<INumeric> source) =>
            Aggregate(source, static (left, right) => Node.Operator.Add(left, right));
    }
}
