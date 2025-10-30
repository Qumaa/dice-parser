using System;

namespace DiceRoll
{
    // todo: use this to explicitly mark that this node is a parent to a distinct family of nodes
    [AttributeUsage(AttributeTargets.Interface)]
    public sealed class BaseNodeTypeAttribute : Attribute
    {
        
    }
}
