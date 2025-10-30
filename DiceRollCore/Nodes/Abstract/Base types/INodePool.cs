using System.Collections.Generic;

namespace DiceRoll
{
    public interface INodePool : INode, IEnumerable<INode> { }
}
