using System.Collections.Generic;

namespace DiceRoll
{
    public interface ISequence<out T> : INode, IEnumerable<T> where T : INode { }
    
    [BaseType("any sequence")]
    public interface ISequence : ISequence<INode> { }
}
