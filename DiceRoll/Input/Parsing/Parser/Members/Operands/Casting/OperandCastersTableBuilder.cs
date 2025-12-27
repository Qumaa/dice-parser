using System.Collections.Generic;

namespace DiceRoll.Input.Parsing
{
    public sealed class OperandCastersTableBuilder
    {
        private readonly HashSet<OperandCaster> _casters = new();

        public OperandCastersTableBuilder Caster<TSource, TResult>(OperandCaster<TSource, TResult> caster)
            where TSource : INode where TResult : INode
        {
            if (caster is not null)
                _casters.Add(caster);

            return this;
        }

        public OperandCastingTable Build() =>
            new(_casters);
    }
}
