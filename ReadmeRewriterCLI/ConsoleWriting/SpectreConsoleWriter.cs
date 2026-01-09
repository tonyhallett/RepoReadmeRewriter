using System.Diagnostics.CodeAnalysis;
using System.Text;
using ReadmeRewriterCLI.RunnerOptions.CommandLineParsing.Help;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace ReadmeRewriterCLI.ConsoleWriting
{
    [ExcludeFromCodeCoverage]
    internal sealed class SpectreConsoleWriter : IConsoleWriter
    {
        private readonly IAnsiConsole _ansiConsole;
        private SpectreConsoleWriter(IAnsiConsole ansiConsole) => _ansiConsole = ansiConsole;
        private static IConsoleWriter? s_instance;
        public static IConsoleWriter Instance()
        {
            if (s_instance != null)
            {
                return s_instance;
            }

            Console.OutputEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
            s_instance = new SpectreConsoleWriter(AnsiConsole.Console);

            return s_instance;
        }

        public void WriteLine(string message) => _ansiConsole.WriteLine(message);

        public void WriteErrorLine(string message) => _ansiConsole.MarkupLine($"[red]{message}[/]");

        public void WriteWarningLine(string message) => _ansiConsole.MarkupLine($"[yellow]{message}[/]");

        private void WriteCapabilities()
        {
            Capabilities capabilities = _ansiConsole.Profile.Capabilities;
            _ansiConsole.WriteLine($"Legacy - {capabilities.Legacy}");
            _ansiConsole.WriteLine($"Unicode - {capabilities.Unicode}");
            _ansiConsole.WriteLine($"Ansi - {capabilities.Ansi}");
            _ansiConsole.WriteLine($"ColorSystem - {capabilities.ColorSystem}");
            _ansiConsole.WriteLine($"Links - {capabilities.Links}");
        }

        public void WriteHelp(IArgumentsOptionsInfo helpOutput)
        {
            WriteDescription();
            WriteUsage();
            WriteArguments();
            WriteOptions();

            void WriteDescription() =>  _ansiConsole.Write(
                CreateRowsSectionPanel(
                    "About",
                    [
                    "A CLI tool to help you rewrite your GitHub or GitLab relative README assets to absolute.",
                    "And more...."
                    ]));

            void WriteUsage() => WriteHeader("Usage");

            void WriteArguments() => WriteHeader($"Arguments - {helpOutput.Arguments.Count}");

            void WriteOptions() => WriteHeader($"Options - {helpOutput.Options.Count}");

            void WriteHeader(string header) => _ansiConsole.MarkupLine($"[bold green]{header}:[/]");

            Panel CreateRowsSectionPanel(string header, IEnumerable<string> lines) => CreateSectionPanel(header, new Rows(lines.Select(l => new Markup(l))));

            Panel CreateSectionPanel(string header, IRenderable content) => new Panel(content).Header($"[bold green]{header}[/]").BorderColor(Color.Green).Expand();
        }
    }
}
