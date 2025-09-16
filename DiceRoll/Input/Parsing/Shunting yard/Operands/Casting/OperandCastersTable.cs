using System.Collections.Generic;
using System.Linq;

namespace DiceRoll.Input.Parsing
{
    public sealed class OperandCastersTable
    {
        public static readonly OperandCastersTable Default = BuildDefault();

        private readonly OperandCaster[] _casters;

        public OperandCastersTable(IEnumerable<OperandCaster> casters)
        {
            _casters = casters.ToArray();
        }

        public bool TryCast<TSource, TResult>(TSource source, out TResult result)
            where TSource : INode where TResult : INode
        {
            if (HasExactlyOneCaster<TSource, TResult>(out OperandCaster<TResult> caster))
                return caster.TryCast(source, out result);

            result = default;
            return false;
        }

        private bool HasExactlyOneCaster<TSource, TResult>(out OperandCaster<TResult> caster)
            where TSource : INode where TResult : INode
        {
            caster = null;
            
            foreach (OperandCaster candidate in _casters)
            {
                if (!(candidate.CastsTo(out OperandCaster<TResult> successfulCandidate) &&
                      successfulCandidate.CastsFrom<TSource>()))
                    continue;

                if (caster is not null)
                    return false;

                caster = successfulCandidate;
            }

            return caster is not null;
        }

        private static OperandCastersTable BuildDefault() =>
            new OperandCastersTableBuilder()
                .Caster(new CompositeOperandCaster())
                .Caster(new OperationOperandCaster())
                .Build();
    }
}
