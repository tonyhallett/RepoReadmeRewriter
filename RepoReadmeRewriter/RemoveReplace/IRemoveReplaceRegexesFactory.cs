using RepoReadmeRewriter.RemoveReplace.Settings;

namespace RepoReadmeRewriter.RemoveReplace
{
    internal interface IRemoveReplaceRegexesFactory
    {
        IRemoveReplaceRegexes Create(RemoveReplaceSettings removeReplaceSettings);
    }
}
