namespace ReadmeRewriterCLI.RunnerOptions.RemoveReplace
{
    internal interface IRemoveReplaceConfigDeserializer
    {
        RemoveReplaceConfig? LoadAndParseJson(string configPath, List<string> loadErrors);
    }
}
