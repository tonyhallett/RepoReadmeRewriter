using ReadmeRewriterCLI.RunnerOptions.Config;
using RepoReadmeRewriter.IOWrapper;
using RepoReadmeRewriter.RemoveReplace.Settings;

namespace ReadmeRewriterCLI.RunnerOptions.RemoveReplace
{
    internal sealed class RemoveReplaceConfigLoader(
        IIOHelper ioHelper,
        IRemoveReplaceConfigDeserializer removeReplaceConfigDeserializer,
        IConfigFileService configFileService,
        IRemoveReplaceWordsParser removeReplaceWordsParser) : IRemoveReplaceConfigLoader
    {

        private sealed class StartEnd(string start, string? end)
        {
            public string Start { get; } = start;
            public string? End { get; } = end;
        }

        public RemoveReplaceSettings? Load(
            string configPath,
            out List<string> loadErrors)
        {
            loadErrors = [];
            RemoveReplaceConfig? config = removeReplaceConfigDeserializer.LoadAndParseJson(configPath, loadErrors);

            if (loadErrors.Count > 0)
            {
                return null;
            }

            RemoveCommentIdentifiers? removeCommentIdentifiers = ParseRemoveCommentIdentifiers(config!.RemoveCommentIdentifiers, loadErrors);
            List<RemovalOrReplacement> removalsOrReplacements = ParseRemovalsOrReplacements(config!.RemovalsOrReplacements, loadErrors);
            List<RemoveReplaceWord> removeReplaceWords = ParseRemoveReplaceWords(config!.RemoveReplaceWordsFilePaths, loadErrors);

            bool configNoConfig = removeCommentIdentifiers == null && removalsOrReplacements.Count == 0 && removeReplaceWords.Count == 0;
            if (configNoConfig && loadErrors.Count == 0)
            {
                loadErrors.Add("Config contained no configuration");
            }

            return loadErrors.Count > 0
                ? null
                : new RemoveReplaceSettings(removeCommentIdentifiers, removalsOrReplacements, removeReplaceWords);
        }

        private static RemoveCommentIdentifiers? ParseRemoveCommentIdentifiers(RemoveCommentIdentifiersConfig? config, List<string> loadErrors)
        {
            if (config == null)
            {
                return null;
            }

            StartEnd? startEnd = GetStartEnd(config.Start, config.End, "removeCommentIdentifiers", loadErrors);
            return startEnd == null ? null : new RemoveCommentIdentifiers(startEnd.Start, startEnd.End);
        }

        private static StartEnd? GetStartEnd(string? start, string? end, string errorPrefix, List<string> loadErrors)
        {
            string startValue = start?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(startValue))
            {
                loadErrors.Add($"{errorPrefix}.start is required.");
                return null;
            }

            if (end != null && string.IsNullOrWhiteSpace(end))
            {
                loadErrors.Add($"{errorPrefix}.end is whitespace.");
                return null;
            }

            string? endValue = end == null ? null : end!.Trim();

            if (endValue != null && string.Equals(start, end, StringComparison.Ordinal))
            {
                loadErrors.Add($"{errorPrefix} start and end cannot be the same.");
                return null;
            }

            return new StartEnd(startValue, endValue);
        }

        private sealed class FileRemovalOrReplacement(StartEndCommentOrRegex startEndCommentOrRegex, string replacementTextFilePath)
        {
            public string Start { get; } = startEndCommentOrRegex.Start;

            public string? End { get; } = startEndCommentOrRegex.End;

            public CommentOrRegex CommentOrRegex { get; } = startEndCommentOrRegex.CommentOrRegex;

            public string ReplacementTextFilePath { get; } = replacementTextFilePath;
        }

        private sealed class StartEndCommentOrRegex(StartEnd startEnd, CommentOrRegex commentOrRegex)
        {
            public string Start { get; } = startEnd.Start;

            public string? End { get; } = startEnd.End;

            public CommentOrRegex CommentOrRegex { get; } = commentOrRegex;
        }

