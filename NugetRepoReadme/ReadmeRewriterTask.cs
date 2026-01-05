using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Build.Framework;
using NugetRepoReadme.MSBuild;
using NugetRepoReadme.MSBuildHelpers;
using NugetRepoReadme.NugetValidation;
using NugetRepoReadme.RemoveReplace;
using NugetRepoReadme.RemoveReplace.Settings;
using RepoReadmeRewriter.Processing;
using RepoReadmeRewriter.Runner;
using InputOutputHelper = RepoReadmeRewriter.IOWrapper.IOHelper;

namespace NugetRepoReadme
{
    public class ReadmeRewriterTask : Microsoft.Build.Utilities.Task
    {
        internal const RewriteTagsOptions DefaultRewriteTagsOptions = RepoReadmeRewriter.Processing.RewriteTagsOptions.None;

        [Required]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public string ProjectDirectoryPath { get; set; }

        [Output]
        public string OutputReadme { get; set; }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

        // one of these required
        public string? RepositoryUrl { get; set; }

        public string? ReadmeRepositoryUrl { get; set; }

        public string? ReadmeRelativePath { get; set; }

        #region for ref - in this order

        public string? RepositoryRef { get; set; }

        public string? RepositoryCommit { get; set; }

        public string? RepositoryBranch { get; set; }

        #endregion
        public string? ErrorOnHtml { get; set; }

        public string? RemoveHtml { get; set; }

        public string? ExtractDetailsContentWithoutSummary { get; set; }

        public string? RewriteTagsOptions { get; set; }

        public string? RemoveCommentIdentifiers { get; set; }

        public ITaskItem[]? RemoveReplaceItems { get; set; }

        public ITaskItem[]? RemoveReplaceWordsItems { get; set; }

        internal IMessageProvider MessageProvider { get; set; } = MSBuild.MessageProvider.Instance;

        internal IRemoveReplaceSettingsProvider RemoveReplaceSettingsProvider { get; set; } = new RemoveReplaceSettingsProvider(
            new MSBuildMetadataProvider(),
            new RemoveCommentsIdentifiersParser(MSBuild.MessageProvider.Instance),
            new RemovalOrReplacementProvider(InputOutputHelper.Instance, MSBuild.MessageProvider.Instance),
            new RemoveReplaceWordsProvider(InputOutputHelper.Instance, MSBuild.MessageProvider.Instance),
            MSBuild.MessageProvider.Instance);

        internal IReadmeRewriterRunner Runner { get; set; } = new ReadmeRewriterRunner(new NuGetImageDomainValidator());

        public override bool Execute()
        {
            LaunchDebuggerIfRequired();

            IRemoveReplaceSettingsResult removeReplaceSettingsResult = RemoveReplaceSettingsProvider.Provide(
                RemoveReplaceItems,
                RemoveReplaceWordsItems,
                RemoveCommentIdentifiers);

            if (removeReplaceSettingsResult.Errors.Count > 0)
            {
                foreach (string error in removeReplaceSettingsResult.Errors)
                {
                    Log.LogError(error);
                }
            }
            else
            {
                ReadmeRewriterRunnerResult result = Runner.Run(
                    ProjectDirectoryPath,
                    ReadmeRelativePath,
                    GetRepositoryUrl(),
                    GetRef(),
                    GetRewriteTagsOptions(),
                    removeReplaceSettingsResult.Settings);

                foreach (string error in result.Errors)
                {
                    Log.LogError(error);
                }

                if (result.Success)
                {
                    OutputReadme = result.OutputReadme!;
                }
            }

            DebugFileWriter.WriteToFile();
            return !Log.HasLoggedErrors;
        }

        [ExcludeFromCodeCoverage]
        private void LaunchDebuggerIfRequired()
        {
            if (Environment.GetEnvironmentVariable("DebugReadmeRewriter") != "1" || Debugger.IsAttached)
            {
                return;
            }

            _ = Debugger.Launch();
        }

        private string? GetRepositoryUrl() => ReadmeRepositoryUrl ?? RepositoryUrl;

        private string GetRef() => RepositoryRef ?? RepositoryCommit ?? RepositoryBranch ?? "master";

        private RewriteTagsOptions GetRewriteTagsOptions()
        {
            RewriteTagsOptions options = DefaultRewriteTagsOptions;
            if (RewriteTagsOptions != null)
            {
                if (Enum.TryParse(RewriteTagsOptions, out RewriteTagsOptions parsedOptions))
                {
                    options = parsedOptions;
                }
                else
                {
                    Log.LogWarning(MessageProvider.CouldNotParseRewriteTagsOptionsUsingDefault(RewriteTagsOptions, DefaultRewriteTagsOptions));
                }
            }
            else
            {
                bool errorsOnHtml = false;
                if (bool.TryParse(ErrorOnHtml, out bool errorOnHtml) && errorOnHtml)
                {
                    options = RepoReadmeRewriter.Processing.RewriteTagsOptions.ErrorOnHtml;
                    errorsOnHtml = true;
                }

                if (!errorsOnHtml && bool.TryParse(RemoveHtml, out bool removeHtml) && removeHtml)
                {
                    options = RepoReadmeRewriter.Processing.RewriteTagsOptions.RemoveHtml;
                }

                if (bool.TryParse(ExtractDetailsContentWithoutSummary, out bool extractDetailsContentWithoutSummary) && extractDetailsContentWithoutSummary)
                {
                    options |= RepoReadmeRewriter.Processing.RewriteTagsOptions.ExtractDetailsContentWithoutSummary;
                }
            }

            return options;
        }
    }
}
