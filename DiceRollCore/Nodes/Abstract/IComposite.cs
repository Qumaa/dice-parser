namespace DiceRoll
{
    public interface IComposite : INode<CompositeEvaluation>
    {
        INumeric AsNumeric { get; }
    }
}
