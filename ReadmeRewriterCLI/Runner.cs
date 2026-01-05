using NugetRepoReadme.IOWrapper;
using NugetRepoReadme.Runner;

namespace ReadmeRewriterCLI
{
    internal sealed class Runner(IReadmeRewriterCommandLineParser parser, IConsoleWriter consoleWriter, IOptionsProvider optionsProvider)
    {
        public Runner() : this(
            new ReadmeRewriterCommandLineParser(),
            ConsoleWriter.Instance,
            new OptionsProvider(
                ConsoleWriter.Instance,
                ConfigFileService.Instance,
                new GitHelper(),
                new RemoveReplaceConfigLoader(
                    IOHelper.Instance,
                    ConfigFileService.Instance,
                    new RemoveReplaceWordsParserWrapper())))
        {
        }

        public int Run(string[] args)
        {
            ReadmeRewriterParseResult parseResult = parser.Parse(args);
            if (parseResult.Errors != null)
            {
                foreach (string error in parseResult.Errors)
                {
                    consoleWriter.WriteError(error);
                }

                return 1;
            }

            (Options? options, IEnumerable<string>? errors) = optionsProvider.Provide(parseResult);
            if (options == null)
            {
                return 1;
            }

            if (errors != null)
            {
                foreach (string error in errors)
                {
                    consoleWriter.WriteError(error);
                }

                return 1;
            }

            var runner = new ReadmeRewriterRunner();
            ReadmeRewriterRunnerResult result = runner.Run(
                options.ProjectDir,
                options.ReadmeRel,
                options.RepoUrl,
                options.RepoRef,
                options.RewriteTagsOptions,
                options.RemoveReplaceSettings);

            foreach (string err in result.Errors)
            {
                consoleWriter.WriteError(err);
            }

            if (!result.Success)
            {
                return 1;
            }

            Console.WriteLine(result.OutputReadme);
            return 0;
        }
    }
}
