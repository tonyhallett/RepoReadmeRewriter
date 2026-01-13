using RepoReadmeRewriter.RemoveReplace.Settings;

namespace RepoReadmeRewriter.RemoveReplace
{
    internal interface IRemoveReplacer
    {
        string RemoveReplace(string text, RemoveReplaceSettings removeReplaceSettings);
    }
}
