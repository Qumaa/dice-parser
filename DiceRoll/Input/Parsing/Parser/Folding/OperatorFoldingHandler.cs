using System;
using System.Collections.Generic;
using System.Linq;

namespace DiceRoll.Input.Parsing
{
    public sealed class OperatorFoldingHandler
    {
        private readonly OperandCastingTable _castingTable;
        
        public OperatorFoldingHandler(OperandCastingTable castingTable)
        {
            ArgumentNullException.ThrowIfNull(castingTable);
            
            _castingTable = castingTable;
        }

        public Range FoldOperators(LexemesList lexemes, in Range range)
        {
            Indexer indexer = IndexOperators(lexemes, in range);
            OperatorPrecedences precedences = IndexPrecedences(indexer);
            
            if (indexer.Count is 0)
                return range;

            (int start, int end) = range.GetStartAndEnd(lexemes.Count);

            foreach (int precedence in precedences)
                end -= ExecuteAllOperatorsAtPrecedence(indexer, precedence);

            return start..end;
        }

        private int ExecuteAllOperatorsAtPrecedence(Indexer indexer, int precedence)
        {
            int lexemesReduced = 0;
            
            start:
            for (int i = 0; i < indexer.Count; i++)
            {
                if (!TryExecuteOperatorWithImmediateContext(indexer, i, precedence, out int reduced))
                    continue;

                indexer.DelistOperatorAt(i);
                lexemesReduced += reduced;
                goto start; // todo instead of restarting the loop, manually check all nearby operators
            }

            return lexemesReduced;
        }

        private bool TryExecuteOperatorWithImmediateContext(Indexer indexer, int i, int precedence, out int lexemesReduced)
        {
            if (!IsPreferableWithinImmediateContext(indexer, i, precedence, out InvocationInfo invocationInfo))
            {
                lexemesReduced = 0;
                return false;
            }

            InvokeUsingImmediateContext(indexer, i, invocationInfo);

            lexemesReduced = invocationInfo.Invoker.Arity;
            return true;
        }

        private bool IsPreferableWithinImmediateContext(Indexer indexer, int i, int precedence, out InvocationInfo invocationInfo)
        {
            IndexedOperator indexedOperator = indexer.GetOperator(i).Value;

            foreach (Overload overload in indexedOperator.SortedOverloads)
            {
                if (!overload.ToRange(indexedOperator.Index).FitsIn(indexer.Limit, indexer.Source.Count))
                    continue;
                
                if (!IsInvokableWithImmediateContext(indexer.Source, indexedOperator.Index, overload))
                    continue;

                if (overload.Precedence != precedence)
                    goto fail;

                invocationInfo = new InvocationInfo(overload.Invoker, overload.CopyOperandsBuffer());
                return true;
            }

            fail:
            invocationInfo = default;
            return false;
        }

        private bool IsInvokableWithImmediateContext(LexemesList list, int position, Overload overload)
        {
            OperatorInvoker invoker = overload.Invoker;
            
            int offset = position - invoker.Arity.Left;
            
            for (int i = 0; i < invoker.Arity; i++)
            {
                int index = offset + i; // current operand index

                if (index >= position) // skipping operator index
                    index++;
                
                if (!list.TryGetTyped(index, out Mapped<Operand> operand))
                    return false;
                
                Type expectedType = invoker.Signature.OperandTypes[i];

                if (!MatchesExpectedType(ref operand, expectedType))
                    return false;
                
                overload.OperandsBuffer[i] = operand;
            }

            return true;
        }

        private bool MatchesExpectedType(ref Mapped<Operand> operand, Type expectedType)
        {
            Type evaluationType = operand.Value.EvaluationType;
            
            if (evaluationType.IsAssignableTo(expectedType))
                return true;

            if (!_castingTable.IsCasterDefined(evaluationType, expectedType, out OperandCaster caster))
                return false;

            INode castedValue = caster.CastOrThrow(operand.Value.Node);
            Operand castedOperand = new(castedValue, expectedType, operand.Value.Parents);
            operand = operand.WithValue(castedOperand);
            return true;
        }

