// ReSharper disable ClassNeverInstantiated.Local
namespace Tests.Syntax.Default
{
    public partial class DefaultSyntaxTests
    {
    #region Operands

        private sealed class SingleNumber : Template
        {
            public SingleNumber() : base("15") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is NumericConstant { Value: 15 };
        }

        private sealed class SingleBoolean : Template
        {
            public SingleBoolean() : base("true") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is BinaryConstant { Value: true };
        }

        private sealed class SingleDie : Template
        {
            public SingleDie() : base("d8") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Die { Faces: 8 };
        }

        private sealed class SingleDice : Template
        {
            public SingleDice() : base("2d6") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Composite
                {
                    UnderlyingNode: Combination
                    {
                        CombinationType: CombinationType.Add, Left: Die { Faces: 6 }, Right: Die { Faces: 6 }
                    }
                };
        }

        private sealed class SimpleSequence : Template
        {
            public SimpleSequence() : base("d12 15") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is ISequence<INumeric> and [Die { Faces: 12 }, NumericConstant { Value: 15 }];
        }

        private sealed class NestedSequence : Template
        {
            public NestedSequence() : base("(4 d4) ( d12 15 )") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is ISequence<ISequence<INumeric>> and
                [
                    [NumericConstant { Value: 4 }, Die { Faces: 4 }],
                    [Die { Faces: 12 }, NumericConstant { Value: 15 }]
                ];
        }

    #endregion

    #region Operators

        private sealed class SimpleAddition : Template
        {
            public SimpleAddition() : base("d4 + 1") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.Add,
                    Left: Die { Faces: 4 },
                    Right: NumericConstant { Value: 1 }
                };
        }

