using NugetRepoReadme.RemoveReplace.Settings;

internal interface IRemoveReplaceConfigLoader
{
    RemoveReplaceSettings? Load(string configPath, out List<string> loadErrors);
}
