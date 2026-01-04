namespace ReadmeRewriterCLI
{
    internal class ConsoleWriter : IConsoleWriter
    {
        public static IConsoleWriter Instance { get; } = new ConsoleWriter();

        public void WriteError(string message) => Console.Error.WriteLine(message);

        public void WriteWarning(string message) => Console.WriteLine(message);
    }
}
