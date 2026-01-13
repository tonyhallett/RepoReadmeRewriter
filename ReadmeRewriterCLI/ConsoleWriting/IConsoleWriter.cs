using ReadmeRewriterCLI.RunnerOptions.CommandLineParsing.Help;

namespace ReadmeRewriterCLI.ConsoleWriting
{
    internal interface IConsoleWriter
    {
        void WriteLine(string message);

        void WriteErrorLine(string message);

        void WriteWarningLine(string message);
        void WriteHelp(IArgumentsOptionsInfo helpOutput);
    }
}
