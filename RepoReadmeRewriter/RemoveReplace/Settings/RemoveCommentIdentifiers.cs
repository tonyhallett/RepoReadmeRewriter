namespace NugetRepoReadme.RemoveReplace.Settings
{
    public class RemoveCommentIdentifiers(string startCommentIdentifier, string? endCommentIdentifier)
    {
        public string Start { get; } = startCommentIdentifier;

        public string? End { get; } = endCommentIdentifier;
    }
}
