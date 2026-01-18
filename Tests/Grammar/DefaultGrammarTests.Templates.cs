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

    #region Add

        private sealed class PrecedenceAddOverAdd : Template
        {
            public PrecedenceAddOverAdd() : base("1 + 2 + 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.Add,
                    Left: Combination
                    {
                        CombinationType: CombinationType.Add,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceAddOverSubtract : Template
        {
            public PrecedenceAddOverSubtract() : base("1 + 2 - 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.Subtract,
                    Left: Combination
                    {
                        CombinationType: CombinationType.Add,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceAddOverNegation : Template
        {
            public PrecedenceAddOverNegation() : base("1 + - 2") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.Add,
                    Left: NumericConstant { Value: 1 },
                    Right: Negation
                    {
                        Source: NumericConstant { Value: 2 }
                    }
                };
        }

        private sealed class PrecedenceAddOverMultiply : Template
        {
            public PrecedenceAddOverMultiply() : base("1 + 2 * 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.Add,
                    Left: NumericConstant { Value: 1 },
                    Right: Combination
                    {
                        CombinationType: CombinationType.Multiply,
                        Left: NumericConstant { Value: 2 },
                        Right: NumericConstant { Value: 3 }
                    }
                };
        }

        private sealed class PrecedenceAddOverDivideRoundDown : Template
        {
            public PrecedenceAddOverDivideRoundDown() : base("1 + 2 / 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.Add,
                    Left: NumericConstant { Value: 1 },
                    Right: Combination
                    {
                        CombinationType: CombinationType.DivideRoundDownwards,
                        Left: NumericConstant { Value: 2 },
                        Right: NumericConstant { Value: 3 }
                    }
                };
        }

        private sealed class PrecedenceAddOverDivideRoundUp : Template
        {
            public PrecedenceAddOverDivideRoundUp() : base("1 + 2 // 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.Add,
                    Left: NumericConstant { Value: 1 },
                    Right: Combination
                    {
                        CombinationType: CombinationType.DivideRoundUpwards,
                        Left: NumericConstant { Value: 2 },
                        Right: NumericConstant { Value: 3 }
                    }
                };
        }

        private sealed class PrecedenceAddOverGreaterThan : Template
        {
            public PrecedenceAddOverGreaterThan() : base("1 + 2 > 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.GreaterThan,
                    Left: Combination
                    {
                        CombinationType: CombinationType.Add,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceAddOverLessThan : Template
        {
            public PrecedenceAddOverLessThan() : base("1 + 2 < 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.LessThan,
                    Left: Combination
                    {
                        CombinationType: CombinationType.Add,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceAddOverGreaterThanOrEqual : Template
        {
            public PrecedenceAddOverGreaterThanOrEqual() : base("1 + 2 >= 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.GreaterThanOrEqual,
                    Left: Combination
                    {
                        CombinationType: CombinationType.Add,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceAddOverLessThanOrEqual : Template
        {
            public PrecedenceAddOverLessThanOrEqual() : base("1 + 2 <= 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.LessThanOrEqual,
                    Left: Combination
                    {
                        CombinationType: CombinationType.Add,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceAddOverEqual : Template
        {
            public PrecedenceAddOverEqual() : base("1 + 2 = 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.Equal,
                    Left: Combination
                    {
                        CombinationType: CombinationType.Add,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceAddOverNotEqual : Template
        {
            public PrecedenceAddOverNotEqual() : base("1 + 2 != 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.NotEqual,
                    Left: Combination
                    {
                        CombinationType: CombinationType.Add,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

    #endregion

    #region Subtract

        private sealed class PrecedenceSubtractOverAdd : Template
        {
            public PrecedenceSubtractOverAdd() : base("1 - 2 + 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.Add,
                    Left: Combination
                    {
                        CombinationType: CombinationType.Subtract,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceSubtractOverSubtract : Template
        {
            public PrecedenceSubtractOverSubtract() : base("1 - 2 - 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.Subtract,
                    Left: Combination
                    {
                        CombinationType: CombinationType.Subtract,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceSubtractOverNegation : Template
        {
            public PrecedenceSubtractOverNegation() : base("1 - - 2") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.Subtract,
                    Left: NumericConstant { Value: 1 },
                    Right: Negation
                    {
                        Source: NumericConstant { Value: 2 }
                    }
                };
        }

        private sealed class PrecedenceSubtractOverMultiply : Template
        {
            public PrecedenceSubtractOverMultiply() : base("1 - 2 * 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.Subtract,
                    Left: NumericConstant { Value: 1 },
                    Right: Combination
                    {
                        CombinationType: CombinationType.Multiply,
                        Left: NumericConstant { Value: 2 },
                        Right: NumericConstant { Value: 3 }
                    }
                };
        }

        private sealed class PrecedenceSubtractOverDivideRoundDown : Template
        {
            public PrecedenceSubtractOverDivideRoundDown() : base("1 - 2 / 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.Subtract,
                    Left: NumericConstant { Value: 1 },
                    Right: Combination
                    {
                        CombinationType: CombinationType.DivideRoundDownwards,
                        Left: NumericConstant { Value: 2 },
                        Right: NumericConstant { Value: 3 }
                    }
                };
        }

        private sealed class PrecedenceSubtractOverDivideRoundUp : Template
        {
            public PrecedenceSubtractOverDivideRoundUp() : base("1 - 2 // 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.Subtract,
                    Left: NumericConstant { Value: 1 },
                    Right: Combination
                    {
                        CombinationType: CombinationType.DivideRoundUpwards,
                        Left: NumericConstant { Value: 2 },
                        Right: NumericConstant { Value: 3 }
                    }
                };
        }

        private sealed class PrecedenceSubtractOverGreaterThan : Template
        {
            public PrecedenceSubtractOverGreaterThan() : base("1 - 2 > 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.GreaterThan,
                    Left: Combination
                    {
                        CombinationType: CombinationType.Subtract,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceSubtractOverLessThan : Template
        {
            public PrecedenceSubtractOverLessThan() : base("1 - 2 < 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.LessThan,
                    Left: Combination
                    {
                        CombinationType: CombinationType.Subtract,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceSubtractOverGreaterThanOrEqual : Template
        {
            public PrecedenceSubtractOverGreaterThanOrEqual() : base("1 - 2 >= 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.GreaterThanOrEqual,
                    Left: Combination
                    {
                        CombinationType: CombinationType.Subtract,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceSubtractOverLessThanOrEqual : Template
        {
            public PrecedenceSubtractOverLessThanOrEqual() : base("1 - 2 <= 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.LessThanOrEqual,
                    Left: Combination
                    {
                        CombinationType: CombinationType.Subtract,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceSubtractOverEqual : Template
        {
            public PrecedenceSubtractOverEqual() : base("1 - 2 = 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.Equal,
                    Left: Combination
                    {
                        CombinationType: CombinationType.Subtract,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceSubtractOverNotEqual : Template
        {
            public PrecedenceSubtractOverNotEqual() : base("1 - 2 != 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.NotEqual,
                    Left: Combination
                    {
                        CombinationType: CombinationType.Subtract,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

    #endregion

    #region Multiply

        private sealed class PrecedenceMultiplyOverAdd : Template
        {
            public PrecedenceMultiplyOverAdd() : base("1 * 2 + 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.Add,
                    Left: Combination
                    {
                        CombinationType: CombinationType.Multiply,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceMultiplyOverSubtract : Template
        {
            public PrecedenceMultiplyOverSubtract() : base("1 * 2 - 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.Subtract,
                    Left: Combination
                    {
                        CombinationType: CombinationType.Multiply,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceMultiplyOverNegation : Template
        {
            public PrecedenceMultiplyOverNegation() : base("1 * - 2") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.Multiply,
                    Left: NumericConstant { Value: 1 },
                    Right: Negation
                    {
                        Source: NumericConstant { Value: 2 }
                    }
                };
        }

        private sealed class PrecedenceMultiplyOverMultiply : Template
        {
            public PrecedenceMultiplyOverMultiply() : base("1 * 2 * 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.Multiply,
                    Left: Combination
                    {
                        CombinationType: CombinationType.Multiply,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceMultiplyOverDivideRoundDown : Template
        {
            public PrecedenceMultiplyOverDivideRoundDown() : base("1 * 2 / 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.DivideRoundDownwards,
                    Left: Combination
                    {
                        CombinationType: CombinationType.Multiply,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceMultiplyOverDivideRoundUp : Template
        {
            public PrecedenceMultiplyOverDivideRoundUp() : base("1 * 2 // 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.DivideRoundUpwards,
                    Left: Combination
                    {
                        CombinationType: CombinationType.Multiply,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceMultiplyOverGreaterThan : Template
        {
            public PrecedenceMultiplyOverGreaterThan() : base("1 * 2 > 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.GreaterThan,
                    Left: Combination
                    {
                        CombinationType: CombinationType.Multiply,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceMultiplyOverLessThan : Template
        {
            public PrecedenceMultiplyOverLessThan() : base("1 * 2 < 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.LessThan,
                    Left: Combination
                    {
                        CombinationType: CombinationType.Multiply,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceMultiplyOverGreaterThanOrEqual : Template
        {
            public PrecedenceMultiplyOverGreaterThanOrEqual() : base("1 * 2 >= 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.GreaterThanOrEqual,
                    Left: Combination
                    {
                        CombinationType: CombinationType.Multiply,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceMultiplyOverLessThanOrEqual : Template
        {
            public PrecedenceMultiplyOverLessThanOrEqual() : base("1 * 2 <= 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.LessThanOrEqual,
                    Left: Combination
                    {
                        CombinationType: CombinationType.Multiply,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceMultiplyOverEqual : Template
        {
            public PrecedenceMultiplyOverEqual() : base("1 * 2 = 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.Equal,
                    Left: Combination
                    {
                        CombinationType: CombinationType.Multiply,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceMultiplyOverNotEqual : Template
        {
            public PrecedenceMultiplyOverNotEqual() : base("1 * 2 != 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.NotEqual,
                    Left: Combination
                    {
                        CombinationType: CombinationType.Multiply,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

    #endregion

    #region Divide round down

        private sealed class PrecedenceDivideRoundDownOverAdd : Template
        {
            public PrecedenceDivideRoundDownOverAdd() : base("1 / 2 + 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.Add,
                    Left: Combination
                    {
                        CombinationType: CombinationType.DivideRoundDownwards,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceDivideRoundDownOverSubtract : Template
        {
            public PrecedenceDivideRoundDownOverSubtract() : base("1 / 2 - 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.Subtract,
                    Left: Combination
                    {
                        CombinationType: CombinationType.DivideRoundDownwards,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceDivideRoundDownOverNegation : Template
        {
            public PrecedenceDivideRoundDownOverNegation() : base("1 / - 2") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.DivideRoundDownwards,
                    Left: NumericConstant { Value: 1 },
                    Right: Negation
                    {
                        Source: NumericConstant { Value: 2 }
                    }
                };
        }

        private sealed class PrecedenceDivideRoundDownOverMultiply : Template
        {
            public PrecedenceDivideRoundDownOverMultiply() : base("1 / 2 * 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.Multiply,
                    Left: Combination
                    {
                        CombinationType: CombinationType.DivideRoundDownwards,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceDivideRoundDownOverDivideRoundDown : Template
        {
            public PrecedenceDivideRoundDownOverDivideRoundDown() : base("1 / 2 / 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.DivideRoundDownwards,
                    Left: Combination
                    {
                        CombinationType: CombinationType.DivideRoundDownwards,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceDivideRoundDownOverDivideRoundUp : Template
        {
            public PrecedenceDivideRoundDownOverDivideRoundUp() : base("1 / 2 // 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.DivideRoundUpwards,
                    Left: Combination
                    {
                        CombinationType: CombinationType.DivideRoundDownwards,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceDivideRoundDownOverGreaterThan : Template
        {
            public PrecedenceDivideRoundDownOverGreaterThan() : base("1 / 2 > 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.GreaterThan,
                    Left: Combination
                    {
                        CombinationType: CombinationType.DivideRoundDownwards,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceDivideRoundDownOverLessThan : Template
        {
            public PrecedenceDivideRoundDownOverLessThan() : base("1 / 2 < 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.LessThan,
                    Left: Combination
                    {
                        CombinationType: CombinationType.DivideRoundDownwards,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceDivideRoundDownOverGreaterThanOrEqual : Template
        {
            public PrecedenceDivideRoundDownOverGreaterThanOrEqual() : base("1 / 2 >= 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.GreaterThanOrEqual,
                    Left: Combination
                    {
                        CombinationType: CombinationType.DivideRoundDownwards,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceDivideRoundDownOverLessThanOrEqual : Template
        {
            public PrecedenceDivideRoundDownOverLessThanOrEqual() : base("1 / 2 <= 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.LessThanOrEqual,
                    Left: Combination
                    {
                        CombinationType: CombinationType.DivideRoundDownwards,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceDivideRoundDownOverEqual : Template
        {
            public PrecedenceDivideRoundDownOverEqual() : base("1 / 2 = 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.Equal,
                    Left: Combination
                    {
                        CombinationType: CombinationType.DivideRoundDownwards,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceDivideRoundDownOverNotEqual : Template
        {
            public PrecedenceDivideRoundDownOverNotEqual() : base("1 / 2 != 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.NotEqual,
                    Left: Combination
                    {
                        CombinationType: CombinationType.DivideRoundDownwards,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

    #endregion

    #region Divide round up

        private sealed class PrecedenceDivideRoundUpOverAdd : Template
        {
            public PrecedenceDivideRoundUpOverAdd() : base("1 // 2 + 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.Add,
                    Left: Combination
                    {
                        CombinationType: CombinationType.DivideRoundUpwards,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceDivideRoundUpOverSubtract : Template
        {
            public PrecedenceDivideRoundUpOverSubtract() : base("1 // 2 - 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.Subtract,
                    Left: Combination
                    {
                        CombinationType: CombinationType.DivideRoundUpwards,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceDivideRoundUpOverNegation : Template
        {
            public PrecedenceDivideRoundUpOverNegation() : base("1 // - 2") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.DivideRoundUpwards,
                    Left: NumericConstant { Value: 1 },
                    Right: Negation
                    {
                        Source: NumericConstant { Value: 2 }
                    }
                };
        }

        private sealed class PrecedenceDivideRoundUpOverMultiply : Template
        {
            public PrecedenceDivideRoundUpOverMultiply() : base("1 // 2 * 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.Multiply,
                    Left: Combination
                    {
                        CombinationType: CombinationType.DivideRoundUpwards,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceDivideRoundUpOverDivideRoundDown : Template
        {
            public PrecedenceDivideRoundUpOverDivideRoundDown() : base("1 // 2 / 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.DivideRoundDownwards,
                    Left: Combination
                    {
                        CombinationType: CombinationType.DivideRoundUpwards,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceDivideRoundUpOverDivideRoundUp : Template
        {
            public PrecedenceDivideRoundUpOverDivideRoundUp() : base("1 // 2 // 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.DivideRoundUpwards,
                    Left: Combination
                    {
                        CombinationType: CombinationType.DivideRoundUpwards,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceDivideRoundUpOverGreaterThan : Template
        {
            public PrecedenceDivideRoundUpOverGreaterThan() : base("1 // 2 > 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.GreaterThan,
                    Left: Combination
                    {
                        CombinationType: CombinationType.DivideRoundUpwards,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceDivideRoundUpOverLessThan : Template
        {
            public PrecedenceDivideRoundUpOverLessThan() : base("1 // 2 < 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.LessThan,
                    Left: Combination
                    {
                        CombinationType: CombinationType.DivideRoundUpwards,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceDivideRoundUpOverGreaterThanOrEqual : Template
        {
            public PrecedenceDivideRoundUpOverGreaterThanOrEqual() : base("1 // 2 >= 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.GreaterThanOrEqual,
                    Left: Combination
                    {
                        CombinationType: CombinationType.DivideRoundUpwards,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceDivideRoundUpOverLessThanOrEqual : Template
        {
            public PrecedenceDivideRoundUpOverLessThanOrEqual() : base("1 // 2 <= 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.LessThanOrEqual,
                    Left: Combination
                    {
                        CombinationType: CombinationType.DivideRoundUpwards,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceDivideRoundUpOverEqual : Template
        {
            public PrecedenceDivideRoundUpOverEqual() : base("1 // 2 = 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.Equal,
                    Left: Combination
                    {
                        CombinationType: CombinationType.DivideRoundUpwards,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

        private sealed class PrecedenceDivideRoundUpOverNotEqual : Template
        {
            public PrecedenceDivideRoundUpOverNotEqual() : base("1 // 2 != 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.NotEqual,
                    Left: Combination
                    {
                        CombinationType: CombinationType.DivideRoundUpwards,
                        Left: NumericConstant { Value: 1 },
                        Right: NumericConstant { Value: 2 }
                    },
                    Right: NumericConstant { Value: 3 }
                };
        }

    #endregion

    #region Negate

        private sealed class PrecedenceNegateOverAdd : Template
        {
            public PrecedenceNegateOverAdd() : base("- 1 + 2") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.Add,
                    Left: Negation
                    {
                        Source: NumericConstant { Value: 1 }
                    },
                    Right: NumericConstant { Value: 2 }
                };
        }

        private sealed class PrecedenceNegateOverSubtract : Template
        {
            public PrecedenceNegateOverSubtract() : base("- 1 - 2") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.Subtract,
                    Left: Negation
                    {
                        Source: NumericConstant { Value: 1 }
                    },
                    Right: NumericConstant { Value: 2 }
                };
        }

        private sealed class PrecedenceNegateOverMultiply : Template
        {
            public PrecedenceNegateOverMultiply() : base("- 1 * 2") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.Multiply,
                    Left: Negation
                    {
                        Source: NumericConstant { Value: 1 }
                    },
                    Right: NumericConstant { Value: 2 }
                };
        }

        private sealed class PrecedenceNegateOverDivideRoundDown : Template
        {
            public PrecedenceNegateOverDivideRoundDown() : base("- 1 / 2") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.DivideRoundDownwards,
                    Left: Negation
                    {
                        Source: NumericConstant { Value: 1 }
                    },
                    Right: NumericConstant { Value: 2 }
                };
        }

        private sealed class PrecedenceNegateOverDivideRoundUp : Template
        {
            public PrecedenceNegateOverDivideRoundUp() : base("- 1 // 2") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is Combination
                {
                    CombinationType: CombinationType.DivideRoundUpwards,
                    Left: Negation
                    {
                        Source: NumericConstant { Value: 1 }
                    },
                    Right: NumericConstant { Value: 2 }
                };
        }

        private sealed class PrecedenceNegateOverGreaterThan : Template
        {
            public PrecedenceNegateOverGreaterThan() : base("- 1 > 2") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.GreaterThan,
                    Left: Negation
                    {
                        Source: NumericConstant { Value: 1 }
                    },
                    Right: NumericConstant { Value: 2 }
                };
        }

        private sealed class PrecedenceNegateOverLessThan : Template
        {
            public PrecedenceNegateOverLessThan() : base("- 1 < 2") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.LessThan,
                    Left: Negation
                    {
                        Source: NumericConstant { Value: 1 }
                    },
                    Right: NumericConstant { Value: 2 }
                };
        }

        private sealed class PrecedenceNegateOverGreaterThanOrEqual : Template
        {
            public PrecedenceNegateOverGreaterThanOrEqual() : base("- 1 >= 2") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.GreaterThanOrEqual,
                    Left: Negation
                    {
                        Source: NumericConstant { Value: 1 }
                    },
                    Right: NumericConstant { Value: 2 }
                };
        }

        private sealed class PrecedenceNegateOverLessThanOrEqual : Template
        {
            public PrecedenceNegateOverLessThanOrEqual() : base("- 1 <= 2") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.LessThanOrEqual,
                    Left: Negation
                    {
                        Source: NumericConstant { Value: 1 }
                    },
                    Right: NumericConstant { Value: 2 }
                };
        }

        private sealed class PrecedenceNegateOverEqual : Template
        {
            public PrecedenceNegateOverEqual() : base("- 1 = 2") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.Equal,
                    Left: Negation
                    {
                        Source: NumericConstant { Value: 1 }
                    },
                    Right: NumericConstant { Value: 2 }
                };
        }

        private sealed class PrecedenceNegateOverNotEqual : Template
        {
            public PrecedenceNegateOverNotEqual() : base("- 1 != 2") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.NotEqual,
                    Left: Negation
                    {
                        Source: NumericConstant { Value: 1 }
                    },
                    Right: NumericConstant { Value: 2 }
                };
        }

    #endregion
        
        // todo next 4
        
    #region Highest

        

    #endregion

    #region Lowest

        

    #endregion

    #region Summation

        

    #endregion

    #region Range

        

    #endregion

    #region Greater than

        private sealed class PrecedenceGreaterThanOverAdd : Template
        {
            public PrecedenceGreaterThanOverAdd() : base("1 > 2 + 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.GreaterThan,
                    Left: NumericConstant { Value: 1 },
                    Right: Combination
                    {
                        CombinationType: CombinationType.Add,
                        Left: NumericConstant { Value: 2 },
                        Right: NumericConstant { Value: 3 }
                    }
                };
        }
        
        private sealed class PrecedenceGreaterThanOverSubtract : Template
        {
            public PrecedenceGreaterThanOverSubtract() : base("1 > 2 - 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.GreaterThan,
                    Left: NumericConstant { Value: 1 },
                    Right: Combination
                    {
                        CombinationType: CombinationType.Subtract,
                        Left: NumericConstant { Value: 2 },
                        Right: NumericConstant { Value: 3 }
                    }
                };
        }

        private sealed class PrecedenceGreaterThanOverNegation : Template
        {
            public PrecedenceGreaterThanOverNegation() : base("1 > - 2") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.GreaterThan,
                    Left: NumericConstant { Value: 1 },
                    Right: Negation
                    {
                        Source: NumericConstant { Value: 2 }
                    }
                };
        }

        private sealed class PrecedenceGreaterThanOverMultiply : Template
        {
            public PrecedenceGreaterThanOverMultiply() : base("1 > 2 * 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.GreaterThan,
                    Left: NumericConstant { Value: 1 },
                    Right: Combination
                    {
                        CombinationType: CombinationType.Multiply,
                        Left: NumericConstant { Value: 2 },
                        Right: NumericConstant { Value: 3 }
                    }
                };
        }

        private sealed class PrecedenceGreaterThanOverDivideRoundDown : Template
        {
            public PrecedenceGreaterThanOverDivideRoundDown() : base("1 > 2 / 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.GreaterThan,
                    Left: NumericConstant { Value: 1 },
                    Right: Combination
                    {
                        CombinationType: CombinationType.DivideRoundDownwards,
                        Left: NumericConstant { Value: 2 },
                        Right: NumericConstant { Value: 3 }
                    }
                };
        }

        private sealed class PrecedenceGreaterThanOverDivideRoundUp : Template
        {
            public PrecedenceGreaterThanOverDivideRoundUp() : base("1 > 2 // 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.GreaterThan,
                    Left: NumericConstant { Value: 1 },
                    Right: Combination
                    {
                        CombinationType: CombinationType.DivideRoundUpwards,
                        Left: NumericConstant { Value: 2 },
                        Right: NumericConstant { Value: 3 }
                    }
                };
        }

        private sealed class PrecedenceGreaterThanOverEqual : Template
        {
            public PrecedenceGreaterThanOverEqual() : base("1 > 2 = true") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryAssertion
                {
                    AssertionType: BinaryAssertionType.Equal,
                    Left: OperationAsAssertion
                    {
                        Source: DefaultBinaryOperation
                        {
                            OperationType: OperationType.GreaterThan,
                            Left: NumericConstant { Value: 1 },
                            Right: NumericConstant { Value: 2 }
                        }
                    },
                    Right: BinaryConstant { Value: true }
                };
        }

        private sealed class PrecedenceGreaterThanOverNotEqual : Template
        {
            public PrecedenceGreaterThanOverNotEqual() : base("1 > 2 != true") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryAssertion
                {
                    AssertionType: BinaryAssertionType.NotEqual,
                    Left: OperationAsAssertion
                    {
                        Source: DefaultBinaryOperation
                        {
                            OperationType: OperationType.GreaterThan,
                            Left: NumericConstant { Value: 1 },
                            Right: NumericConstant { Value: 2 }
                        }
                    },
                    Right: BinaryConstant { Value: true }
                };
        }

    #endregion
        
    #region Greater than or equal

        private sealed class PrecedenceGreaterThanOrEqualOverAdd : Template
        {
            public PrecedenceGreaterThanOrEqualOverAdd() : base("1 >= 2 + 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.GreaterThanOrEqual,
                    Left: NumericConstant { Value: 1 },
                    Right: Combination
                    {
                        CombinationType: CombinationType.Add,
                        Left: NumericConstant { Value: 2 },
                        Right: NumericConstant { Value: 3 }
                    }
                };
        }
        
        private sealed class PrecedenceGreaterThanOrEqualOverSubtract : Template
        {
            public PrecedenceGreaterThanOrEqualOverSubtract() : base("1 >= 2 - 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.GreaterThanOrEqual,
                    Left: NumericConstant { Value: 1 },
                    Right: Combination
                    {
                        CombinationType: CombinationType.Subtract,
                        Left: NumericConstant { Value: 2 },
                        Right: NumericConstant { Value: 3 }
                    }
                };
        }

        private sealed class PrecedenceGreaterThanOrEqualOverNegation : Template
        {
            public PrecedenceGreaterThanOrEqualOverNegation() : base("1 >= - 2") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.GreaterThanOrEqual,
                    Left: NumericConstant { Value: 1 },
                    Right: Negation
                    {
                        Source: NumericConstant { Value: 2 }
                    }
                };
        }

        private sealed class PrecedenceGreaterThanOrEqualOverMultiply : Template
        {
            public PrecedenceGreaterThanOrEqualOverMultiply() : base("1 >= 2 * 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.GreaterThanOrEqual,
                    Left: NumericConstant { Value: 1 },
                    Right: Combination
                    {
                        CombinationType: CombinationType.Multiply,
                        Left: NumericConstant { Value: 2 },
                        Right: NumericConstant { Value: 3 }
                    }
                };
        }

        private sealed class PrecedenceGreaterThanOrEqualOverDivideRoundDown : Template
        {
            public PrecedenceGreaterThanOrEqualOverDivideRoundDown() : base("1 >= 2 / 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.GreaterThanOrEqual,
                    Left: NumericConstant { Value: 1 },
                    Right: Combination
                    {
                        CombinationType: CombinationType.DivideRoundDownwards,
                        Left: NumericConstant { Value: 2 },
                        Right: NumericConstant { Value: 3 }
                    }
                };
        }

        private sealed class PrecedenceGreaterThanOrEqualOverDivideRoundUp : Template
        {
            public PrecedenceGreaterThanOrEqualOverDivideRoundUp() : base("1 >= 2 // 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.GreaterThanOrEqual,
                    Left: NumericConstant { Value: 1 },
                    Right: Combination
                    {
                        CombinationType: CombinationType.DivideRoundUpwards,
                        Left: NumericConstant { Value: 2 },
                        Right: NumericConstant { Value: 3 }
                    }
                };
        }

        private sealed class PrecedenceGreaterThanOrEqualOverEqual : Template
        {
            public PrecedenceGreaterThanOrEqualOverEqual() : base("1 >= 2 = true") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryAssertion
                {
                    AssertionType: BinaryAssertionType.Equal,
                    Left: OperationAsAssertion
                    {
                        Source: DefaultBinaryOperation
                        {
                            OperationType: OperationType.GreaterThanOrEqual,
                            Left: NumericConstant { Value: 1 },
                            Right: NumericConstant { Value: 2 }
                        }
                    },
                    Right: BinaryConstant { Value: true }
                };
        }

        private sealed class PrecedenceGreaterThanOrEqualOverNotEqual : Template
        {
            public PrecedenceGreaterThanOrEqualOverNotEqual() : base("1 >= 2 != true") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryAssertion
                {
                    AssertionType: BinaryAssertionType.NotEqual,
                    Left: OperationAsAssertion
                    {
                        Source: DefaultBinaryOperation
                        {
                            OperationType: OperationType.GreaterThanOrEqual,
                            Left: NumericConstant { Value: 1 },
                            Right: NumericConstant { Value: 2 }
                        }
                    },
                    Right: BinaryConstant { Value: true }
                };
        }

    #endregion
        
    #region Less than

        private sealed class PrecedenceLessThanOverAdd : Template
        {
            public PrecedenceLessThanOverAdd() : base("1 < 2 + 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.LessThan,
                    Left: NumericConstant { Value: 1 },
                    Right: Combination
                    {
                        CombinationType: CombinationType.Add,
                        Left: NumericConstant { Value: 2 },
                        Right: NumericConstant { Value: 3 }
                    }
                };
        }
        
        private sealed class PrecedenceLessThanOverSubtract : Template
        {
            public PrecedenceLessThanOverSubtract() : base("1 < 2 - 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.LessThan,
                    Left: NumericConstant { Value: 1 },
                    Right: Combination
                    {
                        CombinationType: CombinationType.Subtract,
                        Left: NumericConstant { Value: 2 },
                        Right: NumericConstant { Value: 3 }
                    }
                };
        }

        private sealed class PrecedenceLessThanOverNegation : Template
        {
            public PrecedenceLessThanOverNegation() : base("1 < - 2") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.LessThan,
                    Left: NumericConstant { Value: 1 },
                    Right: Negation
                    {
                        Source: NumericConstant { Value: 2 }
                    }
                };
        }

        private sealed class PrecedenceLessThanOverMultiply : Template
        {
            public PrecedenceLessThanOverMultiply() : base("1 < 2 * 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.LessThan,
                    Left: NumericConstant { Value: 1 },
                    Right: Combination
                    {
                        CombinationType: CombinationType.Multiply,
                        Left: NumericConstant { Value: 2 },
                        Right: NumericConstant { Value: 3 }
                    }
                };
        }

        private sealed class PrecedenceLessThanOverDivideRoundDown : Template
        {
            public PrecedenceLessThanOverDivideRoundDown() : base("1 < 2 / 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.LessThan,
                    Left: NumericConstant { Value: 1 },
                    Right: Combination
                    {
                        CombinationType: CombinationType.DivideRoundDownwards,
                        Left: NumericConstant { Value: 2 },
                        Right: NumericConstant { Value: 3 }
                    }
                };
        }

        private sealed class PrecedenceLessThanOverDivideRoundUp : Template
        {
            public PrecedenceLessThanOverDivideRoundUp() : base("1 < 2 // 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.LessThan,
                    Left: NumericConstant { Value: 1 },
                    Right: Combination
                    {
                        CombinationType: CombinationType.DivideRoundUpwards,
                        Left: NumericConstant { Value: 2 },
                        Right: NumericConstant { Value: 3 }
                    }
                };
        }

        private sealed class PrecedenceLessThanOverEqual : Template
        {
            public PrecedenceLessThanOverEqual() : base("1 < 2 = true") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryAssertion
                {
                    AssertionType: BinaryAssertionType.Equal,
                    Left: OperationAsAssertion
                    {
                        Source: DefaultBinaryOperation
                        {
                            OperationType: OperationType.LessThan,
                            Left: NumericConstant { Value: 1 },
                            Right: NumericConstant { Value: 2 }
                        }
                    },
                    Right: BinaryConstant { Value: true }
                };
        }

        private sealed class PrecedenceLessThanOverNotEqual : Template
        {
            public PrecedenceLessThanOverNotEqual() : base("1 < 2 != true") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryAssertion
                {
                    AssertionType: BinaryAssertionType.NotEqual,
                    Left: OperationAsAssertion
                    {
                        Source: DefaultBinaryOperation
                        {
                            OperationType: OperationType.LessThan,
                            Left: NumericConstant { Value: 1 },
                            Right: NumericConstant { Value: 2 }
                        }
                    },
                    Right: BinaryConstant { Value: true }
                };
        }

    #endregion
        
    #region Less than or equal

        private sealed class PrecedenceLessThanOrEqualOverAdd : Template
        {
            public PrecedenceLessThanOrEqualOverAdd() : base("1 <= 2 + 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.LessThanOrEqual,
                    Left: NumericConstant { Value: 1 },
                    Right: Combination
                    {
                        CombinationType: CombinationType.Add,
                        Left: NumericConstant { Value: 2 },
                        Right: NumericConstant { Value: 3 }
                    }
                };
        }
        
        private sealed class PrecedenceLessThanOrEqualOverSubtract : Template
        {
            public PrecedenceLessThanOrEqualOverSubtract() : base("1 <= 2 - 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.LessThanOrEqual,
                    Left: NumericConstant { Value: 1 },
                    Right: Combination
                    {
                        CombinationType: CombinationType.Subtract,
                        Left: NumericConstant { Value: 2 },
                        Right: NumericConstant { Value: 3 }
                    }
                };
        }

        private sealed class PrecedenceLessThanOrEqualOverNegation : Template
        {
            public PrecedenceLessThanOrEqualOverNegation() : base("1 <= - 2") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.LessThanOrEqual,
                    Left: NumericConstant { Value: 1 },
                    Right: Negation
                    {
                        Source: NumericConstant { Value: 2 }
                    }
                };
        }

        private sealed class PrecedenceLessThanOrEqualOverMultiply : Template
        {
            public PrecedenceLessThanOrEqualOverMultiply() : base("1 <= 2 * 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.LessThanOrEqual,
                    Left: NumericConstant { Value: 1 },
                    Right: Combination
                    {
                        CombinationType: CombinationType.Multiply,
                        Left: NumericConstant { Value: 2 },
                        Right: NumericConstant { Value: 3 }
                    }
                };
        }

        private sealed class PrecedenceLessThanOrEqualOverDivideRoundDown : Template
        {
            public PrecedenceLessThanOrEqualOverDivideRoundDown() : base("1 <= 2 / 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.LessThanOrEqual,
                    Left: NumericConstant { Value: 1 },
                    Right: Combination
                    {
                        CombinationType: CombinationType.DivideRoundDownwards,
                        Left: NumericConstant { Value: 2 },
                        Right: NumericConstant { Value: 3 }
                    }
                };
        }

        private sealed class PrecedenceLessThanOrEqualOverDivideRoundUp : Template
        {
            public PrecedenceLessThanOrEqualOverDivideRoundUp() : base("1 <= 2 // 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.LessThanOrEqual,
                    Left: NumericConstant { Value: 1 },
                    Right: Combination
                    {
                        CombinationType: CombinationType.DivideRoundUpwards,
                        Left: NumericConstant { Value: 2 },
                        Right: NumericConstant { Value: 3 }
                    }
                };
        }

        private sealed class PrecedenceLessThanOrEqualOverEqual : Template
        {
            public PrecedenceLessThanOrEqualOverEqual() : base("1 <= 2 = true") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryAssertion
                {
                    AssertionType: BinaryAssertionType.Equal,
                    Left: OperationAsAssertion
                    {
                        Source: DefaultBinaryOperation
                        {
                            OperationType: OperationType.LessThanOrEqual,
                            Left: NumericConstant { Value: 1 },
                            Right: NumericConstant { Value: 2 }
                        }
                    },
                    Right: BinaryConstant { Value: true }
                };
        }

        private sealed class PrecedenceLessThanOrEqualOverNotEqual : Template
        {
            public PrecedenceLessThanOrEqualOverNotEqual() : base("1 <= 2 != true") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryAssertion
                {
                    AssertionType: BinaryAssertionType.NotEqual,
                    Left: OperationAsAssertion
                    {
                        Source: DefaultBinaryOperation
                        {
                            OperationType: OperationType.LessThanOrEqual,
                            Left: NumericConstant { Value: 1 },
                            Right: NumericConstant { Value: 2 }
                        }
                    },
                    Right: BinaryConstant { Value: true }
                };
        }

    #endregion
        
    #region Equal

        private sealed class PrecedenceEqualOverAdd : Template
        {
            public PrecedenceEqualOverAdd() : base("1 = 2 + 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.Equal,
                    Left: NumericConstant { Value: 1 },
                    Right: Combination
                    {
                        CombinationType: CombinationType.Add,
                        Left: NumericConstant { Value: 2 },
                        Right: NumericConstant { Value: 3 }
                    }
                };
        }
        
        private sealed class PrecedenceEqualOverSubtract : Template
        {
            public PrecedenceEqualOverSubtract() : base("1 = 2 - 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.Equal,
                    Left: NumericConstant { Value: 1 },
                    Right: Combination
                    {
                        CombinationType: CombinationType.Subtract,
                        Left: NumericConstant { Value: 2 },
                        Right: NumericConstant { Value: 3 }
                    }
                };
        }

        private sealed class PrecedenceEqualOverNegation : Template
        {
            public PrecedenceEqualOverNegation() : base("1 = - 2") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.Equal,
                    Left: NumericConstant { Value: 1 },
                    Right: Negation
                    {
                        Source: NumericConstant { Value: 2 }
                    }
                };
        }

        private sealed class PrecedenceEqualOverMultiply : Template
        {
            public PrecedenceEqualOverMultiply() : base("1 = 2 * 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.Equal,
                    Left: NumericConstant { Value: 1 },
                    Right: Combination
                    {
                        CombinationType: CombinationType.Multiply,
                        Left: NumericConstant { Value: 2 },
                        Right: NumericConstant { Value: 3 }
                    }
                };
        }

        private sealed class PrecedenceEqualOverDivideRoundDown : Template
        {
            public PrecedenceEqualOverDivideRoundDown() : base("1 = 2 / 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.Equal,
                    Left: NumericConstant { Value: 1 },
                    Right: Combination
                    {
                        CombinationType: CombinationType.DivideRoundDownwards,
                        Left: NumericConstant { Value: 2 },
                        Right: NumericConstant { Value: 3 }
                    }
                };
        }

        private sealed class PrecedenceEqualOverDivideRoundUp : Template
        {
            public PrecedenceEqualOverDivideRoundUp() : base("1 = 2 // 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.Equal,
                    Left: NumericConstant { Value: 1 },
                    Right: Combination
                    {
                        CombinationType: CombinationType.DivideRoundUpwards,
                        Left: NumericConstant { Value: 2 },
                        Right: NumericConstant { Value: 3 }
                    }
                };
        }

        private sealed class PrecedenceEqualOverGreaterThan : Template
        {
            public PrecedenceEqualOverGreaterThan() : base("true = 1 > 2") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryAssertion
                {
                    AssertionType: BinaryAssertionType.Equal,
                    Left: BinaryConstant { Value: true },
                    Right: OperationAsAssertion
                    {
                        Source: DefaultBinaryOperation
                        {
                            OperationType: OperationType.GreaterThan,
                            Left: NumericConstant { Value: 1 },
                            Right: NumericConstant { Value: 2 }
                        }
                    }
                };
        }

        private sealed class PrecedenceEqualOverGreaterThanOrEqual : Template
        {
            public PrecedenceEqualOverGreaterThanOrEqual() : base("true = 1 >= 2") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryAssertion
                {
                    AssertionType: BinaryAssertionType.Equal,
                    Left: BinaryConstant { Value: true },
                    Right: OperationAsAssertion
                    {
                        Source: DefaultBinaryOperation
                        {
                            OperationType: OperationType.GreaterThanOrEqual,
                            Left: NumericConstant { Value: 1 },
                            Right: NumericConstant { Value: 2 }
                        }
                    }
                };
        }

        private sealed class PrecedenceEqualOverLessThan : Template
        {
            public PrecedenceEqualOverLessThan() : base("true = 1 < 2") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryAssertion
                {
                    AssertionType: BinaryAssertionType.Equal,
                    Left: BinaryConstant { Value: true },
                    Right: OperationAsAssertion
                    {
                        Source: DefaultBinaryOperation
                        {
                            OperationType: OperationType.LessThan,
                            Left: NumericConstant { Value: 1 },
                            Right: NumericConstant { Value: 2 }
                        }
                    }
                };
        }

        private sealed class PrecedenceEqualOverLessThanOrEqual : Template
        {
            public PrecedenceEqualOverLessThanOrEqual() : base("true = 1 <= 2") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryAssertion
                {
                    AssertionType: BinaryAssertionType.Equal,
                    Left: BinaryConstant { Value: true },
                    Right: OperationAsAssertion
                    {
                        Source: DefaultBinaryOperation
                        {
                            OperationType: OperationType.LessThanOrEqual,
                            Left: NumericConstant { Value: 1 },
                            Right: NumericConstant { Value: 2 }
                        }
                    }
                };
        }

        private sealed class PrecedenceEqualOverEqual : Template
        {
            public PrecedenceEqualOverEqual() : base("true = 1 = 2") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryAssertion
                {
                    AssertionType: BinaryAssertionType.Equal,
                    Left: BinaryConstant { Value: true },
                    Right: OperationAsAssertion
                    {
                        Source: DefaultBinaryOperation
                        {
                            OperationType: OperationType.Equal,
                            Left: NumericConstant { Value: 1 },
                            Right: NumericConstant { Value: 2 }
                        }
                    }
                };
        }

        private sealed class PrecedenceEqualOverNotEqual : Template
        {
            public PrecedenceEqualOverNotEqual() : base("true = 1 != 2") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryAssertion
                {
                    AssertionType: BinaryAssertionType.Equal,
                    Left: BinaryConstant { Value: true },
                    Right: OperationAsAssertion
                    {
                        Source: DefaultBinaryOperation
                        {
                            OperationType: OperationType.NotEqual,
                            Left: NumericConstant { Value: 1 },
                            Right: NumericConstant { Value: 2 }
                        }
                    }
                };
        }

    #endregion
        
    #region Not equal

        private sealed class PrecedenceNotEqualOverAdd : Template
        {
            public PrecedenceNotEqualOverAdd() : base("1 != 2 + 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.NotEqual,
                    Left: NumericConstant { Value: 1 },
                    Right: Combination
                    {
                        CombinationType: CombinationType.Add,
                        Left: NumericConstant { Value: 2 },
                        Right: NumericConstant { Value: 3 }
                    }
                };
        }
        
        private sealed class PrecedenceNotEqualOverSubtract : Template
        {
            public PrecedenceNotEqualOverSubtract() : base("1 != 2 - 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.NotEqual,
                    Left: NumericConstant { Value: 1 },
                    Right: Combination
                    {
                        CombinationType: CombinationType.Subtract,
                        Left: NumericConstant { Value: 2 },
                        Right: NumericConstant { Value: 3 }
                    }
                };
        }

        private sealed class PrecedenceNotEqualOverNegation : Template
        {
            public PrecedenceNotEqualOverNegation() : base("1 != - 2") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.NotEqual,
                    Left: NumericConstant { Value: 1 },
                    Right: Negation
                    {
                        Source: NumericConstant { Value: 2 }
                    }
                };
        }

        private sealed class PrecedenceNotEqualOverMultiply : Template
        {
            public PrecedenceNotEqualOverMultiply() : base("1 != 2 * 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.NotEqual,
                    Left: NumericConstant { Value: 1 },
                    Right: Combination
                    {
                        CombinationType: CombinationType.Multiply,
                        Left: NumericConstant { Value: 2 },
                        Right: NumericConstant { Value: 3 }
                    }
                };
        }

        private sealed class PrecedenceNotEqualOverDivideRoundDown : Template
        {
            public PrecedenceNotEqualOverDivideRoundDown() : base("1 != 2 / 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.NotEqual,
                    Left: NumericConstant { Value: 1 },
                    Right: Combination
                    {
                        CombinationType: CombinationType.DivideRoundDownwards,
                        Left: NumericConstant { Value: 2 },
                        Right: NumericConstant { Value: 3 }
                    }
                };
        }

        private sealed class PrecedenceNotEqualOverDivideRoundUp : Template
        {
            public PrecedenceNotEqualOverDivideRoundUp() : base("1 != 2 // 3") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryOperation
                {
                    OperationType: OperationType.NotEqual,
                    Left: NumericConstant { Value: 1 },
                    Right: Combination
                    {
                        CombinationType: CombinationType.DivideRoundUpwards,
                        Left: NumericConstant { Value: 2 },
                        Right: NumericConstant { Value: 3 }
                    }
                };
        }

        private sealed class PrecedenceNotEqualOverGreaterThan : Template
        {
            public PrecedenceNotEqualOverGreaterThan() : base("true != 1 > 2") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryAssertion
                {
                    AssertionType: BinaryAssertionType.NotEqual,
                    Left: BinaryConstant { Value: true },
                    Right: OperationAsAssertion
                    {
                        Source: DefaultBinaryOperation
                        {
                            OperationType: OperationType.GreaterThan,
                            Left: NumericConstant { Value: 1 },
                            Right: NumericConstant { Value: 2 }
                        }
                    }
                };
        }

        private sealed class PrecedenceNotEqualOverGreaterThanOrEqual : Template
        {
            public PrecedenceNotEqualOverGreaterThanOrEqual() : base("true != 1 >= 2") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryAssertion
                {
                    AssertionType: BinaryAssertionType.NotEqual,
                    Left: BinaryConstant { Value: true },
                    Right: OperationAsAssertion
                    {
                        Source: DefaultBinaryOperation
                        {
                            OperationType: OperationType.GreaterThanOrEqual,
                            Left: NumericConstant { Value: 1 },
                            Right: NumericConstant { Value: 2 }
                        }
                    }
                };
        }

        private sealed class PrecedenceNotEqualOverLessThan : Template
        {
            public PrecedenceNotEqualOverLessThan() : base("true != 1 < 2") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryAssertion
                {
                    AssertionType: BinaryAssertionType.NotEqual,
                    Left: BinaryConstant { Value: true },
                    Right: OperationAsAssertion
                    {
                        Source: DefaultBinaryOperation
                        {
                            OperationType: OperationType.LessThan,
                            Left: NumericConstant { Value: 1 },
                            Right: NumericConstant { Value: 2 }
                        }
                    }
                };
        }

        private sealed class PrecedenceNotEqualOverLessThanOrEqual : Template
        {
            public PrecedenceNotEqualOverLessThanOrEqual() : base("true != 1 <= 2") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryAssertion
                {
                    AssertionType: BinaryAssertionType.NotEqual,
                    Left: BinaryConstant { Value: true },
                    Right: OperationAsAssertion
                    {
                        Source: DefaultBinaryOperation
                        {
                            OperationType: OperationType.LessThanOrEqual,
                            Left: NumericConstant { Value: 1 },
                            Right: NumericConstant { Value: 2 }
                        }
                    }
                };
        }

        private sealed class PrecedenceNotEqualOverEqual : Template
        {
            public PrecedenceNotEqualOverEqual() : base("true != 1 = 2") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryAssertion
                {
                    AssertionType: BinaryAssertionType.NotEqual,
                    Left: BinaryConstant { Value: true },
                    Right: OperationAsAssertion
                    {
                        Source: DefaultBinaryOperation
                        {
                            OperationType: OperationType.Equal,
                            Left: NumericConstant { Value: 1 },
                            Right: NumericConstant { Value: 2 }
                        }
                    }
                };
        }

        private sealed class PrecedenceNotEqualOverNotEqual : Template
        {
            public PrecedenceNotEqualOverNotEqual() : base("true != 1 != 2") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryAssertion
                {
                    AssertionType: BinaryAssertionType.NotEqual,
                    Left: BinaryConstant { Value: true },
                    Right: OperationAsAssertion
                    {
                        Source: DefaultBinaryOperation
                        {
                            OperationType: OperationType.NotEqual,
                            Left: NumericConstant { Value: 1 },
                            Right: NumericConstant { Value: 2 }
                        }
                    }
                };
        }

    #endregion
        
    #region And

        private sealed class PrecedenceAndOverGreaterThan : Template
        {
            public PrecedenceAndOverGreaterThan() : base("true and 1 > 2") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryAssertion
                {
                    AssertionType: BinaryAssertionType.And,
                    Left: BinaryConstant { Value: true },
                    Right: OperationAsAssertion
                    {
                        Source: DefaultBinaryOperation
                        {
                            OperationType: OperationType.GreaterThan,
                            Left: NumericConstant { Value: 1 },
                            Right: NumericConstant { Value: 2 }
                        }
                    }
                };
        }
        
        private sealed class PrecedenceAndOverGreaterThanOrEqual : Template
        {
            public PrecedenceAndOverGreaterThanOrEqual() : base("true and 1 >= 2") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryAssertion
                {
                    AssertionType: BinaryAssertionType.And,
                    Left: BinaryConstant { Value: true },
                    Right: OperationAsAssertion
                    {
                        Source: DefaultBinaryOperation
                        {
                            OperationType: OperationType.GreaterThanOrEqual,
                            Left: NumericConstant { Value: 1 },
                            Right: NumericConstant { Value: 2 }
                        }
                    }
                };
        }
        
        private sealed class PrecedenceAndOverLessThan : Template
        {
            public PrecedenceAndOverLessThan() : base("true and 1 < 2") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryAssertion
                {
                    AssertionType: BinaryAssertionType.And,
                    Left: BinaryConstant { Value: true },
                    Right: OperationAsAssertion
                    {
                        Source: DefaultBinaryOperation
                        {
                            OperationType: OperationType.LessThan,
                            Left: NumericConstant { Value: 1 },
                            Right: NumericConstant { Value: 2 }
                        }
                    }
                };
        }
        
        private sealed class PrecedenceAndOverLessThanOrEqual : Template
        {
            public PrecedenceAndOverLessThanOrEqual() : base("true and 1 <= 2") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryAssertion
                {
                    AssertionType: BinaryAssertionType.And,
                    Left: BinaryConstant { Value: true },
                    Right: OperationAsAssertion
                    {
                        Source: DefaultBinaryOperation
                        {
                            OperationType: OperationType.LessThanOrEqual,
                            Left: NumericConstant { Value: 1 },
                            Right: NumericConstant { Value: 2 }
                        }
                    }
                };
        }
        
        private sealed class PrecedenceAndOverEqual : Template
        {
            public PrecedenceAndOverEqual() : base("true and 1 = 2") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryAssertion
                {
                    AssertionType: BinaryAssertionType.And,
                    Left: BinaryConstant { Value: true },
                    Right: OperationAsAssertion
                    {
                        Source: DefaultBinaryOperation
                        {
                            OperationType: OperationType.Equal,
                            Left: NumericConstant { Value: 1 },
                            Right: NumericConstant { Value: 2 }
                        }
                    }
                };
        }
        
        private sealed class PrecedenceAndOverNotEqual : Template
        {
            public PrecedenceAndOverNotEqual() : base("true and 1 != 2") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryAssertion
                {
                    AssertionType: BinaryAssertionType.And,
                    Left: BinaryConstant { Value: true },
                    Right: OperationAsAssertion
                    {
                        Source: DefaultBinaryOperation
                        {
                            OperationType: OperationType.NotEqual,
                            Left: NumericConstant { Value: 1 },
                            Right: NumericConstant { Value: 2 }
                        }
                    }
                };
        }
        
        private sealed class PrecedenceAndOverAnd : Template
        {
            public PrecedenceAndOverAnd() : base("true and false and true") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryAssertion
                {
                    AssertionType: BinaryAssertionType.And,
                    Left: DefaultBinaryAssertion
                    {
                        AssertionType: BinaryAssertionType.And,
                        Left: BinaryConstant { Value: true },
                        Right: BinaryConstant { Value: false }
                    },
                    Right: BinaryConstant { Value: true }
                };
        }
        
        private sealed class PrecedenceAndOverOr : Template
        {
            public PrecedenceAndOverOr() : base("true and false or true") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryAssertion
                {
                    AssertionType: BinaryAssertionType.Or,
                    Left: DefaultBinaryAssertion
                    {
                        AssertionType: BinaryAssertionType.And,
                        Left: BinaryConstant { Value: true },
                        Right: BinaryConstant { Value: false }
                    },
                    Right: BinaryConstant { Value: true }
                };
        }
        
        private sealed class PrecedenceAndOverNot : Template
        {
            public PrecedenceAndOverNot() : base("true and not true") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryAssertion
                {
                    AssertionType: BinaryAssertionType.And,
                    Left: BinaryConstant { Value: true },
                    Right: NotAssertion
                    {
                        Source: BinaryConstant { Value: true }
                    }
                };
        }

    #endregion
        
    #region Or

        private sealed class PrecedenceOrOverGreaterThan : Template
        {
            public PrecedenceOrOverGreaterThan() : base("true or 1 > 2") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryAssertion
                {
                    AssertionType: BinaryAssertionType.Or,
                    Left: BinaryConstant { Value: true },
                    Right: OperationAsAssertion
                    {
                        Source: DefaultBinaryOperation
                        {
                            OperationType: OperationType.GreaterThan,
                            Left: NumericConstant { Value: 1 },
                            Right: NumericConstant { Value: 2 }
                        }
                    }
                };
        }
        
        private sealed class PrecedenceOrOverGreaterThanOrEqual : Template
        {
            public PrecedenceOrOverGreaterThanOrEqual() : base("true or 1 >= 2") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryAssertion
                {
                    AssertionType: BinaryAssertionType.Or,
                    Left: BinaryConstant { Value: true },
                    Right: OperationAsAssertion
                    {
                        Source: DefaultBinaryOperation
                        {
                            OperationType: OperationType.GreaterThanOrEqual,
                            Left: NumericConstant { Value: 1 },
                            Right: NumericConstant { Value: 2 }
                        }
                    }
                };
        }
        
        private sealed class PrecedenceOrOverLessThan : Template
        {
            public PrecedenceOrOverLessThan() : base("true or 1 < 2") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryAssertion
                {
                    AssertionType: BinaryAssertionType.Or,
                    Left: BinaryConstant { Value: true },
                    Right: OperationAsAssertion
                    {
                        Source: DefaultBinaryOperation
                        {
                            OperationType: OperationType.LessThan,
                            Left: NumericConstant { Value: 1 },
                            Right: NumericConstant { Value: 2 }
                        }
                    }
                };
        }
        
        private sealed class PrecedenceOrOverLessThanOrEqual : Template
        {
            public PrecedenceOrOverLessThanOrEqual() : base("true or 1 <= 2") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryAssertion
                {
                    AssertionType: BinaryAssertionType.Or,
                    Left: BinaryConstant { Value: true },
                    Right: OperationAsAssertion
                    {
                        Source: DefaultBinaryOperation
                        {
                            OperationType: OperationType.LessThanOrEqual,
                            Left: NumericConstant { Value: 1 },
                            Right: NumericConstant { Value: 2 }
                        }
                    }
                };
        }
        
        private sealed class PrecedenceOrOverEqual : Template
        {
            public PrecedenceOrOverEqual() : base("true or 1 = 2") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryAssertion
                {
                    AssertionType: BinaryAssertionType.Or,
                    Left: BinaryConstant { Value: true },
                    Right: OperationAsAssertion
                    {
                        Source: DefaultBinaryOperation
                        {
                            OperationType: OperationType.Equal,
                            Left: NumericConstant { Value: 1 },
                            Right: NumericConstant { Value: 2 }
                        }
                    }
                };
        }
        
        private sealed class PrecedenceOrOverNotEqual : Template
        {
            public PrecedenceOrOverNotEqual() : base("true or 1 != 2") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryAssertion
                {
                    AssertionType: BinaryAssertionType.Or,
                    Left: BinaryConstant { Value: true },
                    Right: OperationAsAssertion
                    {
                        Source: DefaultBinaryOperation
                        {
                            OperationType: OperationType.NotEqual,
                            Left: NumericConstant { Value: 1 },
                            Right: NumericConstant { Value: 2 }
                        }
                    }
                };
        }
        
        private sealed class PrecedenceOrOverAnd : Template
        {
            public PrecedenceOrOverAnd() : base("true or false and true") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryAssertion
                {
                    AssertionType: BinaryAssertionType.And,
                    Left: DefaultBinaryAssertion
                    {
                        AssertionType: BinaryAssertionType.Or,
                        Left: BinaryConstant { Value: true },
                        Right: BinaryConstant { Value: false }
                    },
                    Right: BinaryConstant { Value: true }
                };
        }
        
        private sealed class PrecedenceOrOverOr : Template
        {
            public PrecedenceOrOverOr() : base("true or false or true") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryAssertion
                {
                    AssertionType: BinaryAssertionType.Or,
                    Left: DefaultBinaryAssertion
                    {
                        AssertionType: BinaryAssertionType.Or,
                        Left: BinaryConstant { Value: true },
                        Right: BinaryConstant { Value: false }
                    },
                    Right: BinaryConstant { Value: true }
                };
        }
        
        private sealed class PrecedenceOrOverNot : Template
        {
            public PrecedenceOrOverNot() : base("true or not true") { }

            protected override bool MeetsExpectedPattern(INode node) =>
                node is DefaultBinaryAssertion
                {
                    AssertionType: BinaryAssertionType.Or,
                    Left: BinaryConstant { Value: true },
                    Right: NotAssertion
                    {
                        Source: BinaryConstant { Value: true }
                    }
                };
        }

    #endregion

    #endregion
    }
}
