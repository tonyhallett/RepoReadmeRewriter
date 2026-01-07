using System.CommandLine;
using ReadmeRewriterCLI.RunnerOptions.CommandLineParsing.Extensions;

namespace ReadmeRewriterCLI.RunnerOptions.CommandLineParsing
{
    internal sealed class ReadmeRewriterCommandLineParser : IReadmeRewriterCommandLineParser
    {
        static ReadmeRewriterCommandLineParser()
        {
            s_repoUrlOption = DefinedStringOption.CreateRequired("--repo-url");
            s_repoUrlOption.Description = "GitHub or GitLab repository URL (required)";
            s_readmeOption = DefinedStringOption.CreateDefault("--readme", (_) => "README.md");
            s_readmeOption.Description = "Readme relative path. Defaults to README.md";
            s_outputReadmeOption = DefinedStringOption.CreateRequired("--output");
            s_outputReadmeOption.Description = "Output readme path, relative to projectdir or absolute";
            s_root = CreateRootCommand();
        }

        private static readonly RootCommand s_root;

        internal static readonly DefinedStringOption s_repoUrlOption;
        internal static readonly DefinedStringOption s_readmeOption;
        internal static readonly DefinedStringOption s_outputReadmeOption;

        internal static readonly Option<string?> s_refOption = new("--ref")
        {
            Description = "Repository ref/commit/branch. Defaults to HEAD commit or 'master'"
        };

        internal static readonly Argument<string> s_projectDirArg = new Argument<string>("projectdir")
        {
            Description = "Project directory path. Defaults to current directory",
            DefaultValueFactory = (_) => Environment.CurrentDirectory,
        }.AcceptLegalFilePathsOnly();

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

        internal static readonly Option<string?> s_configOption = new("--config")
        {
            Description = "Path to JSON remove/replace settings file (see CLI config schema)"
        };



        public (IEnumerable<string>? errors, ReadmeRewriterParseResult? result) Parse(IReadOnlyList<string> args)
        {
            ParseResult parseResult = s_root.Parse(args);
            if (parseResult.Errors.Count > 0)
            {
                return (parseResult.Errors.Select(e => e.Message), null);
            }

            string repoUrl = parseResult.GetRequiredStringValue(s_repoUrlOption)!;
            string readmeRel = parseResult.GetRequiredStringValue(s_readmeOption)!;
            string outputReadme = parseResult.GetRequiredStringValue(s_outputReadmeOption)!;
            string? repoRef = parseResult.GetValue(s_refOption);

            bool errorOnHtml = parseResult.GetValue(s_errorOnHtmlOption);
            bool removeHtml = parseResult.GetValue(s_removeHtmlOption);
            bool extractDetailsSummary = parseResult.GetValue(s_extractDetailsSummaryOption);

            string? configPath = parseResult.GetValue(s_configOption);
            string projectDir = parseResult.GetValue(s_projectDirArg)!;

            return (null, new ReadmeRewriterParseResult(
                repoUrl,
                readmeRel,
                configPath,
                repoRef,
                projectDir,
                outputReadme,
                errorOnHtml,
                removeHtml,
                extractDetailsSummary));
        }

        private static RootCommand CreateRootCommand()
        {
            var root = new RootCommand("ReadmeRewriter CLI")
            {
                s_repoUrlOption,
                s_refOption,
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
