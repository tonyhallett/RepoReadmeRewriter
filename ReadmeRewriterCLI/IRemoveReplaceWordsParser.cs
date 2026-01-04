using NugetRepoReadme.RemoveReplace.Settings;

namespace ReadmeRewriterCLI
{
    internal interface IRemoveReplaceWordsParser
    {
        List<RemoveReplaceWord> Parse(string[] lines);
    }
}
