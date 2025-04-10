namespace DiceRoll.Input
{
    public abstract class OperatorInvoker
    {
        public readonly int RequiredOperands;

        protected OperatorInvoker(int requiredOperands)
        {
            RequiredOperands = requiredOperands;
        }

        public abstract void Invoke(ShuntingYard.OperandsStackAccess operands);
    }
}
