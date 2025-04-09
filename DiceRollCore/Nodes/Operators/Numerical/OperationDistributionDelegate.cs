namespace DiceRoll
{
    public delegate OptionalRollProbabilityDistribution OperationDistributionDelegate(RollProbabilityDistribution left,
        RollProbabilityDistribution right);
}
