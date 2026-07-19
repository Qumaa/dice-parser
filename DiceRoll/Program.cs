using System.CommandLine;

namespace DiceRoll
{
    public static class Program
    {
        public static int Main(string[] args) =>
            new DiceCommand(new DiceCommandStrings()).Invoke("roll d20 > 10 ? 20 : 5 -t");
    }
}
