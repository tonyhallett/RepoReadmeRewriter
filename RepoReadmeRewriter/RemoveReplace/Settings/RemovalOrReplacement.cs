namespace RepoReadmeRewriter.RemoveReplace.Settings
{
    public class RemovalOrReplacement(
        CommentOrRegex commentOrRegex,
        string start,
        string? end,
        string? replacementText)
    {
        public CommentOrRegex CommentOrRegex { get; } = commentOrRegex;

        public string Start { get; } = start;

        public string? End { get; } = end;

        public string? ReplacementText { get; set; } = replacementText;
    }
}
