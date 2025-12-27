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

        public static void Push<T>(this LexemesList lexemes, in Mapped<T> lexeme) where T : Lexeme =>
            lexemes.Push(lexeme.Value, in lexeme.Range);
        
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
        
        public static void Insert<T>(this LexemesList lexemes, Index index, in Mapped<T> lexeme) where T : Lexeme =>
            lexemes.Insert(index, lexeme.Value, in lexeme.Range);
        
        public static void Insert<T>(this LexemesList lexemes, int index, in Mapped<T> lexeme) where T : Lexeme =>
            lexemes.Insert(index, lexeme.Value, in lexeme.Range);

        public static Mapped<Lexeme>[] Take(this LexemesList lexemes, in Range range) =>
            lexemes.TakeTypedOrThrow<Lexeme>(in range);
        
        public static Mapped<T>[] TakeTypedOrThrow<T>(this LexemesList lexemes, in Range range) where T : Lexeme
        {
            (int start, int end) = range.GetStartAndEnd(lexemes.Count);

            int takenCount = end - start;
            Mapped<T>[] taken = new Mapped<T>[takenCount];

            for (int i = end - 1; i >= start; i--)
            {
                Mapped<T> lexeme = lexemes.TakeTypedOrThrow<T>(i);
                taken[i - start] = lexeme;
            }

            return taken;
        }

        public static Mapped<Lexeme>[] Get(this LexemesList lexemes, in Range range) =>
            lexemes.GetTypedOrThrow<Lexeme>(in range);
        
        public static Mapped<T>[] GetTypedOrThrow<T>(this LexemesList lexemes, in Range range) where T : Lexeme
        {
            (int start, int end) = range.GetStartAndEnd(lexemes.Count);

            int getCount = end - start;
            Mapped<T>[] taken = new Mapped<T>[getCount];

            for (int i = end - 1; i >= start; i--)
            {
                Mapped<T> lexeme = lexemes.GetTypedOrThrow<T>(i);
                taken[i - start] = lexeme;
            }

            return taken;
        }

        public static bool TryGetTyped<T>(this LexemesList lexemes, int index, out Mapped<T> result) where T : Lexeme =>
            lexemes.Get(index).TryCastValue(out result);

        public static Mapped<T> GetTypedOrThrow<T>(this LexemesList lexemes, int index) where T : Lexeme =>
            lexemes.Get(index).CastValueOrThrow<Lexeme, T>();
        
        public static bool TryTakeTyped<T>(this LexemesList lexemes, int index, out Mapped<T> result) where T : Lexeme =>
            lexemes.Take(index).TryCastValue(out result);

        public static Mapped<T> TakeTypedOrThrow<T>(this LexemesList lexemes, int index) where T : Lexeme =>
            lexemes.Take(index).CastValueOrThrow<Lexeme, T>();

        public static void Remove(this LexemesList lexemes, int index) =>
            lexemes.Take(index);

        public static void Remove(this LexemesList lexemes, Index index) =>
            lexemes.Remove(index.GetOffset(lexemes.Count));

        public static void Remove(this LexemesList lexemes, in Range range)
        {
            (int start, int length) = range.GetOffsetAndLength(lexemes.Count);

            for (int i = 0; i < length; i++)
                lexemes.Remove(start);
        }

        public static void Replace(this LexemesList lexemes, int index, Lexeme value, int start, int length)
        {
            lexemes.Remove(index);
            lexemes.Insert(index, value, start, length);
        }

        public static void Replace(this LexemesList lexemes, Index index, Lexeme value, int start, int length) =>
            lexemes.Replace(index.GetOffset(lexemes.Count), value, start, length);

        public static void Replace(this LexemesList lexemes, in Range range, Lexeme value, int start, int length)
        {
            (int rangeStart, int rangeLength) = range.GetOffsetAndLength(lexemes.Count);

            if (rangeLength is 1)
            {
                lexemes.Replace(rangeStart, value, start, length);
                return;
            }
            
            lexemes.Remove(in range);
            lexemes.Insert(rangeStart, value, start, length);
        }
        
        
        public static void Replace(this LexemesList lexemes, int index, Lexeme value, in Substring substring) =>
            lexemes.Replace(index, value, substring.Start, substring.Length);
        
        public static void Replace(this LexemesList lexemes, Index index, Lexeme value, in Substring substring) =>
            lexemes.Replace(index, value, substring.Start, substring.Length);
        
        public static void Replace(this LexemesList lexemes, in Range range, Lexeme value, in Substring substring) =>
            lexemes.Replace(in range, value, substring.Start, substring.Length);
        
        public static void Replace(this LexemesList lexemes, int index, Lexeme value, in Range range)
        {
            (int start, int length) = range.GetOffsetAndLength(lexemes.InputLength);
            
            lexemes.Replace(index, value, start, length);
        }

        public static void Replace(this LexemesList lexemes, Index index, Lexeme value, in Range range)
        {
            (int start, int length) = range.GetOffsetAndLength(lexemes.InputLength);
            
            lexemes.Replace(index, value, start, length);
        }

        public static void Replace(this LexemesList lexemes, in Range range, Lexeme value, in Range mappingRange)
        {
            (int start, int length) = mappingRange.GetOffsetAndLength(lexemes.InputLength);
            
            lexemes.Replace(in range, value, start, length);
        }

        public static void Replace<T>(this LexemesList lexemes, in Range range, in Mapped<T> lexeme) where T : Lexeme
        {
            (int start, int length) = lexeme.Range.GetOffsetAndLength(lexemes.InputLength);
            
            lexemes.Replace(in range, lexeme.Value, start, length);
        }
    }
}
