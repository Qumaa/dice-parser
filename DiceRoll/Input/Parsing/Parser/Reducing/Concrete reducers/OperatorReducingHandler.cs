using System;
using System.Collections.Generic;
using System.Linq;

namespace DiceRoll.Input.Parsing
{
    public sealed class OperatorReducingHandler : LexemesReducingHandler
    {
        private readonly OperandCastingTable _castingTable;
        
        public OperatorReducingHandler(OperandCastingTable castingTable)
        {
            ArgumentNullException.ThrowIfNull(castingTable);
            
            _castingTable = castingTable;
        }

        public override Range Reduce(LexemesList lexemes, in Range range, ReducerCursor cursor)
        {
            Indexer indexer = IndexOperators(lexemes, in range);
            OperatorPrecedences precedences = IndexPrecedences(indexer);
            
            if (indexer.Count is 0)
                return range;

            foreach (PrecedenceLevel precedence in precedences)
                ExecuteAllOperatorsAtPrecedence(indexer, precedence, cursor);

            return indexer.Range;
        }

        private void ExecuteAllOperatorsAtPrecedence(Indexer indexer, PrecedenceLevel precedence, ReducerCursor cursor)
        {
            // todo instead of restarting the loop, manually check all nearby operators
            start:
            if (precedence.HasAssociativity(Associativity.Right))
                for (int i = indexer.Count - 1; i >= 0; i--)
                    if (TryExecuteOperatorWithImmediateContext(indexer, i, in precedence, cursor))
                        goto start; 
            
            if (precedence.HasAssociativity(Associativity.Left))
                for (int i = 0; i < indexer.Count; i++)
                    if (TryExecuteOperatorWithImmediateContext(indexer, i, in precedence, cursor))
                        goto start; 
        }

        private bool TryExecuteOperatorWithImmediateContext(Indexer indexer, int i, in PrecedenceLevel precedence,
            ReducerCursor cursor)
        {
            if (!IsPreferableWithinImmediateContext(indexer, i, precedence, out InvocationInfo invocationInfo))
                return false;
            
            InvokeUsingImmediateContext(indexer, i, invocationInfo, cursor);
            return true;
        }

        private bool IsPreferableWithinImmediateContext(Indexer indexer, int i, in PrecedenceLevel precedence,
            out InvocationInfo invocationInfo)
        {
            IndexedOperator indexedOperator = indexer.GetOperator(i).Value;

            foreach (Overload overload in indexedOperator.SortedOverloads)
            {
                if (!overload.FitsIn(indexedOperator.Index, indexer.Range, indexer.Source.Count))
                    continue;
                
                if (!IsInvokableWithImmediateContext(indexer.Source, indexedOperator.Index, overload))
                    continue;

                if (overload.Precedence != precedence.Value || !precedence.HasAssociativity(overload.Associativity))
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

        private static void InvokeUsingImmediateContext(Indexer indexer, int i, InvocationInfo invocationInfo,
            ReducerCursor cursor)
        {
            OperatorInvoker invoker = invocationInfo.Invoker;
            Mapped<IndexedOperator> @operator = indexer.GetOperator(i);
            int position = @operator.Value.Index;
            
            cursor.MoveTo(in @operator.Range);

            Range usedRange = invoker.Arity.ToRange(position);

            Operand operand = Invoke(in invocationInfo);

            indexer.DelistOperatorAt(i, invoker.Arity);

            indexer.Source.Replace(in usedRange, operand, in @operator.Range);
            
            cursor.MoveToPrevious();
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
                    precedences.Add(overload.Precedence, overload.Associativity);
            }

            return precedences;
        }

        private sealed class Indexer
        {
            public readonly LexemesList Source;
            private readonly List<Mapped<IndexedOperator>> _indexedOperators;
            private Range _range;

            public int Count => _indexedOperators.Count;

            public Range Range => _range;

            public Indexer(LexemesList source, in Range range, IEnumerable<Mapped<IndexedOperator>> operators)
            {
                Source = source;
                _range = range;
                _indexedOperators = operators.ToList();
            }

            public Mapped<IndexedOperator> GetOperator(int index) =>
                _indexedOperators[index];

            public void DelistOperatorAt(int index, int lexemesReduced)
            {
                _indexedOperators.RemoveAt(index);
                
                for (int i = index; i < Count; i++)
                    _indexedOperators[i].Value.InsetIndex(lexemesReduced);
                
                ReduceRange(lexemesReduced);
            }

            private void ReduceRange(int reduced)
            {
                (int start, int end) = _range.GetStartAndEnd(Source.Count);

                _range = start..(end - reduced);
            }

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
            public readonly Overload[] SortedOverloads;

            public int Index { get; private set; }
            
            public IndexedOperator(int index, IEnumerable<OperatorDefinition> definitions)
            {
                Index = index;
                SortedOverloads = Sort(definitions);
            }

            private static Overload[] Sort(IEnumerable<OperatorDefinition> definitions) =>
                definitions
                    .SelectMany(x => x.InvocationBehaviour.Invokers.Select(y => (definition: x, invoker: y)))
                    .OrderByDescending(x => x.invoker.Arity)
                    .ThenByDescending(x => x.definition.InvocationBehaviour.Precedence)
                    .Select(x => new Overload(
                            x.definition.InvocationBehaviour.Precedence,
                            x.invoker,
                            x.definition.InvocationBehaviour.Associativity
                            )
                        )
                    .ToArray();

            public void InsetIndex(int inset) =>
                Index -= inset;
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
            
            public bool FitsIn(int position, in Range range, int length)
            {
                (int start, int end) = range.GetStartAndEnd(length);
                Arity arity = Invoker.Arity;

                return position - arity.Left >= start && position + arity.Right < end;
            }
        }
    }
}
