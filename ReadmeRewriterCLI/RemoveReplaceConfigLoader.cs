using System.Text.Json;
using NugetRepoReadme.IOWrapper;
using NugetRepoReadme.RemoveReplace.Settings;

namespace ReadmeRewriterCLI
{
    internal sealed class RemoveReplaceConfigLoader(
        IIOHelper ioHelper,
        IConfigFileService fileService,
        IRemoveReplaceWordsParser removeReplaceWordsParser) : IRemoveReplaceConfigLoader
    {
        private sealed class RemoveReplaceConfig
        {
            public RemoveCommentIdentifiersConfig? RemoveCommentIdentifiers { get; set; }

            public List<RemovalOrReplacementConfig>? RemovalsOrReplacements { get; set; }

            public List<string>? RemoveReplaceWordsFiles { get; set; }
        }

        private sealed class RemoveCommentIdentifiersConfig
        {
            public string? Start { get; set; }

            public string? End { get; set; }
        }

        private sealed class RemovalOrReplacementConfig
        {
            public string? CommentOrRegex { get; set; }

            public string? Start { get; set; }

            public string? End { get; set; }

            public string? ReplacementText { get; set; }

            public bool? ReplacementFromFile { get; set; }
        }

        private static readonly JsonSerializerOptions s_cachedJsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
        };

        private RemoveReplaceConfig? LoadAndParseJson(string configPath, List<string> loadErrors)
        {
            RemoveReplaceConfig? config;
            try
            {
                string json = ioHelper.ReadAllText(configPath);
                config = JsonSerializer.Deserialize<RemoveReplaceConfig>(json, s_cachedJsonSerializerOptions);
            }
            catch (Exception exception)
            {
                loadErrors.Add($"Failed to read config '{configPath}': {exception.Message}");
                return null;
            }

            if (config == null)
            {
                loadErrors.Add($"Config file '{configPath}' is empty or invalid.");
                return null;
            }

            return config;
        }

        public RemoveReplaceSettings? Load(
            string configPath,
            out List<string> loadErrors)
        {
            loadErrors = [];
            RemoveReplaceConfig? config = LoadAndParseJson(configPath, loadErrors);

            if (loadErrors.Count > 0)
            {
                return null;
            }

            RemoveCommentIdentifiers? removeCommentIdentifiers = ParseRemoveCommentIdentifiers(config!.RemoveCommentIdentifiers, loadErrors);
            List<RemovalOrReplacement> removalsOrReplacements = ParseRemovalsOrReplacements(config!.RemovalsOrReplacements, loadErrors);
            List<RemoveReplaceWord> removeReplaceWords = ParseRemoveReplaceWords(config!.RemoveReplaceWordsFiles, loadErrors);

            return loadErrors.Count > 0
                ? null
                : removeCommentIdentifiers == null && removalsOrReplacements.Count == 0 && removeReplaceWords.Count == 0
                ? null
                : new RemoveReplaceSettings(removeCommentIdentifiers, removalsOrReplacements, removeReplaceWords);
        }

        private static RemoveCommentIdentifiers? ParseRemoveCommentIdentifiers(RemoveCommentIdentifiersConfig? config, List<string> loadErrors)
        {
            if (config == null)
            {
                return null;
            }

            string start = config.Start?.Trim() ?? string.Empty;
            string? end = string.IsNullOrWhiteSpace(config.End) ? null : config.End!.Trim();
            if (string.IsNullOrWhiteSpace(start))
            {
                loadErrors.Add("removeCommentIdentifiers.start is required.");
                return null;
            }

            if (end != null && string.Equals(start, end, StringComparison.Ordinal))
            {
                loadErrors.Add("removeCommentIdentifiers start and end cannot be the same.");
                return null;
            }

            return new RemoveCommentIdentifiers(start, end);
        }

        private List<RemovalOrReplacement> ParseRemovalsOrReplacements(IEnumerable<RemovalOrReplacementConfig>? configs, List<string> loadErrors)
        {
            var removalsOrReplacements = new List<RemovalOrReplacement>();
            if (configs == null)
            {
                return removalsOrReplacements;
            }

            foreach (RemovalOrReplacementConfig config in configs)
            {
                string start = config.Start?.Trim() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(start))
                {
                    loadErrors.Add("removalsOrReplacements.start is required.");
                    continue;
                }

                if (!Enum.TryParse(config.CommentOrRegex, true, out CommentOrRegex commentOrRegex))
                {
                    loadErrors.Add($"Unsupported commentOrRegex '{config.CommentOrRegex}' for '{start}'.");
                    continue;
                }

                string? end = string.IsNullOrWhiteSpace(config.End) ? null : config.End!.Trim();
                if (end != null && string.Equals(start, end, StringComparison.Ordinal))
                {
                    loadErrors.Add($"removalsOrReplacements start and end cannot be the same for '{start}'.");
                    continue;
                }

                string? replacementText = config.ReplacementText;
                if (config.ReplacementFromFile == true)
                {
                    if (replacementText == null)
                    {
                        loadErrors.Add($"removalsOrReplacements.replacementText is required when replacementFromFile is true for '{start}'.");
                        continue;
                    }

                    string? resolvedFilePath = fileService.GetConfigFile(replacementText);
                    if (resolvedFilePath == null)
                    {
                        loadErrors.Add($"Failed to load replacement text from file '{replacementText}'.");
                        continue;
                    }

                    replacementText = ioHelper.ReadAllText(resolvedFilePath);
                }

                removalsOrReplacements.Add(new RemovalOrReplacement(commentOrRegex, start, end, replacementText));
            }

            return removalsOrReplacements;
        }

        private List<RemoveReplaceWord> ParseRemoveReplaceWords(IEnumerable<string>? filePaths, List<string> loadErrors)
        {
            var removeReplaceWords = new List<RemoveReplaceWord>();
            if (filePaths == null)
            {
                return removeReplaceWords;
            }

            foreach (string filePath in filePaths)
            {
                string? resolvedPath = fileService.GetConfigFile(filePath);
                if (resolvedPath == null)
                {
                    loadErrors.Add($"Failed to load removeReplaceWords file '{filePath}'.");
                    continue;
                }

                if (loadErrors.Count > 0)
                {
                    continue;
                }

                string[] lines = ioHelper.ReadAllLines(resolvedPath);
                removeReplaceWords.AddRange(removeReplaceWordsParser.Parse(lines));
            }

            return removeReplaceWords;
        }
    }
}
