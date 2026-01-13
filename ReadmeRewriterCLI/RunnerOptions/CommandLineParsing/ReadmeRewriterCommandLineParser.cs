using System.CommandLine;
using ReadmeRewriterCLI.RunnerOptions.CommandLineParsing.Extensions;
using ReadmeRewriterCLI.RunnerOptions.CommandLineParsing.Help;

namespace ReadmeRewriterCLI.RunnerOptions.CommandLineParsing
{
    internal sealed class ReadmeRewriterCommandLineParser : IReadmeRewriterCommandLineParser
    {
        static ReadmeRewriterCommandLineParser()
        {
            s_repoUrlOption = DefinedStringOption.CreateRequired("--repo-url", "-r");
            s_repoUrlOption.Description = "GitHub or GitLab repository URL";

            s_readmeOption = DefinedStringOption.CreateDefault("--readme", (_) => "README.md");
            s_readmeOption.Description = "Readme relative path";

            s_outputReadmeOption = DefinedStringOption.CreateRequired("--output", "-o");
            s_outputReadmeOption.Description = $"Output readme path, relative to {s_projectDirArgumentName} or absolute";
        }
        internal static readonly DefinedStringOption s_repoUrlOption;
        internal static readonly DefinedStringOption s_readmeOption;
        internal static readonly DefinedStringOption s_outputReadmeOption;

        internal static readonly string s_gitRefKindOptionName = "--gh";
        internal static readonly string s_projectDirArgumentName = "projectdir";

        internal static readonly Argument<DirectoryInfo> s_projectDirArg = new HelpArgument<DirectoryInfo>(s_projectDirArgumentName, "Environment.CurrentDirectory")
        {
            Description = "Project directory path.",
            DefaultValueFactory = (_) => new DirectoryInfo(Environment.CurrentDirectory),
        }.AcceptExistingOnly();

        internal static readonly Option<string?> s_refOption = new("--ref")
        {
            Description = $"Repository ref. Alternatively use {s_gitRefKindOptionName}'"
        };

        internal static readonly Dictionary<GitRefKind, string[]> s_gitRefKindLookup = new()
        {
            { GitRefKind.Tag, ["tag"] },
            { GitRefKind.TagOrSha, ["tagorsha", "tag-or-sha"] },
            { GitRefKind.CommitSha, ["commit", "sha", "full"] },
            { GitRefKind.ShortCommitSha, ["short", "short-commit", "shortcommit"] },
            { GitRefKind.BranchName, ["branch", "branchname"] }
        };

        internal static readonly Option<GitRefKind> s_gitRefKindOption = new EnumLookUpOption<GitRefKind>(s_gitRefKindOptionName, s_gitRefKindLookup)
        {
            Description = "Resolve ref using git.",
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
            Description = "Path to JSON remove/replace settings file"
        }.AcceptLegalFilePathsOnly();

        public void SetRefKindAutoBehaviour(string refKindAutoBehaviour)
            => s_gitRefKindOption.Description = $"{s_gitRefKindOption.Description!} Defaults to {refKindAutoBehaviour}";

        public (IEnumerable<string>? errors, ReadmeRewriterParseResult? result, IArgumentsOptionsInfo? helpOutput) Parse(IReadOnlyList<string> args)
        {
            RootCommand rootCommand = new("ReadmeRewriter CLI")
            {
                s_projectDirArg,
                s_repoUrlOption,
                s_outputReadmeOption,
                s_readmeOption,
                s_refOption,
                s_gitRefKindOption,
                s_errorOnHtmlOption,
                s_removeHtmlOption,
                s_extractDetailsSummaryOption,
                 s_configOption
           };
            CommandLineHelpParseResult helpParseResult = CommandLineHelpParser.Parse(rootCommand.RemoveVersionOption(), args);

            ParseResult parseResult = helpParseResult.ParseResult;

            if (parseResult.Errors.Count > 0)
            {
                return (parseResult.Errors.Select(e => e.Message), null, null);
            }

            if (helpParseResult.HelpInvoked)
            {
                return (null, null, new ArgumentsOptionsInfo(parseResult.RootCommandResult.Command));
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
                parseResult.GetValue(s_extractDetailsSummaryOption),
                s_errorOnHtmlOption.Name,
                s_removeHtmlOption.Name
                ), null);
        }
    }
}
