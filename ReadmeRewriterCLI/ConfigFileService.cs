namespace ReadmeRewriterCLI
{
    internal sealed class ConfigFileService : IConfigFileService
    {
        public static IConfigFileService Instance { get; } = new ConfigFileService();

        private string? _configPath;
        private string? _projectDirectory;

        public string? GetConfigFile(string path)
        {
            if (Path.IsPathRooted(path))
            {
                return ExistsOrNull(path);
            }

            string resolved = ResolveRelativeToConfigPath(path);
            if (File.Exists(resolved))
            {
                return resolved;
            }

            resolved = ResolveRelativeToProjectDirectory(path);
            return ExistsOrNull(resolved);
        }

        private static string? ExistsOrNull(string path) => File.Exists(path) ? path : null;

        private string ResolveRelativeToConfigPath(string path)
        {
            if (_configPath == null)
            {
                throw new InvalidOperationException("Config path is not set. Call GetConfigPath first.");
            }

            string configDirectory = Path.GetDirectoryName(_configPath) ?? string.Empty;
            return Path.Combine(configDirectory, path);
        }

        private string ResolveRelativeToProjectDirectory(string path) => _projectDirectory == null
                ? throw new InvalidOperationException("Project directory is not set. Call GetConfigPath first.")
                : Path.Combine(_projectDirectory, path);

        public string? GetConfigPath(string projectDirectory, string path)
        {
            _projectDirectory = projectDirectory;
            // if path is absolute, return it directly
            string? resolvedConfigPath = Path.IsPathRooted(path) ? path : ResolveRelativeToProjectDirectory(path);
            if (File.Exists(resolvedConfigPath))
            {
                _configPath = resolvedConfigPath;
            }

            return _configPath;
        }
    }
}
