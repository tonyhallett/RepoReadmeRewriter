using System.Diagnostics.CodeAnalysis;
using RepoReadmeRewriter.IOWrapper;
using RepoReadmeRewriter.Runner;

namespace ReadmeRewriterCLI
{
    internal sealed class Runner(
        IReadmeRewriterCommandLineParser parser,
        IConsoleWriter consoleWriter,
        IOptionsProvider optionsProvider,
        IReadmeRewriterRunner readmeRewriterRunner,
        IIOHelper ioHelper)
    {
        [ExcludeFromCodeCoverage]
        public Runner() : this(
            new ReadmeRewriterCommandLineParser(),
            ConsoleWriter.Instance,
            new OptionsProvider(
                ConfigFileService.Instance,
                new GitHelper(),
                new RemoveReplaceConfigLoader(
                    IOHelper.Instance,
                    ConfigFileService.Instance,
                    new RemoveReplaceWordsParserWrapper()),
                IOHelper.Instance),
            new ReadmeRewriterRunner(),
            IOHelper.Instance
            )
        {
        }

        public int Run(string[] args)
        {
            (IEnumerable<string>? Errors, ReadmeRewriterParseResult? Result) = parser.Parse(args);
            if (Errors != null)
            {
                foreach (string error in Errors)
                {
                    consoleWriter.WriteErrorLine(error);
                }

                return 1;
            }

            (Options? options, IEnumerable<string>? errors) = optionsProvider.Provide(Result!);

            if (errors != null)
            {
                foreach (string error in errors)
                {
                    consoleWriter.WriteErrorLine(error);
                }

                return 1;
            }

            ReadmeRewriterRunnerResult result = readmeRewriterRunner.Run(
                options!.ProjectDir,
                options!.ReadmeRel,
                options!.RepoUrl,
                options!.RepoRef,
                options!.RewriteTagsOptions,
                options!.RemoveReplaceSettings);

            foreach (string err in result.Errors)
            {
                consoleWriter.WriteErrorLine(err);
            }

            if (!result.Success)
            {
                return 1;
            }

            consoleWriter.WriteLine($"Rewritten to {options.OutputReadme}");
            ioHelper.WriteAllText(options.OutputReadme, result.OutputReadme!);
            return 0;
        }
    }
}
