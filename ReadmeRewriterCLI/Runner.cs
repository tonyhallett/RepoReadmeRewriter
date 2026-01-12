using System.Diagnostics.CodeAnalysis;
using ReadmeRewriterCLI.ConsoleWriting;
using ReadmeRewriterCLI.RunnerOptions;
using ReadmeRewriterCLI.RunnerOptions.CommandLineParsing;
using ReadmeRewriterCLI.RunnerOptions.CommandLineParsing.Help;
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
            SpectreConsoleWriter.Instance(),
            new OptionsProvider(),
            new ReadmeRewriterRunner(),
            IOHelper.Instance
            )
        {
        }

        public int Run(string[] args)
        {
            parser.SetRefKindAutoBehaviour(optionsProvider.RefKindAutoBehaviour);
            (IEnumerable<string>? Errors, ReadmeRewriterParseResult? Result, IArgumentsOptionsInfo? helpOutput) = parser.Parse(args);
            if (Errors != null)
            {
                foreach (string error in Errors)
                {
                    consoleWriter.WriteErrorLine(error);
                }

                return 1;
            }

            if (helpOutput != null)
            {
                consoleWriter.WriteHelp(helpOutput);
                return 0;
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
