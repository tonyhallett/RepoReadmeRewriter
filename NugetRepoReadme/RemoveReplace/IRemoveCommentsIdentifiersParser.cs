using RepoReadmeRewriter.RemoveReplace.Settings;

namespace NugetRepoReadme.RemoveReplace
{
    internal interface IRemoveCommentsIdentifiersParser
    {
        RemoveCommentIdentifiers? Parse(string? removeCommentIdentifiers, IAddError addErrors);
    }
}
