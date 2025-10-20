using System.Collections.Generic;

namespace DiceRoll
{
    public sealed class KeepHighest : Composer
    {
        public override INumeric Compose(IEnumerable<INumeric> source) =>
            Aggregate(source, static (left, right) => Node.Operator.SelectHighest(left, right));
    }
}
