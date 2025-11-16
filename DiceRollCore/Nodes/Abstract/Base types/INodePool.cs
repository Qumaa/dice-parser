using System.Collections.Generic;

namespace DiceRoll
{
    public interface INodePool<out T> : INode, IEnumerable<T> where T : INode { }
    
    public interface INodePool : INodePool<INode> { }
}
