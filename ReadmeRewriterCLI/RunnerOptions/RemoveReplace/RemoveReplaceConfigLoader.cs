using System.Diagnostics.CodeAnalysis;
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

        private class StartEnd(string start, string? end)
        {
            public string Start { get; } = start;
            public string? End { get; } = end;
        }

        [ExcludeFromCodeCoverage]
        public RemoveReplaceConfigLoader() : this(
            IOHelper.Instance,
            new RemoveReplaceConfigDeserializer(),
            ConfigFileService.Instance,
            new RemoveReplaceWordsParserWrapper())
        {
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
            string startValue =start?.Trim() ?? string.Empty;

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

        private class FileRemovalOrReplacement(StartEnd startEnd, CommentOrRegex commentOrRegex, string replacementTextFilePath)
        {
            public StartEnd StartEnd { get; } = startEnd;

            public CommentOrRegex CommentOrRegex { get; } = commentOrRegex;

            public string ReplacementTextFilePath { get; } = replacementTextFilePath;
        }

        private List<RemovalOrReplacement> ParseRemovalsOrReplacements(IEnumerable<RemovalOrReplacementConfig>? configs, List<string> loadErrors)
        {
            var removalsOrReplacements = new List<RemovalOrReplacement>();
            if (configs == null)
            {
                return removalsOrReplacements;
            }

            int counter = -1;
            List<FileRemovalOrReplacement> fileRemovalOrReplacements = [];
            foreach (RemovalOrReplacementConfig config in configs)
            {
                counter++;
                string errorPrefix = $"removalsOrReplacements[{counter}]";
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

                if (config.ReplacementFromFile == true)
                {
                    string? resolvedReplacementTextFilePath = GetResolvedReplacementTextFilePath(config, loadErrors, errorPrefix);
                    if(resolvedReplacementTextFilePath == null)
                    {
                        continue;
                    }

                    if (parsedCommentOrRegex.HasValue && startEnd != null)
                    {
                        fileRemovalOrReplacements.Add(new FileRemovalOrReplacement(startEnd, parsedCommentOrRegex.Value, resolvedReplacementTextFilePath));
                    }
                }
                else
                {
                    if (parsedCommentOrRegex.HasValue && startEnd != null)
                    {
                        removalsOrReplacements.Add(new RemovalOrReplacement(parsedCommentOrRegex.Value, startEnd.Start, startEnd.End, config.ReplacementText));
                    }
                }
            }

            if (loadErrors.Count == 0)
            {
                removalsOrReplacements.AddRange(
                    fileRemovalOrReplacements.Select(frr => new RemovalOrReplacement(
                        frr.CommentOrRegex,
                        frr.StartEnd.Start,
                        frr.StartEnd.End,
                        ioHelper.ReadAllText(frr.ReplacementTextFilePath))));
            }
            
            return removalsOrReplacements;
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
