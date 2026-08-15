using System;
using System.Collections.Generic;

namespace DiceRoll.Input.Parsing
{
    public static class GrammarExtensions
    {
        public static IGrammar Then(this IGrammar grammar, IGrammar next)
        {
            if (grammar is not ISequentialGrammar composite)
                return new Then(grammar, next);

            composite.Add(next);
            return composite;
        }

        public static IGrammar Or(this IGrammar grammar, IGrammar next)
        {
            if (grammar is not ISequentialGrammar composite)
                return new Or(grammar, next);

            composite.Add(next);
            return composite;
        }

        public static GrammarProbe ProbeAllSequentially(this IEnumerable<IGrammar> grammars, ParseContext context) =>
            ProbeAllSequentially(grammars, context, static (probe, next) => probe.Merge(next));

        public static GrammarProbe ProbeAllSequentially(
            this IEnumerable<IGrammar> grammars,
            ParseContext context,
            Func<GrammarProbe, GrammarProbe, GrammarProbe> aggregator
            )
        {
            if (grammars is null || aggregator is null)
                return GrammarProbe.Failed;

            using IEnumerator<IGrammar> enumerator = grammars.GetEnumerator();

            bool atLeastOne = TryMoveToNext(enumerator);
            
            if (!atLeastOne)
                return GrammarProbe.Failed;

            IGrammar first = enumerator.Current;
            
            GrammarProbe probe = first!.ProbeContext(context);
            
            if (!probe.IsSuccessful)
                return GrammarProbe.Failed;

            while (TryMoveToNext(enumerator))
            {
                GrammarProbe nextProbe = enumerator.Current!.ProbeContext(context);
                probe = aggregator(probe, nextProbe);
            
                if (!probe.IsSuccessful)
                    return GrammarProbe.Failed;
            }
            
            return probe;
        }
        
        public static GrammarProbe ProbeSelectivelyUntilFirst(this IEnumerable<IGrammar> grammars, ParseContext context)
        {
            if (grammars is null)
                return GrammarProbe.Failed;

            using IEnumerator<IGrammar> enumerator = grammars.GetEnumerator();

            while (TryMoveToNext(enumerator))
            {
                GrammarProbe probe = enumerator.Current!.ProbeContext(context);
            
                if (probe.IsSuccessful)
                    return probe;
            }
            
            return GrammarProbe.Failed;
        }
        
        public static GrammarProbe ProbeSelectivelyAll(
            this IEnumerable<IGrammar> grammars,
            ParseContext context,
            Func<GrammarProbe, GrammarProbe, GrammarProbe> aggregator
            )
        {
            if (grammars is null || aggregator is null)
                return GrammarProbe.Failed;

            using IEnumerator<IGrammar> enumerator = grammars.GetEnumerator();

            GrammarProbe? probe = null;
            
            while (TryMoveToNext(enumerator))
            {
                GrammarProbe nextProbe = enumerator.Current!.ProbeContext(context);
                
                if (nextProbe.IsSuccessful)
                    probe = probe is null ? nextProbe : aggregator(probe.Value, nextProbe);
            }
            
            return probe ?? GrammarProbe.Failed;
        }

        private static bool TryMoveToNext<T>(IEnumerator<T> enumerator) where T : class
        {
            while (enumerator.MoveNext())
                if (enumerator.Current is not null)
                    return true;

            return false;
        }
        
        private static void Example1()
        {
            new GrammarGraphBuilder()
                .Add("uscalar", Grammar.UnsignedInteger())
                .Add("scalar", Grammar.AsOptional(Grammar.Literal("-")).Then(Grammar.Reference("uscalar")))
                .Add(
                    "die",
                    Grammar.AsOptional(Grammar.Reference("uscalar"), "1")
                        .Then(Grammar.Literal("d").Then(Grammar.Reference("uscalar")))
                    )
                .Add("bool", Grammar.Literal(Syntax.Params("true", "false"), StringComparison.OrdinalIgnoreCase))
                .Relate("uscalar")
                .To("scalar")
                .AsSuperior();
        }
    }
}