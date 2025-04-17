using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DiceRoll.Input.Parsing
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

        public bool Matches(in Substring input, out Substring match)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            // ReSharper disable once ForCanBeConvertedToForeach
            for (int i = 0; i < _patterns.Length; i++)
                if (Matches(in input, _patterns[i], out match))
                    return true;

            match = default;
            return false;
        }

        private static bool Matches(in Substring input, Regex pattern, out Substring matchSubstring)
        {
            Regex.ValueMatchEnumerator enumerator = pattern.EnumerateMatches(input.AsSpan());

            if (!enumerator.MoveNext())
            {
                matchSubstring = default;
                return false;
            }

            ValueMatch match = enumerator.Current;
            
            matchSubstring = new Substring(input, match.Index, match.Length);
            return true;
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
