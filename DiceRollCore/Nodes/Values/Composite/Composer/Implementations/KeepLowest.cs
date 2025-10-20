using System.Collections.Generic;

namespace DiceRoll
{
    public sealed class KeepLowest : Composer
    {
        public override INumeric Compose(IEnumerable<INumeric> source) =>
            Aggregate(source, static (left, right) => Node.Operator.SelectLowest(left, right));
    }
}
