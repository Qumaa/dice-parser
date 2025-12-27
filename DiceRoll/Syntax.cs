using System.Collections.Generic;

namespace DiceRoll
{
    internal static class Syntax
    {
        public static IEnumerable<T> SingleEnumerable<T>(T value) =>
            Params(value);

        public static T[] Params<T>(params T[] args) =>
            args;
    }
}
