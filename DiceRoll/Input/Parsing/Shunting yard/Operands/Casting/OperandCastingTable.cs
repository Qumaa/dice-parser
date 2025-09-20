using System;
using System.Collections.Generic;
using System.Linq;

namespace DiceRoll.Input.Parsing
{
    public sealed class OperandCastingTable
    {
        public static readonly OperandCastingTable Default = BuilderWithDefaults().Build();

        private readonly OperandCaster[] _casters;

        public OperandCastingTable(IEnumerable<OperandCaster> casters)
        {
            _casters = casters.ToArray();
        }

        /*
         * todo
         * what happens when multiple identical TResult casters have different TSource?
         * won't .CastsFrom() sabotage the system, if any of TSource's are related types?
         * think of <Operation, Assertion> & <Numeric, Assertion>
         * Both sources are related to INode
         */
        public bool IsCasterDefined(Type source, Type result, out OperandCaster caster)
        {
            caster = null;
            
            foreach (OperandCaster candidate in _casters)
            {
                if (!(candidate.CastsTo(result) && candidate.CastsFrom(source)))
                    continue;

                if (caster is not null)
                    return false;

                caster = candidate;
            }

            return caster is not null;
        }

        public static OperandCastersTableBuilder BuilderWithDefaults() =>
            new OperandCastersTableBuilder()
                .Caster(new CompositeOperandCaster())
                .Caster(new OperationOperandCaster());

    }

    public static class OperandCastingTableExtensions
    {
        public static bool TryCast<TSource, TResult>(this OperandCastingTable table, TSource source, out TResult result)
            where TSource : INode where TResult : INode
        {
            if (table.IsCasterDefined<TSource, TResult>(out OperandCaster<TResult> caster))
                return caster.TryCast(source, out result);

            result = default;
            return false;
        }
        
        public static bool IsCasterDefined<TSource, TResult>(this OperandCastingTable table, out OperandCaster<TResult> caster)
            where TSource : INode where TResult : INode
        {
            if (table.IsCasterDefined(typeof(TSource), typeof(TResult), out OperandCaster caster1))
            {
                caster = (OperandCaster<TResult>) caster1;
                return true;
            }

            caster = null;
            return false;
        }
    }
}
