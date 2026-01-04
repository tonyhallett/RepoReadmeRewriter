using NugetRepoReadme.Processing;
using NugetRepoReadme.RemoveReplace.Settings;

namespace ReadmeRewriterCLI
{
    internal class OptionsProvider(
        IConsoleWriter consoleWriter,
        IConfigFileService fileService,
        IGitHelper gitHelper,
        IRemoveReplaceConfigLoader removeReplaceConfigLoader) : IOptionsProvider
    {
        public (Options? options, IEnumerable<string>? errors) Provide(ReadmeRewriterParseResult parseResult)
        {
            string? repoRef = parseResult.RepoRef;
            if (repoRef == null)
            {
                string? gitRoot = gitHelper.FindGitRoot(Environment.CurrentDirectory);
                if (gitRoot == null)
                {
                    return (null, ["no git root"]);
                }

                try
                {
                    repoRef = gitHelper.CommitSha(gitRoot) ?? "master";
                }
                catch (Exception exception)
                {
                    return (null, [exception.Message]);
                }
            }

            RemoveReplaceSettings? removeReplace = null;
            if (!string.IsNullOrWhiteSpace(parseResult.ConfigPath))
            {
                string? configPath = fileService.GetConfigPath(Environment.CurrentDirectory, parseResult.ConfigPath);
                if (configPath == null)
                {
                    return (null, [$"Config file not found: {parseResult.ConfigPath}"]);
                }

                removeReplace = removeReplaceConfigLoader.Load(
                    configPath,
                    out List<string>? loadErrors);

                if (loadErrors.Count > 0)
                {
                    return (null, loadErrors);
                }
            }

            return (new Options(
                Environment.CurrentDirectory, // todo - command line argument ?
                parseResult.RepoUrl!,
                repoRef,
                parseResult.ReadmeRelative ?? "README.md",
                ParseRewriteTagsOptions(parseResult.RewriteTags),
                removeReplace), null);
        }

        private RewriteTagsOptions ParseRewriteTagsOptions(string? rewriteTags)
        {
            RewriteTagsOptions rewriteTagsOptions = RewriteTagsOptions.None;
            if (rewriteTags != null && !Enum.TryParse(rewriteTags, out rewriteTagsOptions))
            {
                // todo - should not be aware of --rewrite-tags
                consoleWriter.WriteWarning($"Could not parse --rewrite-tags '{rewriteTags}', defaulting to None.");
                rewriteTagsOptions = RewriteTagsOptions.None;
            }

            return rewriteTagsOptions;
        }
    }
}
