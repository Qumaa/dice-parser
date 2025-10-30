namespace DiceRoll.Input.Parsing
{
    public abstract class OperandParser
    {
        // note: this constructor is important, as it effectively prevents deriving this class outside the assembly
        // only flat and recursive variants are ever expected to be used and derived
        // do not remove
        protected private OperandParser() { }
        
        public static FlatOperandParser FromDelegate(FlatOperandParsingHandler handler) =>
            FlatOperandParser.FromDelegate(handler);
        
        public static RecursiveOperandParser FromDelegate(RecursiveOperandParsingHandler handler) =>
            RecursiveOperandParser.FromDelegate(handler);
    }
}
