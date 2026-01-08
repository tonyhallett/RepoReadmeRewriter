using System.Diagnostics.CodeAnalysis;
using ReadmeRewriterCLI.RunnerOptions.CommandLineParsing.Help;
using Spectre.Console;

namespace ReadmeRewriterCLI.ConsoleWriting
{
    [ExcludeFromCodeCoverage]
    internal sealed class SpectreConsoleWriter : IConsoleWriter
    {
        private readonly IAnsiConsole _ansiConsole;
        private SpectreConsoleWriter(IAnsiConsole ansiConsole) => _ansiConsole = ansiConsole;

        public static IConsoleWriter Instance { get; } = new SpectreConsoleWriter(AnsiConsole.Console);

        public void WriteLine(string message) => _ansiConsole.WriteLine(message);

        public void WriteErrorLine(string message) => _ansiConsole.MarkupLine($"[red]{message}[/]");

        public void WriteWarningLine(string message) => _ansiConsole.MarkupLine($"[yellow]{message}[/]");

        public void WriteHelp(IArgumentsOptionsInfo helpOutput)
        {
            WriteDescription();
            WriteUsage();
            WriteArguments();
            WriteOptions();

            void WriteDescription()
            {
                  WriteHeader("ReameMeRewriter");
                _ansiConsole.WriteLine();
                _ansiConsole.MarkupLine("[green]A CLI tool to help you rewrite your GitHub or GitLab relative README assets to absolute.  And more.[/]");
                _ansiConsole.WriteLine();
            }

            void WriteUsage() => WriteHeader("Usage");

            void WriteArguments() => WriteHeader($"Arguments - {helpOutput.Arguments.Count}");

            void WriteOptions() => WriteHeader($"Options - {helpOutput.Options.Count}");

            void WriteHeader(string header) => _ansiConsole.MarkupLine($"[bold green]{header}:[/]");
        }
    }
}
