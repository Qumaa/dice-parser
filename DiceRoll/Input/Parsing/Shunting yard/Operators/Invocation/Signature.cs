using System;
using System.Collections.Generic;

namespace DiceRoll.Input.Parsing
{
    public sealed class Signature
    {
        private readonly Type[] _operandTypes;
        private readonly Type _returnType;

        public int OperandsNumber => _operandTypes.Length;

        public Signature(Type returnType, params Type[] operandTypes)
        {
            CommonException.ThrowIfTypeIsNotNode(returnType);
            CommonException.ThrowIfParamsArrayIsEmpty(operandTypes);
            CommonException.ThrowIfAnyTypeIsNotNode(operandTypes);
            
            _returnType = returnType;
            _operandTypes = operandTypes;
        }

        public ReadOnlySpan<Type> GetOperandTypes() =>
            _operandTypes;

        public IEnumerable<Type> EnumerateOperandTypes() =>
            _operandTypes;

        public Type GetReturnType() =>
            _returnType;

        public static Builder Arguments<T1>() where T1 : INode =>
            new(typeof(T1));

        public static Builder Arguments<T1, T2>() where T1 : INode where T2 : INode =>
            new(typeof(T1), typeof(T2));

        public static Builder Arguments<T1, T2, T3>() where T1 : INode where T2 : INode where T3 : INode =>
            new(typeof(T1), typeof(T2), typeof(T3));

        public static Builder Arguments<T1, T2, T3, T4>()
            where T1 : INode where T2 : INode where T3 : INode where T4 : INode =>
            new(typeof(T1), typeof(T2), typeof(T3), typeof(T4));

        public static Builder Arguments<T1, T2, T3, T4, T5>()
            where T1 : INode where T2 : INode where T3 : INode where T4 : INode where T5 : INode =>
            new(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5));

