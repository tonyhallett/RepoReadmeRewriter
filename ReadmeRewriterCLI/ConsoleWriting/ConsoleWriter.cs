using System.Diagnostics.CodeAnalysis;

namespace ReadmeRewriterCLI.ConsoleWriting
{
    [ExcludeFromCodeCoverage]
    internal sealed class ConsoleWriter : IConsoleWriter
    {
        public static IConsoleWriter Instance { get; } = new ConsoleWriter();

        public void WriteLine(string message) => Console.WriteLine(message);

        public void WriteErrorLine(string message) => Console.Error.WriteLine(message);

        public void WriteWarningLine(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Error.WriteLine(message);
            Console.ResetColor();
        }
    }
}
