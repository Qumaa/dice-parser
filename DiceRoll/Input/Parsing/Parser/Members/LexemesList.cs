using System;
using System.Collections;
using System.Collections.Generic;

namespace DiceRoll.Input.Parsing
{
    public sealed class LexemesList : IEnumerable<Mapped<Lexeme>>
    {
        private readonly InputMapper _mapper;
        private readonly List<Mapped<Lexeme>> _lexemes;

        public int Count => _lexemes.Count;

        public int InputLength => _mapper.InputLength;
        
        public LexemesList(InputMapper mapper)
        {
            ArgumentNullException.ThrowIfNull(mapper);
            
            _mapper = mapper;
            _lexemes = new List<Mapped<Lexeme>>();
        }

        public void Push(Lexeme lexeme, int start, int length)
        {
            if (lexeme is null)
                return;
            
            _lexemes.Add(_mapper.Map(lexeme, start, length));
        }

        public void Insert(int index, Lexeme lexeme, int start, int length)
        {
            if (lexeme is null)
                return;
            
            if (index == Count)
                Push(lexeme, start, length);
            else
                _lexemes.Insert(index, _mapper.Map(lexeme, start, length));
        }

        public Mapped<Lexeme> Take(int index)
        {
            Mapped<Lexeme> lexeme = Get(index);
            
            _lexemes.RemoveAt(index);

            return lexeme;
        }

        public Mapped<Lexeme> Get(int index) =>
            _lexemes[index];

        public void Clear() =>
            _lexemes.Clear();

        public List<Mapped<Lexeme>>.Enumerator GetEnumerator() =>
            _lexemes.GetEnumerator();

        IEnumerator<Mapped<Lexeme>> IEnumerable<Mapped<Lexeme>>.GetEnumerator() =>
            GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();
    }

    public static class EquationLexemesExtensions
    {
        public static void Push(this LexemesList lexemes, Lexeme lexeme, in Substring substring) =>
            lexemes.Push(lexeme, substring.Start, substring.Length);
            
        public static void Push(this LexemesList lexemes, Lexeme lexeme, in Range range)
        {
            (int start, int length) = range.GetOffsetAndLength(lexemes.Count);
            
            lexemes.Push(lexeme, start, length);
        }
        
        public static void Insert(this LexemesList lexemes, int index, Lexeme lexeme, in Substring substring) =>
            lexemes.Insert(index, lexeme, substring.Start, substring.Length);
        
        public static void Insert(this LexemesList lexemes, Index index, Lexeme lexeme, in Substring substring) =>
            lexemes.Insert(index.GetOffset(lexemes.Count), lexeme, in substring);
            
        public static void Insert(this LexemesList lexemes, int index, Lexeme lexeme, in Range range)
        {
            (int start, int length) = range.GetOffsetAndLength(lexemes.InputLength);
            
            lexemes.Insert(index, lexeme, start, length);
        }

        public static void Insert(this LexemesList lexemes, Index index, Lexeme lexeme, in Range range) =>
            lexemes.Insert(index.GetOffset(lexemes.Count), lexeme, in range);

        public static Mapped<Lexeme>[] TakeMany(this LexemesList lexemes, in Range range)
        {
            int rangeStart = range.Start.GetOffset(lexemes.Count);
            int rangeEnd = range.End.GetOffset(lexemes.Count);

            int replacedCount = rangeEnd - rangeStart;
            Mapped<Lexeme>[] taken = new Mapped<Lexeme>[replacedCount];

            for (int i = rangeEnd - 1; i >= rangeStart; i--)
            {
                Mapped<Lexeme> lexeme = lexemes.Take(i);
                taken[i - rangeStart] = lexeme;
            }

            return taken;
        }

        public static bool TryGetTyped<T>(this LexemesList lexemes, int index, out Mapped<T> result) where T : Lexeme =>
            lexemes.Get(index).TryCastValue(out result);

        public static Mapped<T> GetTypedOrThrow<T>(this LexemesList lexemes, int index) where T : Lexeme =>
            lexemes.Get(index).CastValueOrThrow<Lexeme, T>();

        public static Mapped<Lexeme>[] GetMany(this LexemesList lexemes, in Range range)
        {
            int rangeStart = range.Start.GetOffset(lexemes.Count);
            int rangeEnd = range.End.GetOffset(lexemes.Count);

            int replacedCount = rangeEnd - rangeStart;
            Mapped<Lexeme>[] taken = new Mapped<Lexeme>[replacedCount];

            for (int i = rangeStart; i < rangeEnd; i++)
            {
                Mapped<Lexeme> lexeme = lexemes.Get(i);
                taken[i - rangeStart] = lexeme;
            }

            return taken;
        }
    }
}
