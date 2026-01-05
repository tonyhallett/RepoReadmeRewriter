using System.CommandLine;
using System.CommandLine.Parsing;

namespace ReadmeRewriterCLI
{
    internal sealed class ReadmeRewriterCommandLineParser : IReadmeRewriterCommandLineParser
    {
        public ReadmeRewriterParseResult Parse(string[] args)
        {
            var repoUrlOption = new Option<string>(
    name: "--repo-url"
)
            {
                Description = "GitHub or GitLab repository URL (required)",
                Required = true

            };

            // but might want to look for tags
            var refOption = new Option<string?>(
                name: "--ref"
            )
            {
                Description = "Repository ref/commit/branch. Defaults to HEAD commit or 'master'"
            };

            var readmeOption = new Option<string?>(
                name: "--readme"
                )
            {
                Description = "Readme relative path. Defaults to README.md"
            };

            var rewriteTagsOption = new Option<string?>(
                name: "--rewrite-tags"
            )
            {
                Description = "RewriteTagsOptions enum value (None|RewriteBrTags|RewriteAll|ErrorOnHtml|RemoveHtml|ExtractDetailsContentWithoutSummary)"
            };

            var configOption = new Option<string?>(
                name: "--config"
                )
            {
                Description = "Path to JSON remove/replace settings file (see CLI config schema)"
            };

            var root = new RootCommand("ReadmeRewriter CLI")
        {
            repoUrlOption,
            refOption,
            readmeOption,
            rewriteTagsOption,
            configOption,
        };
            ParseResult parseResult = CommandLineParser.Parse(root, args);
            if (parseResult.Errors.Count > 0)
            {
                return new ReadmeRewriterParseResult(parseResult.Errors.Select(e => e.Message));
            }

            string repoUrl = parseResult.GetValue(repoUrlOption)!;
            string? repoRef = parseResult.GetValue(refOption);
            string? readmeRel = parseResult.GetValue(readmeOption);
            string? rewriteTags = parseResult.GetValue(rewriteTagsOption);
            string? configPath = parseResult.GetValue(configOption);
            return new ReadmeRewriterParseResult(repoUrl, readmeRel, rewriteTags, configPath, repoRef);
            //probably add project directory
        }
    }
}
