using System.Collections.Generic;
using System.Linq;

namespace DiceRoll
{
    internal static class Syntax
    {
        public static IEnumerable<T> SingleEnumerable<T>(T value) =>
            Params(value);

        public static T[] Params<T>(params T[] args) =>
            args;

        public static T[] ToArray<T>(IEnumerable<T> enumerable) =>
            enumerable as T[] ?? enumerable.ToArray();
    }
}
