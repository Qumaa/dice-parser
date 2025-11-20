using System.CommandLine;

namespace DiceRoll
{
    public static class Program
    {
        public static int Main(string[] args) =>
            new DiceCommand(new DiceCommandStrings()).Invoke("r 2 - -2");
    }
}
