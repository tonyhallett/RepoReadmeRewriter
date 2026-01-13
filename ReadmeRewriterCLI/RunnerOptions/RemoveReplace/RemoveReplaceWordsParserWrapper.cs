using System.Diagnostics.CodeAnalysis;
using RepoReadmeRewriter.RemoveReplace.Settings;

namespace ReadmeRewriterCLI.RunnerOptions.RemoveReplace
{
    [ExcludeFromCodeCoverage]
    internal sealed class RemoveReplaceWordsParserWrapper : IRemoveReplaceWordsParser
    {
        public List<RemoveReplaceWord> Parse(string[] lines) => RemoveReplaceWordsParser.Parse(lines);
    }
}
