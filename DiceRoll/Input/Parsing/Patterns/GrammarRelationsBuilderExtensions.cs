using System.Runtime.InteropServices;

namespace DiceRoll.Input.Parsing
{
    public static class GrammarCollectionBuilderExtensions
    {
        public static GrammarCollectionBuilder Add(
            this GrammarCollectionBuilder builder,
            string tag,
            IGrammar grammar
            )
        {
            builder.TryAdd(tag, grammar);
            return builder;
        }
        
        public static GrammarCollectionBuilder Relate(
            this GrammarCollectionBuilder builder,
            string tag,
            string tagTo,
            Relation relation
            )
        {
            builder.TryRelate(tag, tagTo, relation);
            return builder;
        }

        public static RelationToSelector<RelationsBuilderWrapper, GrammarCollectionBuilder> Relate(
            this GrammarCollectionBuilder builder,
            string tag
            ) =>
            new(new RelationsBuilderWrapper(builder), tag);

        public static RelationToSelector<GraphBuilderWrapper, GrammarGraphBuilder> Relate(
            this GrammarGraphBuilder builder,
            string tag
            ) =>
            new(new GraphBuilderWrapper(builder), tag);

        [StructLayout(LayoutKind.Auto)]
        public readonly struct RelationToSelector<T, U> where T : struct, IWrapper<U>
        {
            private readonly T _wrapper;
            private readonly string _tag;
            
            public RelationToSelector(T wrapper, string tag)
            {
                _wrapper = wrapper;
                _tag = tag;
            }

            public RelationKindSelector<T, U> To(string tag) =>
                new(_wrapper, _tag, tag);
        }

        [StructLayout(LayoutKind.Auto)]
        public readonly struct RelationKindSelector<T, U> where T : struct, IWrapper<U>
        {
            private readonly T _wrapper;
            private readonly string _tag;
            private readonly string _tagTo;
            
            public RelationKindSelector(T wrapper, string tag, string tagTo)
            {
                _wrapper = wrapper;
                _tag = tag;
                _tagTo = tagTo;
            }

            public U AsSuperior() =>
                Relate(Relation.Superior);

            public U AsInferior() =>
                Relate(Relation.Inferior);

            private U Relate(Relation relation)
            {
                // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
                _wrapper.Relate(_tag, _tagTo, relation);
                return _wrapper.Builder;
            }
        }

        public interface IWrapper<out T>
        {
            T Builder { get; }
            void Relate(string tag, string tagTo, Relation relation);
        }

        public readonly struct RelationsBuilderWrapper : IWrapper<GrammarCollectionBuilder>
        {
            public GrammarCollectionBuilder Builder { get; }
            
            public RelationsBuilderWrapper(GrammarCollectionBuilder builder)
            {
                Builder = builder;
            }

            public void Relate(string tag, string tagTo, Relation relation) =>
                Builder.Relate(tag, tagTo, relation);
        }

        public readonly struct GraphBuilderWrapper : IWrapper<GrammarGraphBuilder>
        {
            public GrammarGraphBuilder Builder { get; }

            public GraphBuilderWrapper(GrammarGraphBuilder builder)
            {
                Builder = builder;
            }

            public void Relate(string tag, string tagTo, Relation relation) =>
                Builder.Relate(tag, tagTo, relation);
        }
    }
}
