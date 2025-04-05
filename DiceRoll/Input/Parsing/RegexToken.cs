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

        public bool Matches(ReadOnlySpan<char> token, out MatchInfo matchInfo)
        {
            token = token.Trim();
            
            // ReSharper disable once LoopCanBeConvertedToQuery
            // ReSharper disable once ForCanBeConvertedToForeach
            for (int i = 0; i < _patterns.Length; i++)
                if (Matches(token, _patterns[i], out matchInfo))
                    return true;

            matchInfo = default;
            return false;
        }

        private static bool Matches(ReadOnlySpan<char> token, Regex pattern, out MatchInfo matchInfo)
        {
            Regex.ValueMatchEnumerator enumerator = pattern.EnumerateMatches(token);

            if (!enumerator.MoveNext())
            {
                matchInfo = default;
                return false;
            }

            ValueMatch match = enumerator.Current;

            if (match.Index is not 0)
            {
                matchInfo = default;
                return false;
            }
            
            matchInfo = new MatchInfo(token, match.Index, match.Length);
            return true;
        }

        public IEnumerable<string> EnumerateRawTokens() =>
            _patterns.Select(static x => x.ToString());

        public static RegexToken ExactIgnoreCase(string word) =>
            new(CreateExactIgnoreCaseRegex(word));

        public static RegexToken ExactIgnoreCase(IEnumerable<string> words) =>
            new(words.Select(static x => CreateExactIgnoreCaseRegex(x)));
        
        public static RegexToken ExactIgnoreCase(params string[] words) =>
            ExactIgnoreCase(words as IEnumerable<string>);

        public static Regex CreateExactIgnoreCaseRegex(string word) =>
            new(Regex.Escape(word), RegexOptions.IgnoreCase);
    }
}
