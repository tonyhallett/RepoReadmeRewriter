namespace ReadmeRewriterCLI.ConsoleWriting
{
    internal interface IConsoleWriter
    {
        void WriteLine(string message);

        void WriteErrorLine(string message);

        void WriteWarningLine(string message);
    }
}
