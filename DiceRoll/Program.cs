using System.CommandLine;

namespace DiceRoll
{
    public class Program
    {
        public static int Main(string[] args) =>
            new DiceCommand(new DiceCommandStrings()).Invoke(args);
    }
}
