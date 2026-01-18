using System.Collections.Generic;

namespace DiceRoll
{
    // todo base type with formattable name? (e.g. not static string but "sequence (name of T)")
    [BaseType("typed sequence")]
    public interface ISequence<out T> : ISequence, IReadOnlyList<T> where T : INode { }

    public interface ISequence : INode { }
}
