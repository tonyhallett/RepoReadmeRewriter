using RepoReadmeRewriter.IOWrapper;

namespace ReadmeRewriterCLI.RunnerOptions.Config
{
    internal sealed class ConfigFileService(IIOHelper ioHelper) : IConfigFileService
    {
        private sealed class RelativeDirectories(string configDirectory, string projectDirectory)
        {
            internal string GetDirectory(bool getConfigDirectory) => getConfigDirectory ? configDirectory : projectDirectory;
        }

        private RelativeDirectories? _relativeDirectories;

        public string? GetConfigFile(string path)
        {
            string resolved = ResolveRelativeTo(path, true);
            if (ioHelper.FileExists(resolved))
            {
                return resolved;
            }

            resolved = ResolveRelativeTo(path, false);
            return ioHelper.FileExists(resolved) ? resolved : null;
        }

        private string ResolveRelativeTo(string path, bool toConfigDirectory) => _relativeDirectories == null
                ? throw new InvalidOperationException("Call GetConfigPath first")
                : ioHelper.EnsureAbsolute(_relativeDirectories.GetDirectory(toConfigDirectory), path);

        public string? GetConfigPath(string projectDirectory, string path)
        {
            string? resolvedConfigPath = ioHelper.EnsureAbsolute(projectDirectory, path);
            if (ioHelper.FileExists(resolvedConfigPath))
            {
                string? configDirectory = ioHelper.GetDirectoryName(resolvedConfigPath) ?? throw new InvalidOperationException("Could not determine config directory from config file path.");
                _relativeDirectories = new RelativeDirectories(configDirectory, projectDirectory);
            }
            else
            {
                resolvedConfigPath = null;
            }

            return resolvedConfigPath;
        }
    }
}
