using System.Collections.Generic;

namespace DiceRoll.Input
{
    public abstract class RPNOperatorParser
    {
        public readonly int RequiredOperands;

        protected RPNOperatorParser(int requiredOperands)
        {
            RequiredOperands = requiredOperands;
        }

        public abstract void TransformOperands(Stack<INode> operands);
    }
}
