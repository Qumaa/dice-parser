using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DiceRoll.Input
{
    public sealed class RegexToken : IToken
    {
        private readonly Regex[] _patterns;

        public RegexToken(params Regex[] patterns)
        {
            _patterns = patterns;
        }

        public RegexToken(IEnumerable<Regex> patterns) : this(patterns.ToArray()) { }

        public RegexToken(Regex pattern) : this(new [] {pattern}) { }

        public bool Matches(in Substring input, out Substring substring)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            // ReSharper disable once ForCanBeConvertedToForeach
            for (int i = 0; i < _patterns.Length; i++)
                if (Matches(input, _patterns[i], out substring))
                    return true;

            substring = default;
            return false;
        }

        // todo: don't check whether the substring starts with the token; do it outside
        private static bool Matches(Substring input, Regex pattern, out Substring matchSubstring)
        {
            int firstNonWhitespaceIndex = GetFirstNonWhitespaceIndex(input);
            Regex.ValueMatchEnumerator enumerator = pattern.EnumerateMatches(input.AsSpan(), firstNonWhitespaceIndex);

            if (!enumerator.MoveNext())
            {
                matchSubstring = default;
                return false;
            }

            ValueMatch match = enumerator.Current;

            if (match.Index != firstNonWhitespaceIndex)
            {
                matchSubstring = default;
                return false;
            }
            
            matchSubstring = new Substring(input, match.Index, match.Length);
            return true;
        }

        private static int GetFirstNonWhitespaceIndex(Substring token)
        {
            int whitespaces = 0;

            foreach (char c in token)
            {
                if (!char.IsWhiteSpace(c))
                    break;

                whitespaces++;
            }

            return whitespaces;
        }

        public static RegexToken ExactIgnoreCase(string word) =>
            new(CreateExactIgnoreCaseRegex(word));

        public static RegexToken ExactIgnoreCase(IEnumerable<string> words) =>
            new(words.Select(static x => CreateExactIgnoreCaseRegex(x)));
        
        public static RegexToken ExactIgnoreCase(params string[] words) =>
            ExactIgnoreCase(words as IEnumerable<string>);

        public static Regex CreateExactIgnoreCaseRegex(string word) =>
            new(Regex.Escape(word),
                RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);
    }
}
