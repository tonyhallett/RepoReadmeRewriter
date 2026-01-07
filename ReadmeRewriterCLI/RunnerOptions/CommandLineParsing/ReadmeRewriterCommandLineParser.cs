using System.CommandLine;
using ReadmeRewriterCLI.RunnerOptions.CommandLineParsing.Extensions;

namespace ReadmeRewriterCLI.RunnerOptions.CommandLineParsing
{
    internal sealed class ReadmeRewriterCommandLineParser : IReadmeRewriterCommandLineParser
    {
        static ReadmeRewriterCommandLineParser()
        {
            s_repoUrlOption = DefinedStringOption.CreateRequired("--repo-url","-r");
            s_repoUrlOption.Description = "GitHub or GitLab repository URL (required)";

            s_readmeOption = DefinedStringOption.CreateDefault("--readme", (_) => "README.md");
            s_readmeOption.Description = "Readme relative path. Defaults to README.md";

            s_outputReadmeOption = DefinedStringOption.CreateRequired("--output","-o");
            s_outputReadmeOption.Description = "Output readme path, relative to projectdir or absolute";

            s_root = CreateRootCommand();
        }

        private static readonly RootCommand s_root;

        internal static readonly DefinedStringOption s_repoUrlOption;
        internal static readonly DefinedStringOption s_readmeOption;
        internal static readonly DefinedStringOption s_outputReadmeOption;

        internal static readonly Argument<DirectoryInfo> s_projectDirArg = new Argument<DirectoryInfo>("projectdir")
        {
            Description = "Project directory path. Defaults to current directory",
            DefaultValueFactory = (_) => new DirectoryInfo(Environment.CurrentDirectory)
        }.AcceptExistingOnly();

        internal static readonly Option<string?> s_refOption = new("--ref")
        {
            Description = "Repository ref/commit/branch. Defaults to HEAD commit or 'master'"
        };

        internal static readonly Dictionary<GitRefKind, string[]> s_gitRefKindLookup = new()
        {
            { GitRefKind.TagOrSha, ["tag", "tagorsha", "tag-or-sha"] },
            { GitRefKind.CommitSha, ["commit", "sha", "full"] },
            { GitRefKind.ShortCommitSha, ["short", "short-commit", "shortcommit"] },
            { GitRefKind.BranchName, ["branch", "branchname"] }
        };

        internal static readonly Option<GitRefKind> s_gitRefKindOption = new EnumLookUpOption<GitRefKind>("--gh", s_gitRefKindLookup)
        {
            Description = "Resolve ref using git: tag, commit, short-commit, branch. Defaults to commit SHA",
        }.Build();

        internal static readonly Option<bool> s_errorOnHtmlOption = new("--error-on-html")
        {
            Description = "If set, the presence of HTML tags in the README will cause an error"
        };

        internal static readonly Option<bool> s_removeHtmlOption = new("--remove-html")
        {
            Description = "If set, HTML tags in the README will be removed"
        };

        internal static readonly Option<bool> s_extractDetailsSummaryOption = new("--extract-details-summary")
        {
            Description = "If set, content inside <details> <summary> tags will be extracted"
        };

        internal static readonly Option<string?> s_configOption = new Option<string?>("--config", "-c")
        {
            Description = "Path to JSON remove/replace settings file (see CLI config schema)"
        }.AcceptLegalFilePathsOnly();

        public (IEnumerable<string>? errors, ReadmeRewriterParseResult? result) Parse(IReadOnlyList<string> args)
        {
            ParseResult parseResult = s_root.Parse(args);
            if (parseResult.Errors.Count > 0)
            {
                return (parseResult.Errors.Select(e => e.Message), null);
            }

            return (null, new ReadmeRewriterParseResult(
                parseResult.GetDefinedStringOptionValue(s_repoUrlOption),
                parseResult.GetDefinedStringOptionValue(s_readmeOption),
                parseResult.GetValue(s_configOption),
                parseResult.GetValue(s_refOption),
                parseResult.GetValue(s_gitRefKindOption),
                parseResult.GetValue(s_projectDirArg)!.FullName,
                parseResult.GetDefinedStringOptionValue(s_outputReadmeOption),
                parseResult.GetValue(s_errorOnHtmlOption),
                parseResult.GetValue(s_removeHtmlOption),
                parseResult.GetValue(s_extractDetailsSummaryOption)));
        }

        private static RootCommand CreateRootCommand()
        {
            var root = new RootCommand("ReadmeRewriter CLI")
            {
                s_repoUrlOption,
                s_refOption,
                s_gitRefKindOption,
                s_readmeOption,
                s_configOption,
                s_projectDirArg,
                s_outputReadmeOption,
                s_errorOnHtmlOption,
                s_removeHtmlOption,
                s_extractDetailsSummaryOption
            };
            return root;
        }
    }
}
