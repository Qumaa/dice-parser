using System;

namespace DiceRoll.Input
{
    public delegate INumeric OperandHandler(ReadOnlySpan<char> match);
}
