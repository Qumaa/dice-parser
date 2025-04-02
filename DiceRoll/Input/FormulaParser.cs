namespace DiceRoll.Input
{
    public class FormulaParser
    {
        public bool TryParse<T>(string[] args, int start, out int end, out INode node)
        {
            end = 0;
            node = null;
            
            return false;
        }
    }
}