        public static Builder Arguments<T1, T2, T3, T4, T5, T6>()
            where T1 : INode where T2 : INode where T3 : INode where T4 : INode where T5 : INode where T6 : INode =>
            new(typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6));

        public static Builder Arguments<T1, T2, T3, T4, T5, T6, T7>()
            where T1 : INode
            where T2 : INode
            where T3 : INode
            where T4 : INode
            where T5 : INode
            where T6 : INode
            where T7 : INode =>
            new(
                typeof(T1),
                typeof(T2),
                typeof(T3),
                typeof(T4),
                typeof(T5),
                typeof(T6),
                typeof(T7)
                );

        public static Builder Arguments<T1, T2, T3, T4, T5, T6, T7, T8>()
            where T1 : INode
            where T2 : INode
            where T3 : INode
            where T4 : INode
            where T5 : INode
            where T6 : INode
            where T7 : INode
            where T8 : INode =>
            new(
                typeof(T1),
                typeof(T2),
                typeof(T3),
                typeof(T4),
                typeof(T5),
                typeof(T6),
                typeof(T7),
                typeof(T8)
                );

        public static Builder Arguments<T1, T2, T3, T4, T5, T6, T7, T8, T9>()
            where T1 : INode
            where T2 : INode
            where T3 : INode
            where T4 : INode
            where T5 : INode
            where T6 : INode
            where T7 : INode
            where T8 : INode
            where T9 : INode =>
            new(
                typeof(T1),
                typeof(T2),
                typeof(T3),
                typeof(T4),
                typeof(T5),
                typeof(T6),
                typeof(T7),
                typeof(T8),
                typeof(T9)
                );

        public static Builder Arguments<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>()
            where T1 : INode
            where T2 : INode
            where T3 : INode
            where T4 : INode
            where T5 : INode
            where T6 : INode
            where T7 : INode
            where T8 : INode
            where T9 : INode
            where T10 : INode =>
            new(
                typeof(T1),
                typeof(T2),
                typeof(T3),
                typeof(T4),
                typeof(T5),
                typeof(T6),
                typeof(T7),
                typeof(T8),
                typeof(T9),
                typeof(T10)
                );

        public static Builder Arguments<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>()
            where T1 : INode
            where T2 : INode
            where T3 : INode
            where T4 : INode
            where T5 : INode
            where T6 : INode
            where T7 : INode
            where T8 : INode
            where T9 : INode
            where T10 : INode
            where T11 : INode =>
            new(
                typeof(T1),
                typeof(T2),
                typeof(T3),
                typeof(T4),
                typeof(T5),
                typeof(T6),
                typeof(T7),
                typeof(T8),
                typeof(T9),
                typeof(T10),
                typeof(T11)
                );

        public static Builder Arguments<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>()
            where T1 : INode
            where T2 : INode
            where T3 : INode
            where T4 : INode
            where T5 : INode
            where T6 : INode
            where T7 : INode
            where T8 : INode
            where T9 : INode
            where T10 : INode
            where T11 : INode
            where T12 : INode =>
            new(
                typeof(T1),
                typeof(T2),
                typeof(T3),
                typeof(T4),
                typeof(T5),
                typeof(T6),
                typeof(T7),
                typeof(T8),
                typeof(T9),
                typeof(T10),
                typeof(T11),
                typeof(T12)
                );

        public static Builder Arguments<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>()
            where T1 : INode
            where T2 : INode
            where T3 : INode
            where T4 : INode
            where T5 : INode
            where T6 : INode
            where T7 : INode
            where T8 : INode
            where T9 : INode
            where T10 : INode
            where T11 : INode
            where T12 : INode
            where T13 : INode =>
            new(
                typeof(T1),
                typeof(T2),
                typeof(T3),
                typeof(T4),
                typeof(T5),
                typeof(T6),
                typeof(T7),
                typeof(T8),
                typeof(T9),
                typeof(T10),
                typeof(T11),
                typeof(T12),
                typeof(T13)
                );

        public static Builder Arguments<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>()
            where T1 : INode
            where T2 : INode
            where T3 : INode
            where T4 : INode
            where T5 : INode
            where T6 : INode
            where T7 : INode
            where T8 : INode
            where T9 : INode
            where T10 : INode
            where T11 : INode
            where T12 : INode
            where T13 : INode
            where T14 : INode =>
            new(
                typeof(T1),
                typeof(T2),
                typeof(T3),
                typeof(T4),
                typeof(T5),
                typeof(T6),
                typeof(T7),
                typeof(T8),
                typeof(T9),
                typeof(T10),
                typeof(T11),
                typeof(T12),
                typeof(T13),
                typeof(T14)
                );

        public static Builder Arguments<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>()
            where T1 : INode
            where T2 : INode
            where T3 : INode
            where T4 : INode
            where T5 : INode
            where T6 : INode
            where T7 : INode
            where T8 : INode
            where T9 : INode
            where T10 : INode
            where T11 : INode
            where T12 : INode
            where T13 : INode
            where T14 : INode
            where T15 : INode =>
            new(
                typeof(T1),
                typeof(T2),
                typeof(T3),
                typeof(T4),
                typeof(T5),
                typeof(T6),
                typeof(T7),
                typeof(T8),
                typeof(T9),
                typeof(T10),
                typeof(T11),
                typeof(T12),
                typeof(T13),
                typeof(T14),
                typeof(T15)
                );

        public static Builder Arguments<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>()
            where T1 : INode
            where T2 : INode
            where T3 : INode
            where T4 : INode
            where T5 : INode
            where T6 : INode
            where T7 : INode
            where T8 : INode
            where T9 : INode
            where T10 : INode
            where T11 : INode
            where T12 : INode
            where T13 : INode
            where T14 : INode
            where T15 : INode
            where T16 : INode =>
            new(
                typeof(T1),
                typeof(T2),
                typeof(T3),
                typeof(T4),
                typeof(T5),
                typeof(T6),
                typeof(T7),
                typeof(T8),
                typeof(T9),
                typeof(T10),
                typeof(T11),
                typeof(T12),
                typeof(T13),
                typeof(T14),
                typeof(T15),
                typeof(T16)
                );
        }
    
    public readonly ref struct Builder
    {
        private readonly Type[] _operands;
        
        public Builder(params Type[] operands)
        {
            _operands = operands;
        }
        
        public Signature Returns(Type type) =>
            new(type, _operands);

        public Signature Returns<T>() where T : INode =>
            Returns(typeof(T));
    }
}
