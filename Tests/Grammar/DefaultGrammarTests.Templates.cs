// ReSharper disable ClassNeverInstantiated.Local
namespace Tests.Grammar.Default
{
    public partial class DefaultGrammarTests
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

        private sealed class ImplicitCompositionDice : Template
        {
            public ImplicitCompositionDice() : base("2d6") { }

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
        
        private sealed class Multiply : OperatorContract
        {
            public Multiply() : base(ArgumentsSampler.Numeric, Arity.Binary, "*") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination { CombinationType: CombinationType.Multiply };
        }
        
        private sealed class Add : OperatorContract
        {
            public Add() : base(ArgumentsSampler.Numeric, Arity.Binary, "+") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination { CombinationType: CombinationType.Add };
        }

        private sealed class And : OperatorContract
        {
            public And() : base(ArgumentsSampler.Boolean, Arity.Binary, "&", "&&", "and") { }
            
            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryAssertion { AssertionType: BinaryAssertionType.And };
        }

    #endregion
    }
}
