using System.CommandLine;

namespace DiceRoll
{
    public static class Program
    {
        public static int Main(string[] args) =>
            new DiceCommand(new DiceCommandStrings()).Invoke("r d4 (10) s -t");
    }
}
