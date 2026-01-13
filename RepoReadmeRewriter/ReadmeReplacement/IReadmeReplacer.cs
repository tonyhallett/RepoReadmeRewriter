using System.Collections.Generic;

namespace RepoReadmeRewriter.ReadmeReplacement
{
    internal interface IReadmeReplacer
    {
        IReplacementResult Replace(string text, IEnumerable<SourceReplacement> replacements);

    }
}
