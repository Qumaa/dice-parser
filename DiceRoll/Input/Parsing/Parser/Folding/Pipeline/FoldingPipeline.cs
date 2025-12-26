using System;
using System.Collections.Generic;
using System.Linq;

namespace DiceRoll.Input.Parsing
{
    public sealed class FoldingPipeline
    {
        private readonly Folder[] _folders;

        public FoldingPipeline(IEnumerable<Folder> folders)
        {
            ArgumentNullException.ThrowIfNull(folders);
            
            _folders = folders.ToArray();
        }

        public void ExecuteAll(LexemesList lexemes, UnknownLexemeSolver solver)
        {
            foreach (Folder folder in _folders)
                folder.Execute(lexemes);
        }
    }
}
