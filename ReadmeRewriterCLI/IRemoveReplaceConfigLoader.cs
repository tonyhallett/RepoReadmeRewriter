using RepoReadmeRewriter.RemoveReplace.Settings;

internal interface IRemoveReplaceConfigLoader
{
    RemoveReplaceSettings? Load(string configPath, out List<string> loadErrors);
}
