using System.Diagnostics.CodeAnalysis;
using ReadmeRewriterCLI.RunnerOptions.CommandLineParsing;
using ReadmeRewriterCLI.RunnerOptions.Config;
using ReadmeRewriterCLI.RunnerOptions.Git;
using ReadmeRewriterCLI.RunnerOptions.RemoveReplace;
using RepoReadmeRewriter.IOWrapper;
using RepoReadmeRewriter.Processing;
using RepoReadmeRewriter.RemoveReplace.Settings;

namespace ReadmeRewriterCLI.RunnerOptions
{
    internal sealed class OptionsProvider(
        IConfigFileService configFileService,
        IGitHelper gitHelper,
        IRemoveReplaceConfigLoader removeReplaceConfigLoader,
        IIOHelper ioHelper
        ) : IOptionsProvider
    {
        [ExcludeFromCodeCoverage]
        public OptionsProvider() : this(
            ConfigFileService.Instance,
            new GitHelper(),
            new RemoveReplaceConfigLoader(),
            IOHelper.Instance)
        {
        }

        public (Options? options, IEnumerable<string>? errors) Provide(ReadmeRewriterParseResult parseResult)
        {
            string projectDir = parseResult.ProjectDir;
            if (!ioHelper.DirectoryExists(projectDir))
            {
                return (null, [$"Project directory does not exist: {projectDir}"]);
            }

            List<string> errors = [];

            string? repoRef = GetRepoRef(parseResult, errors, projectDir);

            RemoveReplaceSettings? removeReplace = GetRemoveReplaceSettings(parseResult.ConfigPath, errors, projectDir);

            RewriteTagsOptions rewriteTagsOptions = GetRewriteTagsOptions(parseResult, errors);

            if (errors.Count > 0)
            {
                return (null, errors);
            }

            return (
                new Options(
                    projectDir,
                    parseResult.RepoUrl,
                    repoRef!,
                    parseResult.ReadmeRelative,
                    rewriteTagsOptions,
                    removeReplace,
                    ioHelper.EnsureAbsolute(
                        projectDir,
                        parseResult.OutputReadme!)
                    ),
                null);
        }

        private RemoveReplaceSettings? GetRemoveReplaceSettings(string? configPath, List<string> errors, string projectDirectory)
        {
            if (string.IsNullOrWhiteSpace(configPath))
            {
                return null;
            }

            string? absoluteConfigPath = configFileService.GetConfigPath(projectDirectory, configPath);
            if (absoluteConfigPath == null)
            {
                errors.Add($"Config file not found: {configPath}");
                return null;
            }

            RemoveReplaceSettings? removeReplace = removeReplaceConfigLoader.Load(
                absoluteConfigPath,
                out List<string>? loadErrors);
            if (loadErrors.Count > 0)
            {
                errors.AddRange(loadErrors);
                return null;
            }

            return removeReplace;
        }

        private string? GetRepoRef(ReadmeRewriterParseResult parseResult, List<string> errors, string projectDirectory)
        {
            if (parseResult.RepoRef != null)
            {
                return parseResult.RepoRef;
            }

            string? gitRoot = gitHelper.FindGitRoot(projectDirectory);
            if (gitRoot == null)
            {
                errors.Add("no git root");
                return null;
            }

            try
            {
                string? repoRef = parseResult.GitRefKind switch
                {
                    GitRefKind.TagOrSha => gitHelper.TagOrSha(gitRoot),
                    GitRefKind.BranchName => gitHelper.BranchName(gitRoot),
                    GitRefKind.ShortCommitSha => gitHelper.ShortCommitSha(gitRoot),
                    GitRefKind.CommitSha => gitHelper.CommitSha(gitRoot),
                    _ => gitHelper.CommitSha(gitRoot) // default to commit sha
                };

                if (string.IsNullOrWhiteSpace(repoRef) && parseResult.GitRefKind == GitRefKind.Auto)
                {
                    repoRef = "master";
                }

                if (repoRef == null)
                {
                    errors.Add("Unable to obtain repo reference");
                }

                return repoRef;
            }
            catch (Exception exception)
            {
                errors.Add(exception.Message);
                return null;
            }
        }

        private static RewriteTagsOptions GetRewriteTagsOptions(ReadmeRewriterParseResult parseResult, List<string> errors)
        {
            RewriteTagsOptions options = RewriteTagsOptions.None;

            if (parseResult.RemoveHtml == true && parseResult.ErrorOnHtml == true)
            {
                errors.Add("Cannot set both RemoveHtml and ErrorOnHtml to true");
                return options;
            }

            if (parseResult.RemoveHtml == true)
            {
                options = RewriteTagsOptions.RemoveHtml;
            }

            if (parseResult.ErrorOnHtml == true)
            {
                options = RewriteTagsOptions.ErrorOnHtml;
            }

            if (parseResult.ExtractDetailsSummary == true)
            {
                options |= RewriteTagsOptions.ExtractDetailsContentWithoutSummary;
            }

            return options;
        }
    }
}
