using System.Collections.Generic;

namespace RepoReadmeRewriter.ReadmeReplacement
{
    internal sealed class ReadmeReplacer : IReadmeReplacer
    {
        public IReplacementResult Replace(string text, IEnumerable<SourceReplacement> replacements)
            => new ReplacementResult(text, replacements);
    }
}
