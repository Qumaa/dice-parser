using System;
using System.Runtime.InteropServices;

namespace DiceRoll.Input.Parsing
{
    [StructLayout(LayoutKind.Auto)]
    public readonly struct RelationPair : IEquatable<RelationPair>
    {
        private readonly string _tag1;
        private readonly string _tag2;
            
        public RelationPair(string tag1, string tag2)
        {
            _tag1 = tag1;
            _tag2 = tag2;
        }

        public bool Equals(RelationPair other) =>
            _tag1 == other._tag1 && _tag2 == other._tag2;

        public override bool Equals(object obj) =>
            obj is RelationPair other && Equals(other);

        public override int GetHashCode() =>
            HashCode.Combine(_tag1, _tag2);

        public static bool operator ==(RelationPair left, RelationPair right) =>
            left.Equals(right);

        public static bool operator !=(RelationPair left, RelationPair right) =>
            !left.Equals(right);
    }
}
