using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace Dice
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            
            Constant dc = new(10);
            Dice dice = new(new Random(0), 20, 2);
            LikelihoodExpression dcExpr = new(dice, Operation.GreaterThan, dc);
            Likelihood likelihood = dcExpr.Evaluate();
            
            stopwatch.Stop();
            
            Console.WriteLine(LikelihoodToString(likelihood));
            
            Console.WriteLine($"Elapsed {stopwatch.ElapsedMilliseconds}ms ({stopwatch.Elapsed.Seconds:f2}s)");
        }

        private static string ProbabilityToString(ProbabilityData probabilityData) =>
            probabilityData.Aggregate("",
                (s, probability) =>
                    s + $"Probability of {probability.RollResult.Value} is {LikelihoodToString(probability.Likelihood)}%\n");

        private static string LikelihoodToString(Likelihood likelihood) =>
            $"{likelihood.Value * 100d:F2}%";
    }

    public interface IExpression<T>
    {
        T Evaluate();
    }

    public interface IAnalyzable : IExpression<RollResult>
    {
        ProbabilityData GetProbabilityData();
    }

    public static class Expression
    {
        public static Constant Constant(int value) =>
            new(value);
    }
    
    public class Constant : IAnalyzable
    {
        private readonly int _value;

        public Constant(int value) 
        {
            _value = value;
        }

        public RollResult Evaluate() =>
            new(_value);

        public ProbabilityData GetProbabilityData() =>
            ProbabilityData.OfConstant(_value);
    }
    
    public class Dice : IAnalyzable
    {
        private readonly Random _random;
        private readonly int _faces;
        private readonly int _number;

        public ProbabilityData GetProbabilityData() =>
            ProbabilityData.OfDice(_faces, _number);

        public Dice(Random random, int faces, int number)
        {
            _random = random;
            _faces = faces;
            _number = number;
        }

        public RollResult Evaluate() =>
            new(_random.Next(_number, _faces * _number));
    }

    public sealed class LikelihoodExpression : IExpression<Likelihood>
    {
        private readonly IAnalyzable _left;
        private readonly IAnalyzable _right;
        private readonly Operation _operation;

        public LikelihoodExpression(IAnalyzable left, Operation operation, IAnalyzable right)
        {
            _left = left;
            _operation = operation;
            _right = right;
        }

        public Likelihood Evaluate() =>
            GetDelegate(_operation).Invoke(_left.GetProbabilityData(), _right.GetProbabilityData());

        private static OperationDelegate GetDelegate(Operation operation)
        {
            return operation switch
            {
                Operation.Equal => Operations.Equal,
                Operation.NotEqual => Operations.Not(Operations.Equal),
                Operation.GreaterThan => Operations.GreaterThan,
                Operation.GreaterThanOrEqual => Operations.Not(Operations.LessThan),
                Operation.LessThan => Operations.LessThan,
                Operation.LessThanOrEqual => Operations.Not(Operations.GreaterThan),
                _ => throw new ArgumentOutOfRangeException(nameof(operation), operation, null)
            };
        }

        private static class Operations
        {
            
            public static OperationDelegate Not(OperationDelegate operationDelegate) =>
                (left, right) => operationDelegate(left, right).Inversed();

            public static Likelihood Equal(ProbabilityData left, ProbabilityData right)
            {
                if (left.Max.Value < right.Min.Value || right.Max.Value < left.Min.Value)
                    return Likelihood.Zero;
                
                return Evaluate(left, right, (leftRoll, rightRoll) => leftRoll.Value == rightRoll.Value);
            }

            public static Likelihood GreaterThan(ProbabilityData left, ProbabilityData right)
            {
                if (left.Max.Value < right.Min.Value)
                    return Likelihood.Zero;

                return Evaluate(left, right, (leftRoll, rightRoll) => leftRoll.Value > rightRoll.Value);
            }

            public static Likelihood LessThan(ProbabilityData left, ProbabilityData right)
            {
                if ( right.Max.Value < left.Min.Value)
                    return Likelihood.Zero;
                
                return Evaluate(left, right, (leftRoll, rightRoll) => leftRoll.Value < rightRoll.Value);
            }
        }

        private static Likelihood Evaluate(ProbabilityData left, ProbabilityData right,
            Func<RollResult, RollResult, bool> predicate) =>
            new(left
                .SelectMany(_ => right, (left, right) => new { left, right })
                .Select(rolls => (rolls, probability: rolls.left.Likelihood.Value * rolls.right.Likelihood.Value))
                .Where(t => predicate(t.rolls.left.RollResult, t.rolls.right.RollResult))
                .Select(t => t.probability)
                .Sum());

        private delegate Likelihood OperationDelegate(ProbabilityData left, ProbabilityData right);
    }

    public enum Operation
    {
        Equal = 0,
        NotEqual = 1,
        GreaterThan = 2,
        GreaterThanOrEqual = 3,
        LessThan = 4,
        LessThanOrEqual = 5
    }

    public readonly struct RollResult
    {
        public readonly int Value;
        
        public RollResult(int value)
        {
            Value = value;
        }
    }

    public sealed class ProbabilityData : IEnumerable<RollProbability>
    {
        public readonly RollResult Min;
        public readonly RollResult Max;
        
        private readonly IEnumerable<RollProbability> _probabilities;

        public ProbabilityData(IEnumerable<RollProbability> probabilities)
        {
            _probabilities = probabilities;
            
            Init(out Min, out Max);
        }

        private void Init(out RollResult min, out RollResult max)
        {
            using IEnumerator<RollProbability> enumerator = _probabilities.GetEnumerator();

            if (!enumerator.MoveNext())
            {
                min = max = default;
                return;
            }

            min = max = enumerator.Current.RollResult;

            while (enumerator.MoveNext())
            {
                RollResult current = enumerator.Current.RollResult;

                if (current.Value < min.Value)
                    min = current;

                if (current.Value > max.Value)
                    max = current;
            }
        }

        public ProbabilityData Combine(ProbabilityData other)
        {
            int minValue = Min.Value + other.Min.Value;
            int maxValue = Max.Value + other.Max.Value;

            double[] newProbabilities = new double[maxValue - minValue + 1];

            foreach (RollProbability thisProbability in _probabilities)
            foreach (RollProbability otherProbability in other)
            {
                int value = thisProbability.RollResult.Value + otherProbability.RollResult.Value;
                double probability = thisProbability.Likelihood.Value * otherProbability.Likelihood.Value;

                newProbabilities[value - minValue] += probability;
            }

            return new ProbabilityData(newProbabilities
                .Select(
                    (d, i) => (RollProbability) new(i + minValue, d)
                )
            );
        }

        public IEnumerator<RollProbability> GetEnumerator() =>
            _probabilities.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        public static ProbabilityData OfDice(int faces, int count = 1)
        {
            Likelihood likelihood = new(1d / faces);

            ProbabilityData data = new(Enumerable.Range(1, faces)
                .Select(index => new RollResult(index))
                .Select(rollResult => new RollProbability(rollResult, likelihood)));

            if (count <= 1)
                goto ret;

            for (int i = 1; i < count; i++)
                data = data.Combine(data);

            ret:
            return data;
        }

        public static ProbabilityData OfConstant(int value) =>
            new(Enumerable.Repeat(new RollProbability(value, Likelihood.Hundred), 1));
    }

    [StructLayout(LayoutKind.Auto)]
    public readonly struct RollProbability
    {
        public readonly RollResult RollResult;
        public readonly Likelihood Likelihood;
        
        public RollProbability(RollResult rollResult, Likelihood likelihood)
        {
            RollResult = rollResult;
            Likelihood = likelihood;
        }

        public RollProbability(int rollResult, double likelihood) : 
            this(new RollResult(rollResult), new Likelihood(likelihood)) 
        { }

        public RollProbability(int rollResult, Likelihood likelihood) : this(new RollResult(rollResult), likelihood) { }
    }

    public readonly struct Likelihood
    {
        public static Likelihood Hundred => new(1d);
        public static Likelihood Zero => new(0d);
        
        public readonly double Value;

        public Likelihood(double value) 
        {
            Value = value;
        }

        public Likelihood Inversed() =>
            new(1d - Value);
    }
}