        private sealed class SimpleSubtraction : Template
        {
            public SimpleSubtraction() : base("d4 - 1") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.Subtract,
                    Left: Die { Faces: 4 },
                    Right: NumericConstant { Value: 1 }
                };
        }

        private sealed class SimpleNegation : Template
        {
            public SimpleNegation() : base("- 1") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Negation { Source: NumericConstant { Value: 1 } };
        }

        private sealed class SimpleMultiplication : Template
        {
            public SimpleMultiplication() : base("d4 * 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.Multiply,
                    Left: Die { Faces: 4 },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class SimpleDivideRoundDown : Template
        {
            public SimpleDivideRoundDown() : base("d4 / 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.DivideRoundDownwards,
                    Left: Die { Faces: 4 },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class SimpleDivideRoundUp : Template
        {
            public SimpleDivideRoundUp() : base("d4 // 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.DivideRoundUpwards,
                    Left: Die { Faces: 4 },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class SimpleEqualNumeric : Template
        {
            public SimpleEqualNumeric() : base("d4 {0} 3", "=", "==") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.Equal, Left: Die { Faces: 4 }, Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class SimpleNotEqualNumeric : Template
        {
            public SimpleNotEqualNumeric() : base("d4 {0} 3", "!=", "=/=") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.NotEqual,
                    Left: Die { Faces: 4 },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class SimpleEqualBinary : Template
        {
            public SimpleEqualBinary() : base("false {0} true", "=", "==") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryAssertion
                {
                    AssertionType: BinaryAssertionType.Equal,
                    Left: BinaryConstant { Value: false },
                    Right: BinaryConstant { Value: true }
                };
        }

        private sealed class SimpleNotEqualBinary : Template
        {
            public SimpleNotEqualBinary() : base("false {0} true", "!=", "=/=") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryAssertion
                {
                    AssertionType: BinaryAssertionType.NotEqual,
                    Left: BinaryConstant { Value: false },
                    Right: BinaryConstant { Value: true }
                };
        }

        private sealed class SimpleGreaterThan : Template
        {
            public SimpleGreaterThan() : base("d4 > 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.GreaterThan,
                    Left: Die { Faces: 4 },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class SimpleGreaterThanOrEqual : Template
        {
            public SimpleGreaterThanOrEqual() : base("d4 >= 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.GreaterThanOrEqual,
                    Left: Die { Faces: 4 },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class SimpleLessThan : Template
        {
            public SimpleLessThan() : base("d4 < 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.LessThan,
                    Left: Die { Faces: 4 },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class SimpleLessThanOrEqual : Template
        {
            public SimpleLessThanOrEqual() : base("d4 <= 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.LessThanOrEqual,
                    Left: Die { Faces: 4 },
                    Right: NumericConstant { Value: 3 }
                };
        }
        
        private sealed class SimpleAnd : Template
        {
            public SimpleAnd() : base("false {0} true", "&", "&&", "and") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryAssertion
                {
                    AssertionType: BinaryAssertionType.And,
                    Left: BinaryConstant { Value: false },
                    Right: BinaryConstant { Value: true }
                };
        }

        private sealed class SimpleOr : Template
        {
            public SimpleOr() : base("false {0} true", "|", "||", "or") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryAssertion
                {
                    AssertionType: BinaryAssertionType.Or,
                    Left: BinaryConstant { Value: false },
                    Right: BinaryConstant { Value: true }
                };
        }
        
        private sealed class SimpleNot : Template
        {
            public SimpleNot() : base("{0} false", "!", "not") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is NotAssertion
                {
                    Source: BinaryConstant { Value: false }
                };
        }

    #endregion

    #region Precedence

        private sealed class PrecedenceOverridenByParenthesis : Template
        {
            public PrecedenceOverridenByParenthesis() : base("(1 + 2) * 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.Multiply,
                    Left: Combination
                    {
                        CombinationType: CombinationType.Add,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }
        
        private sealed class BinaryDice : OperatorContract
        {
            public BinaryDice() : base(sampler: ArgumentsSampler.Numeric, arity: Arity.Binary, false, "d") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is IComposite;
        }
        
        private sealed class UnaryDie : OperatorContract
        {
            public UnaryDie() : base(sampler: ArgumentsSampler.Numeric, arity: Arity.PrefixUnary, false, "d") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Die;
        }
        
        private sealed class Times : OperatorContract
        {
            public Times() : base(sampler: ArgumentsSampler.Numeric, arity: Arity.Binary, false, ":", "times") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is ISequence<INumeric>;
        }
        
        private sealed class Range : OperatorContract
        {
            public Range() : base(sampler: new Sampler(), arity: Arity.Binary, false, "..", "through") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is ISequence<INumeric>;
            
            private sealed class Sampler : ArgumentsSampler
            {
                public override string[] Generate(int operatorIndex, int argumentsCount) =>
                    Enumerable.Range(operatorIndex + 1, argumentsCount).Select(x => x.ToString()).ToArray();

                protected override bool Verify(INode argument, int operatorIndex, int argumentIndex) =>
                    argument is NumericConstant constant && constant.Value == operatorIndex + 1 + argumentIndex;
            }
        }
        
        private sealed class Not : OperatorContract
        {
            public Not() : base(sampler: ArgumentsSampler.Boolean, arity: Arity.PrefixUnary, "!", "not") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is NotAssertion;
        }
        
        private sealed class Negate : OperatorContract
        {
            public Negate() : base(sampler: ArgumentsSampler.Numeric, arity: Arity.PrefixUnary, "-") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Negation;
        }
        
        private sealed class UnaryTotal : OperatorContract
        {
            public UnaryTotal() : base(sampler: ArgumentsSampler.Sequence, arity: Arity.PostfixUnary, false, "s", "sum", "summation", "total") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Composite { UnderlyingNode: Combination { CombinationType: CombinationType.Add } } or INumeric;
        }
        
        private sealed class UnaryHighest : OperatorContract
        {
            public UnaryHighest() : base(sampler: ArgumentsSampler.Sequence, arity: Arity.PostfixUnary, false, "h", "highest") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Composite { UnderlyingNode: DefaultSelection { SelectionType: SelectionType.Highest } } or INumeric;
        }

        private sealed class UnaryLowest : OperatorContract
        {
            public UnaryLowest() : base(sampler: ArgumentsSampler.Sequence, arity: Arity.PostfixUnary, false, "l", "lowest") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Composite { UnderlyingNode: DefaultSelection { SelectionType: SelectionType.Lowest } } or INumeric;
        }
        
        private sealed class Multiply : OperatorContract
        {
            public Multiply() : base(ArgumentsSampler.Numeric, Arity.Binary, "*") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination { CombinationType: CombinationType.Multiply };
        }

        private sealed class DivideRoundUp : OperatorContract
        {
            public DivideRoundUp() : base(ArgumentsSampler.Numeric, Arity.Binary, "//") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination { CombinationType: CombinationType.DivideRoundUpwards };
        }

        private sealed class DivideRoundDown : OperatorContract
        {
            public DivideRoundDown() : base(ArgumentsSampler.Numeric, Arity.Binary, "/") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination { CombinationType: CombinationType.DivideRoundDownwards };
        }
        
        private sealed class Add : OperatorContract
        {
            public Add() : base(ArgumentsSampler.Numeric, Arity.Binary, "+") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination { CombinationType: CombinationType.Add };
        }

        private sealed class Subtract : OperatorContract
        {
            public Subtract() : base(ArgumentsSampler.Numeric, Arity.Binary, "-") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination { CombinationType: CombinationType.Subtract };
        }
        
        private sealed class GreaterThanOrEqual : OperatorContract
        {
            public GreaterThanOrEqual() : base(sampler: ArgumentsSampler.Numeric, arity: Arity.Binary, ">=") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation { OperationType: OperationType.GreaterThanOrEqual } ||
                node is OperationAsAssertion oas && MeetsExpectedPattern(oas.Source);
        }

        private sealed class LessThanOrEqual : OperatorContract
        {
            public LessThanOrEqual() : base(sampler: ArgumentsSampler.Numeric, arity: Arity.Binary, "<=") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation { OperationType: OperationType.LessThanOrEqual } ||
                node is OperationAsAssertion oas && MeetsExpectedPattern(oas.Source);
        }
        
        private sealed class GreaterThan : OperatorContract
        {
            public GreaterThan() : base(sampler: ArgumentsSampler.Numeric, arity: Arity.Binary, ">") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation { OperationType: OperationType.GreaterThan } ||
                node is OperationAsAssertion oas && MeetsExpectedPattern(oas.Source);
        }

        private sealed class LessThan : OperatorContract
        {
            public LessThan() : base(sampler: ArgumentsSampler.Numeric, arity: Arity.Binary, "<") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation { OperationType: OperationType.LessThan } ||
                node is OperationAsAssertion oas && MeetsExpectedPattern(oas.Source);
        }
        
        private sealed class NumericEqual : OperatorContract
        {
            public NumericEqual() : base(sampler: ArgumentsSampler.Numeric, arity: Arity.Binary, "=", "==") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation { OperationType: OperationType.Equal } ||
                node is OperationAsAssertion oas && MeetsExpectedPattern(oas.Source);
        }
        
        private sealed class NumericNotEqual : OperatorContract
        {
            public NumericNotEqual() : base(sampler: ArgumentsSampler.Numeric, arity: Arity.Binary, "!=", "=/=") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation { OperationType: OperationType.NotEqual } ||
                node is OperationAsAssertion oas && MeetsExpectedPattern(oas.Source);
        }
        
        private sealed class BooleanEqual : OperatorContract
        {
            public BooleanEqual() : base(sampler: ArgumentsSampler.Boolean, arity: Arity.Binary, "=", "==") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryAssertion { AssertionType: BinaryAssertionType.Equal };
        }
        
        private sealed class BooleanNotEqual : OperatorContract
        {
            public BooleanNotEqual() : base(sampler: ArgumentsSampler.Boolean, arity: Arity.Binary, "!=", "=/=") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryAssertion { AssertionType: BinaryAssertionType.NotEqual };
        }

        private sealed class And : OperatorContract
        {
            public And() : base(ArgumentsSampler.Boolean, Arity.Binary, "&", "&&", "and") { }
            
            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryAssertion { AssertionType: BinaryAssertionType.And };
        }
        
        private sealed class Or : OperatorContract
        {
            public Or() : base(sampler: ArgumentsSampler.Boolean, arity: Arity.Binary, "|", "||", "or") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryAssertion { AssertionType: BinaryAssertionType.Or };
        }

    #endregion
    }
}
