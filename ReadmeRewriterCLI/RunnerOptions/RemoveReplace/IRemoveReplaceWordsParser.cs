using RepoReadmeRewriter.RemoveReplace.Settings;

namespace ReadmeRewriterCLI.RunnerOptions.RemoveReplace
{
    internal interface IRemoveReplaceWordsParser
    {
        List<RemoveReplaceWord> Parse(string[] lines);
    }
}
