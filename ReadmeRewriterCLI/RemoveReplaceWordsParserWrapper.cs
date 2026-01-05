using NugetRepoReadme.RemoveReplace.Settings;

namespace ReadmeRewriterCLI
{
    internal sealed class RemoveReplaceWordsParserWrapper : IRemoveReplaceWordsParser
    {
        public List<RemoveReplaceWord> Parse(string[] lines) => RemoveReplaceWordsParser.Parse(lines);
    }
}
