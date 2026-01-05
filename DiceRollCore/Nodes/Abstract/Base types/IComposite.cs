namespace DiceRoll
{
    [BaseType("numerical set")]
    public interface IComposite : INumeric, ISequence<INumeric> { }
}