        private List<RemovalOrReplacement> ParseRemovalsOrReplacements(IEnumerable<RemovalOrReplacementConfig>? configs, List<string> loadErrors)
        {
            var removalsOrReplacements = new List<RemovalOrReplacement>();
            if (configs == null)
            {
                return removalsOrReplacements;
            }

            List<FileRemovalOrReplacement> fileRemovalOrReplacements = [];
            foreach ((RemovalOrReplacementConfig config, int counter) in configs.Select((config, index) => (config, index)))
            {
                string errorPrefix = $"removalsOrReplacements[{counter}]";
                StartEndCommentOrRegex? validated = ValidateStartEndCommentOrRegex(config, errorPrefix, loadErrors);

                if (config.ReplacementFromFile == true)
                {
                    string? resolvedReplacementTextFilePath = GetResolvedReplacementTextFilePath(config, loadErrors, errorPrefix);
                    if (resolvedReplacementTextFilePath != null && validated != null)
                    {
                        fileRemovalOrReplacements.Add(new FileRemovalOrReplacement(validated, resolvedReplacementTextFilePath));
                    }
                }
                else if (validated != null)
                {
                    removalsOrReplacements.Add(
                        new RemovalOrReplacement(validated.CommentOrRegex, validated.Start, validated.End, config.ReplacementText));
                }
            }

            if (loadErrors.Count == 0)
            {
                removalsOrReplacements.AddRange(
                    fileRemovalOrReplacements.Select(frr => new RemovalOrReplacement(
                        frr.CommentOrRegex,
                        frr.Start,
                        frr.End,
                        ioHelper.ReadAllText(frr.ReplacementTextFilePath))));
            }

            return removalsOrReplacements;
        }

        private static StartEndCommentOrRegex? ValidateStartEndCommentOrRegex(RemovalOrReplacementConfig config, string errorPrefix, List<string> loadErrors)
        {
            StartEnd? startEnd = GetStartEnd(config.Start, config.End, errorPrefix, loadErrors);
            CommentOrRegex? parsedCommentOrRegex = null;
            if (!Enum.TryParse(config.CommentOrRegex, true, out CommentOrRegex commentOrRegex))
            {
                loadErrors.Add($"{errorPrefix}.commentOrRegex unsupported '{config.CommentOrRegex}'.");
            }
            else
            {
                parsedCommentOrRegex = commentOrRegex;
            }

            return startEnd != null && parsedCommentOrRegex.HasValue
                ? new StartEndCommentOrRegex(startEnd, parsedCommentOrRegex.Value)
                : null;
        }

        private string? GetResolvedReplacementTextFilePath(RemovalOrReplacementConfig config, List<string> loadErrors, string errorPrefix)
        {
            string? replacementText = config.ReplacementText;

            if (replacementText == null)
            {
                loadErrors.Add($"{errorPrefix}.replacementText is required when replacementFromFile is true.");
                return null;
            }

            string? resolvedFilePath = configFileService.GetConfigFile(replacementText);
            if (resolvedFilePath == null)
            {
                loadErrors.Add($"{errorPrefix}.replacementText - failed to load from file '{replacementText}'.");
                return null;
            }

            return resolvedFilePath;
        }

        private List<RemoveReplaceWord> ParseRemoveReplaceWords(IEnumerable<string>? filePaths, List<string> loadErrors)
        {
            var removeReplaceWords = new List<RemoveReplaceWord>();
            if (filePaths == null)
            {
                return removeReplaceWords;
            }

            List<string> resolvedReplaceWordsFilePaths = GetRemoveReplaceWordsFilePaths(filePaths, loadErrors);
            if (loadErrors.Count == 0)
            {
                removeReplaceWords = [.. resolvedReplaceWordsFilePaths.SelectMany(ReadAndParse)];
            }

            List<RemoveReplaceWord> ReadAndParse(string resolvedPath)
            {
                string[] lines = ioHelper.ReadAllLines(resolvedPath);
                return removeReplaceWordsParser.Parse(lines);
            }

            return removeReplaceWords;
        }

        private List<string> GetRemoveReplaceWordsFilePaths(IEnumerable<string> filePaths, List<string> loadErrors)
        {
            List<string> resolvedFilePaths = [];
            foreach (string filePath in filePaths)
            {
                string? resolvedPath = configFileService.GetConfigFile(filePath);
                if (resolvedPath == null)
                {
                    loadErrors.Add($"Failed to load removeReplaceWords file '{filePath}'.");
                    continue;
                }
                else
                {
                    resolvedFilePaths.Add(resolvedPath);
                }
            }

            return resolvedFilePaths;
        }
    }
}
