using System.Runtime.InteropServices;

namespace DiceRoll.Input.Parsing
{
    public static class GrammarRelationsBuilderExtensions
    {
        public static RelationToSelector<RelationsBuilderWrapper> Relate(this GrammarRelationsBuilder builder, string tag) =>
            new(new RelationsBuilderWrapper(builder), tag);

        public static RelationToSelector<GraphBuilderWrapper> Relate(this GrammarGraphBuilder builder, string tag) =>
            new(new GraphBuilderWrapper(builder), tag);

        [StructLayout(LayoutKind.Auto)]
        public readonly struct RelationToSelector<T> where T : struct, IWrapper
        {
            private readonly T _builder;
            private readonly string _tag;
            
            public RelationToSelector(T builder, string tag)
            {
                _builder = builder;
                _tag = tag;
            }

            public RelationKindSelector<T> To(string tag) =>
                new(_builder, _tag, tag);
        }

        [StructLayout(LayoutKind.Auto)]
        public readonly struct RelationKindSelector<T> where T : struct, IWrapper
        {
            private readonly T _builder;
            private readonly string _tag;
            private readonly string _tagTo;
            
            public RelationKindSelector(T builder, string tag, string tagTo)
            {
                _builder = builder;
                _tag = tag;
                _tagTo = tagTo;
            }

            public T AsSuperior() =>
                Relate(Relation.Superior);

            public T AsInferior() =>
                Relate(Relation.Inferior);

            private T Relate(Relation relation)
            {
                // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable
                _builder.Relate(_tag, _tagTo, relation);
                return _builder;
            }
        }

        public interface IWrapper
        {
            void Relate(string tag, string tagTo, Relation relation);
        }

        public readonly struct RelationsBuilderWrapper : IWrapper
        {
            public readonly GrammarRelationsBuilder Builder;
            
            public RelationsBuilderWrapper(GrammarRelationsBuilder builder)
            {
                Builder = builder;
            }

            public void Relate(string tag, string tagTo, Relation relation) =>
                Builder.Relate(tag, tagTo, relation);
        }

        public readonly struct GraphBuilderWrapper : IWrapper
        {
            public readonly GrammarGraphBuilder Builder;
            
            public GraphBuilderWrapper(GrammarGraphBuilder builder)
            {
                Builder = builder;
            }

            public void Relate(string tag, string tagTo, Relation relation) =>
                Builder.Relate(tag, tagTo, relation);
        }
    }
}