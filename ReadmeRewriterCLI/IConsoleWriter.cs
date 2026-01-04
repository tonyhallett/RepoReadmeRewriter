namespace ReadmeRewriterCLI
{
    internal interface IConsoleWriter
    {
        void WriteError(string message);

        void WriteWarning(string message);
    }
}
