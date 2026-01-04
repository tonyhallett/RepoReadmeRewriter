namespace ReadmeRewriterCLI
{
    internal interface IConfigFileService
    {
        string? GetConfigFile(string replacementText);

        string? GetConfigPath(string projectDirectory, string path);
    }
}
