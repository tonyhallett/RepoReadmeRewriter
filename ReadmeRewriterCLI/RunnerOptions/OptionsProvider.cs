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

            if (errors.Count >0)
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
            string? repoRef = parseResult.RepoRef;
            if (repoRef == null)
            {
                string? gitRoot = gitHelper.FindGitRoot(projectDirectory);
                if (gitRoot == null)
                {
                    errors.Add("no git root");
                    return null;
                }

                try
                {
                    repoRef = gitHelper.CommitSha(gitRoot) ?? "master";
                }
                catch (Exception exception)
                {
                    errors.Add(exception.Message);
                    return null;
                }
            }

            return repoRef;
        }

        private  static RewriteTagsOptions GetRewriteTagsOptions(ReadmeRewriterParseResult parseResult, List<string> errors)
        {
            RewriteTagsOptions options = RewriteTagsOptions.None;

            if (parseResult.RemoveHtml == true && parseResult.ErrorOnHtml == true)
            {
                errors.Add("Cannot set both RemoveHtml and ErrorOnHtml to true");
                return options;
            }

            if (parseResult.RemoveHtml == true)
            {
                options =  RewriteTagsOptions.RemoveHtml;
            }

            if (parseResult.ErrorOnHtml == true)
            {
                options =  RewriteTagsOptions.ErrorOnHtml;
            }

            if (parseResult.ExtractDetailsSummary == true)
            {
                options |= RewriteTagsOptions.ExtractDetailsContentWithoutSummary;
            }

            return options;
        }
    }
}
