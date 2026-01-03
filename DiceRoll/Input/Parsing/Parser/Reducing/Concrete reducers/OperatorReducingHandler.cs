using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

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

        public override Range Reduce(LexemesList lexemes, in Range range, Cursor cursor)
        {
            Indexer indexer = IndexOperators(lexemes, in range);
            
            if (indexer.Count is 0)
                return range;
            
            OperatorPrecedences precedences = IndexPrecedences(indexer);

            Args args = new(indexer, cursor);
            
            foreach (PrecedenceLevel precedence in precedences)
                ExecuteAllOperatorsAtPrecedence(args, precedence);

            ThrowIfAnyOperatorLeft(args);

            return indexer.Range;
        }

        private void ExecuteAllOperatorsAtPrecedence(Args args, PrecedenceLevel precedence)
        {
            // todo instead of restarting the loop, manually check all nearby operators
            start:
            if (precedence.HasAssociativity(Associativity.Right))
                for (int i = args.Indexer.Count - 1; i >= 0; i--)
                    if (TryExecuteOperatorWithImmediateContext(args, new ScopeArgs(i,  precedence)))
                        goto start; 
            
            if (precedence.HasAssociativity(Associativity.Left))
                for (int i = 0; i < args.Indexer.Count; i++)
                    if (TryExecuteOperatorWithImmediateContext(args, new ScopeArgs(i,  precedence)))
                        goto start; 
        }

        private static void ThrowIfAnyOperatorLeft(Args args)
        {
            if (args.Indexer.Count is 0)
                return;

            ThrowNoMatchingSignature(args.Indexer.GetOperator(0), args.Cursor);
        }

        private static void ThrowNoMatchingSignature(in Mapped<IndexedOperator> @operator, Cursor cursor)
        {
            IEnumerable<OperatorInvocationBehaviour> behaviours =
                @operator.Value.SortedOverloads.Select(x => x.InvocationBehaviour);
            
            cursor.MoveTo(@operator.Range);
            string operatorString = cursor.GetSubstringOfCurrent().ToString();
            
            throw OperatorInvocationException.NoMatchingSignature(behaviours, operatorString);
        }

        private bool TryExecuteOperatorWithImmediateContext(Args args, in ScopeArgs scopeArgs)
        {
            Cursor cursor = args.Cursor;
            Indexer indexer = args.Indexer;
            
            cursor.MoveTo(indexer.GetOperator(scopeArgs.OperatorPointer).Range);
            
            if (!IsInvokableWithinImmediateContext(args, in scopeArgs, out InvocationInfo invocationInfo))
            {
                cursor.MoveToPrevious();
                return false;
            }

            InvokeUsingImmediateContext(args, scopeArgs.OperatorPointer, in invocationInfo);
            cursor.MoveToPrevious();
            return true;
        }

        private bool IsInvokableWithinImmediateContext(Args args, in ScopeArgs scopeArgs, out InvocationInfo invocationInfo)
        {
            Indexer indexer = args.Indexer;
            PrecedenceLevel precedence = scopeArgs.PrecedenceLevel;
            Mapped<IndexedOperator> @operator = indexer.GetOperator(scopeArgs.OperatorPointer);
            IndexedOperator indexedOperator = @operator.Value;
            
            foreach (Overload overload in indexedOperator.SortedOverloads)
            {
                if (overload.Precedence > precedence.Value)
                    continue;
                
                if (!overload.FitsIn(indexedOperator.Index, indexer.Range, indexer.Source.Count))
                    continue;
                
                if (!IsInvokableWithImmediateContext(indexer.Source, indexedOperator.Index, overload))
                    continue;

                // here the very first overload that can be invoked is reached and is considered best
                // if its precedence and associativity equals to expected values, it is invoked
                // otherwise, since the best overload cannot be invoked, so can't be the operator
                // todo this must be flawed
                if (overload.Precedence != precedence.Value || !precedence.HasAssociativity(overload.Associativity))
                    break;

                invocationInfo = new InvocationInfo(overload.Invoker, overload.CopyOperandsBuffer());
                return true;
            }

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

        private static void InvokeUsingImmediateContext(Args args, int operatorPointer, in InvocationInfo invocationInfo)
        {
            Indexer indexer = args.Indexer;
            Cursor cursor = args.Cursor;
            OperatorInvoker invoker = invocationInfo.Invoker;
            Mapped<IndexedOperator> @operator = indexer.GetOperator(operatorPointer);
            int position = @operator.Value.Index;
            
            cursor.MoveTo(in @operator.Range);

            Range usedRange = invoker.Arity.ToRange(position);

            Operand operand = Invoke(in invocationInfo);

            indexer.DelistOperatorAt(operatorPointer, invoker.Arity);

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

            public Indexer(LexemesList source, in Range range, IEnumerable<Mapped<IndexedOperator>> operators) : this(
                source,
                in range,
                operators.ToList()
                ) { }

            private Indexer(LexemesList source, in Range range, List<Mapped<IndexedOperator>> operators)
            {
                Source = source;
                _range = range;
                _indexedOperators = operators;
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
                            x.definition.InvocationBehaviour,
                            x.invoker
                            )
                        )
                    .ToArray();

            public void InsetIndex(int inset) =>
                Index -= inset;
        }

        private sealed class Overload
        {
            public readonly OperatorInvocationBehaviour InvocationBehaviour;
            public readonly OperatorInvoker Invoker;
            public readonly Mapped<Operand>[] OperandsBuffer;
            
            public int Precedence => InvocationBehaviour.Precedence;
            public Associativity Associativity => InvocationBehaviour.Associativity;
            
            public Overload(OperatorInvocationBehaviour invocationBehaviour, OperatorInvoker invoker)
            {
                InvocationBehaviour = invocationBehaviour;
                Invoker = invoker;
                
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

        private sealed class Args
        {
            public readonly Indexer Indexer;
            public readonly Cursor Cursor;
            
            public Args(Indexer indexer, Cursor cursor)
            {
                Indexer = indexer;
                Cursor = cursor;
            }
        }

        [StructLayout(LayoutKind.Auto)]
        private readonly ref struct ScopeArgs
        {
            public readonly int OperatorPointer;
            public readonly PrecedenceLevel PrecedenceLevel;
            
            public ScopeArgs(int operatorPointer, PrecedenceLevel precedenceLevel)
            {
                OperatorPointer = operatorPointer;
                PrecedenceLevel = precedenceLevel;
            }
        }
    }
}