        private static void InvokeUsingImmediateContext(Indexer indexer, int i, InvocationInfo invocationInfo)
        {
            OperatorInvoker invoker = invocationInfo.Invoker;
            Mapped<IndexedOperator> @operator = indexer.GetOperator(i);
            int position = @operator.Value.Index;

            int leftMostPosition = position - invoker.Arity.Left;
            Range usedRange = leftMostPosition..(position + invoker.Arity.Right + 1);

            indexer.Source.TakeMany(in usedRange);
            Operand operand = Invoke(in invocationInfo);

            indexer.Source.Insert(leftMostPosition, operand, in @operator.Range);
        }

        private static Operand Invoke(in InvocationInfo info)
        {
            Signature signature = info.Invoker.Signature;
            
            OperandsAccess access = new(info.Operands, signature);
            
            INode invocationResult = info.Invoker.Invoke(access);

            return new Operand(invocationResult, signature.ReturnType, info.Operands);
        }

        private static Indexer IndexOperators(LexemesList lexemes, in Range range) =>
            Indexer.FromLexemesList(lexemes, in range);

        private static OperatorPrecedences IndexPrecedences(Indexer indexer)
        {
            OperatorPrecedences precedences = new();

            for (int i = 0; i < indexer.Count; i++)
            {
                IndexedOperator @operator = indexer.GetOperator(i).Value;

                foreach (Overload overload in @operator.SortedOverloads)
                    precedences.Add(overload.Precedence);
            }

            return precedences;
        }

        private sealed class Indexer
        {
            public readonly LexemesList Source;
            public readonly Range Limit;
            private readonly List<Mapped<IndexedOperator>> _indexedOperators;

            public int Count => _indexedOperators.Count;

            public Indexer(LexemesList source, in Range limit, IEnumerable<Mapped<IndexedOperator>> operators)
            {
                Source = source;
                Limit = limit;
                _indexedOperators = operators.ToList();
            }

            public Mapped<IndexedOperator> GetOperator(int index) =>
                _indexedOperators[index];

            public void DelistOperatorAt(int index) =>
                _indexedOperators.RemoveAt(index);

            public static Indexer FromLexemesList(LexemesList list, in Range range)
            {
                (int start, int length) = range.GetOffsetAndLength(list.Count);
                int end = start + length;

                List<Mapped<IndexedOperator>> operators = new(length);

                for (int i = start; i < end; i++)
                {
                    Mapped<Lexeme> lexeme = list.Get(i);
                    
                    if (!lexeme.TryCastValue(out Mapped<Operator> @operator))
                        continue;

                    IndexedOperator indexed = new(i, @operator.Value.Definitions);
                    Mapped<IndexedOperator> mapped = new(indexed, in lexeme.Range);
                    operators.Add(mapped);
                }

                return new Indexer(list, in range, operators);
            }
        }

        private sealed class IndexedOperator
        {
            public readonly int Index;
            public readonly Overload[] SortedOverloads;
            
            public IndexedOperator(int index, IEnumerable<OperatorDefinition> definitions)
            {
                Index = index;
                SortedOverloads = Sort(definitions);
            }

            private static Overload[] Sort(IEnumerable<OperatorDefinition> definitions) =>
                definitions
                    .SelectMany(x => x.InvocationBehaviour.Invokers.Select(y => (definition: x, invoker: y)))
                    .OrderByDescending(x => x.invoker.Arity)
                    .ThenByDescending(x => x.definition.Precedence)
                    .Select(x => new Overload(x.definition.Precedence, x.invoker, x.definition.Associativity))
                    .ToArray();
        }

        private sealed class Overload
        {
            public readonly int Precedence;
            public readonly Associativity Associativity;
            public readonly OperatorInvoker Invoker;
            public readonly Mapped<Operand>[] OperandsBuffer;
            
            public Overload(int precedence, OperatorInvoker invoker, Associativity associativity)
            {
                Precedence = precedence;
                Invoker = invoker;
                Associativity = associativity;
                
                OperandsBuffer = new Mapped<Operand>[invoker.Arity];
            }

            public Mapped<Operand>[] CopyOperandsBuffer()
            {
                Mapped<Operand>[] copy = new Mapped<Operand>[Invoker.Arity];
                
                Array.Copy(OperandsBuffer, copy, Invoker.Arity);

                return copy;
            }

            public Range ToRange(int position) =>
                (position - Invoker.Arity.Left)..(position + 1 + Invoker.Arity.Right);
        }
    }
}
