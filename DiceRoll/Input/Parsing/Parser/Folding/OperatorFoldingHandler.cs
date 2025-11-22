using System;
using System.Collections.Generic;
using System.Linq;

namespace DiceRoll.Input.Parsing
{
    public sealed class OperatorFoldingHandler
    {
        public Range FoldOperators(LexemesList lexemes, in Range range)
        {
            Indexer indexer = IndexOperators(lexemes, in range);
            OperatorPrecedences precedences = IndexPrecedences(indexer);
            
            if (indexer.Count is 0)
                return range;

            (int start, int length) = range.GetOffsetAndLength(lexemes.Count);

            foreach (int precedence in precedences)
                length -= ExecuteAllOperatorsAtPrecedence(indexer, precedence);

            return start..(start + length);
        }

        private int ExecuteAllOperatorsAtPrecedence(Indexer indexer, int precedence)
        {
            int lexemesReduced = 0;
            
            start:
            for (int i = 0; i < indexer.Count; i++)
            {
                if (!TryExecuteOperatorWithImmediateContext(indexer, i, precedence, out lexemesReduced))
                    continue;

                indexer.DelistOperatorAt(i);
                goto start;
            }

            return lexemesReduced; //todo
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
                if (!IsInvokableWithImmediateContext(indexer.Source, indexedOperator.Index, overload.Invoker, out Mapped<Operand>[] operands))
                    continue;

                if (overload.Precedence != precedence)
                    goto fail;

                invocationInfo = new InvocationInfo(overload.Invoker, operands);
                return true;
            }

            fail:
            invocationInfo = default;
            return false;
        }

        private bool IsInvokableWithImmediateContext(LexemesList list, int position, OperatorInvoker invoker,
            out Mapped<Operand>[] operands)
        {
            operands = new Mapped<Operand>[invoker.Arity];

            for (int i = 0; i < invoker.LeftArity; i++)
            {
                int index = position - invoker.LeftArity + i;

                if (!list.TryGetTyped(index, out Mapped<Operand> operand))
                    return false;

                Type expectedType = invoker.Signature.GetOperandTypes()[i];

                if (operand.Value.EvaluationType != expectedType)
                    return false;

                operands[i] = operand;
            }
            
            for (int i = 0; i < invoker.RightArity; i++)
            {
                int index = position + 1 + i;

                if (!list.TryGetTyped(index, out Mapped<Operand> operand))
                    return false;

                Type expectedType = invoker.Signature.GetOperandTypes()[invoker.LeftArity + i];

                if (operand.Value.EvaluationType != expectedType)
                    return false;

                operands[invoker.LeftArity + i] = operand;
            }

            return true;
        }

        private void InvokeUsingImmediateContext(Indexer indexer, int i, InvocationInfo invocationInfo)
        {
            OperatorInvoker invoker = invocationInfo.Invoker;
            Mapped<IndexedOperator> @operator = indexer.GetOperator(i);
            int position = @operator.Value.Index;

            int leftMostPosition = position - invoker.LeftArity;
            Range usedRange = leftMostPosition..(position + invoker.RightArity + 1);

            indexer.Source.TakeMany(in usedRange);
            Operand operand = Invoke(in invocationInfo);

            indexer.Source.Insert(leftMostPosition, operand, in @operator.Range);
        }

        private static Operand Invoke(in InvocationInfo info)
        {
            Signature signature = info.Invoker.Signature;
            
            OperandsAccess access = new(info.Operands, signature);
            
            INode invocationResult = info.Invoker.Invoke(access);

            return new Operand(invocationResult, signature.GetReturnType(), info.Operands);
        }

        private Indexer IndexOperators(LexemesList lexemes, in Range range) =>
            Indexer.FromLexemesList(lexemes, in range);

        private OperatorPrecedences IndexPrecedences(Indexer indexer)
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
            private readonly List<Mapped<IndexedOperator>> _indexedOperators;

            public int Count => _indexedOperators.Count;

            public Indexer(LexemesList source, IEnumerable<Mapped<IndexedOperator>> operators)
            {
                Source = source;
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

                return new Indexer(list, operators);
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
            
            public Overload(int precedence, OperatorInvoker invoker, Associativity associativity)
            {
                Precedence = precedence;
                Invoker = invoker;
                Associativity = associativity;
            }
        }
    }
}
