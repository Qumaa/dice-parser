using System;

namespace DiceRoll.Input.Parsing
{
    public sealed class ParseContext
    {
        private readonly GrammarCollection _collection;
        private Substring _unrecognized;
        
        public ParseContext(string source, GrammarCollection collection)
        {
            _unrecognized = new Substring(source).TrimStart();
            _collection = collection;
        }

        public bool HasUnrecognizedInput => _unrecognized.Length > 0;

        public Substring GetUnrecognized() =>
            _unrecognized;

        public bool TryGetGrammarByTag(string tag, out IGrammar grammar) =>
            _collection.TryGetGrammarByTag(tag, out grammar);

        public void PushRecognized(int recognized) =>
            _unrecognized = _unrecognized.MoveStart(Math.Min(_unrecognized.Length, recognized));
        
        public void PopRecognized(int recognized) =>
            _unrecognized = _unrecognized.MoveStart(-Math.Min(_unrecognized.Start, recognized));
    }

    public static class ParseContextExceptions
    {
        public static int PushRecognized(this ParseContext context, GrammarProbe probe) =>
            context.PushRecognized(probe.RecognizedStart, probe.RecognizedLength);
        
        public static int PushRecognized(this ParseContext context, int start, int length)
        {
            int recognized = context.GetRecognizedOffset(start, length);

            context.PushRecognized(recognized);
            
            return recognized;
        }
        
        public static int GetRecognizedOffset(this ParseContext context, GrammarProbe probe) =>
            context.GetRecognizedOffset(probe.RecognizedStart, probe.RecognizedLength);

        public static int GetRecognizedOffset(this ParseContext context, int start, int length)
        {
            Substring unrecognized = context.GetUnrecognized();
            int end = start + length;

            return Math.Max(0, end - unrecognized.Start);
        }
    }
}
