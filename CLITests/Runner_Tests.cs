using Moq;
using ReadmeRewriterCLI;
using RepoReadmeRewriter.IOWrapper;
using RepoReadmeRewriter.Runner;

namespace CLITests
{
    internal sealed class Runner_Tests
    {
        [Test]
        public void Should_Write_Errors_And_Exit_1_When_Args_Parsing_Errors()
        {
            string[] args = ["arg1", "arg2"];
            var mockParser = new Mock<IReadmeRewriterCommandLineParser>();
            List<string> errors = ["error1", "error2"];
            mockParser.Setup(m => m.Parse(args)).Returns((errors, null)).Verifiable();
            var mockConsoleWriter = new Mock<IConsoleWriter>();
            var runner = new Runner(
                mockParser.Object,
                mockConsoleWriter.Object,
                Mock.Of<IOptionsProvider>(MockBehavior.Strict),
                Mock.Of<IReadmeRewriterRunner>(MockBehavior.Strict),
                Mock.Of<IIOHelper>(MockBehavior.Strict));

            int exitCode = runner.Run(args);

            Assert.That(exitCode, Is.EqualTo(1));
            mockConsoleWriter.Verify(m => m.WriteErrorLine("error1"), Times.Once);
            mockConsoleWriter.Verify(m => m.WriteErrorLine("error2"), Times.Once);
        }

        [Test]
        public void Should_Write_Errors_And_Exit_1_When_Options_Errors()
        {
            string[] args = ["arg1", "arg2"];
            var mockParser = new Mock<IReadmeRewriterCommandLineParser>();
            ReadmeRewriterParseResult parserResult = new("repourl", "", null, null, "", "", false, false, false);
            mockParser.Setup(m => m.Parse(args)).Returns((null, parserResult)).Verifiable();
            var mockOptionsProvider = new Mock<IOptionsProvider>();
            _ = mockOptionsProvider.Setup(m => m.Provide(parserResult)).Returns((null, ["error1", "error2"]));
            var mockConsoleWriter = new Mock<IConsoleWriter>();
            var runner = new Runner(
                mockParser.Object,
                mockConsoleWriter.Object,
                mockOptionsProvider.Object,
                Mock.Of<IReadmeRewriterRunner>(MockBehavior.Strict),
                Mock.Of<IIOHelper>(MockBehavior.Strict));

            int exitCode = runner.Run(args);

            Assert.That(exitCode, Is.EqualTo(1));
            mockConsoleWriter.Verify(m => m.WriteErrorLine("error1"), Times.Once);
            mockConsoleWriter.Verify(m => m.WriteErrorLine("error2"), Times.Once);
        }

        [Test]
        public void Should_Invoke_The_ReadmeRewriterRunner_With_Options_From_Parsed_Args() => InvokeReadmeRewriterRunner(new ReadmeRewriterRunnerResult());

        private static int InvokeReadmeRewriterRunner(ReadmeRewriterRunnerResult result, IConsoleWriter? consoleWriter = null, IIOHelper? ioHelper = null)
        {
            string[] args = ["arg1", "arg2"];
            var mockParser = new Mock<IReadmeRewriterCommandLineParser>();
            ReadmeRewriterParseResult parserResult = new("repourl", "", null, null, "", "", false, false, false);
            mockParser.Setup(m => m.Parse(args)).Returns((null, parserResult)).Verifiable();
            var mockOptionsProvider = new Mock<IOptionsProvider>();
            var options = new Options("projectdir", "repourl", "reporef", "readmerel", RepoReadmeRewriter.Processing.RewriteTagsOptions.ErrorOnHtml, null, "outputreadme");
            _ = mockOptionsProvider.Setup(m => m.Provide(It.IsAny<ReadmeRewriterParseResult>())).Returns((options, null));
            var mockReadmeRewriterRunner = new Mock<IReadmeRewriterRunner>();
            mockReadmeRewriterRunner.Setup(m => m.Run(options.ProjectDir, options.ReadmeRel, options.RepoUrl, options.RepoRef, options.RewriteTagsOptions, options.RemoveReplaceSettings)).Returns(result).Verifiable();
            var runner = new Runner(
                mockParser.Object,
               consoleWriter ?? Mock.Of<IConsoleWriter>(),
                mockOptionsProvider.Object,
                mockReadmeRewriterRunner.Object,
               ioHelper ?? Mock.Of<IOHelper>());

            int exitCode = runner.Run(args);

            mockReadmeRewriterRunner.VerifyAll();
            return exitCode;
        }

        [Test]
        public void Should_Write_Errors_And_Exit_1_When_ReadmeRewriterRunner_Errors()
        {
            var mockConsoleWriter = new Mock<IConsoleWriter>();
            var result = new ReadmeRewriterRunnerResult();
            result.Errors.Add("error1");
            result.Errors.Add("error2");

            int exitCode = InvokeReadmeRewriterRunner(result, mockConsoleWriter.Object);

            Assert.That(exitCode, Is.EqualTo(1));
            mockConsoleWriter.Verify(m => m.WriteErrorLine("error1"), Times.Once());
            mockConsoleWriter.Verify(m => m.WriteErrorLine("error2"), Times.Once());
        }

        [Test]
        public void Should_Notify_Successful_Rewrite()
        {
            var mockConsoleWriter = new Mock<IConsoleWriter>();
            var result = new ReadmeRewriterRunnerResult
            {
                OutputReadme = "outputreadme"
            };

            _ = InvokeReadmeRewriterRunner(result, mockConsoleWriter.Object);

            mockConsoleWriter.Verify(m => m.WriteLine("Rewritten to outputreadme}"), Times.Once());
            mockConsoleWriter.Verify(m => m.WriteErrorLine("error2"), Times.Never());
        }

        [Test]
        public void Should_Write_Rewritten_To_OutputReadme_Option_Upon_Success()
        {
            var mockIOHelper = new Mock<IIOHelper>();
            var result = new ReadmeRewriterRunnerResult
            {
                OutputReadme = "outputreadme.md"
            };

            _ = InvokeReadmeRewriterRunner(result, null, mockIOHelper.Object);

            mockIOHelper.Verify(m => m.WriteAllText("outputreadme", result.OutputReadme));
        }

        [Test]
        public void Should_Exit_0_Upon_Successful_Rewrite()
        {
            var result = new ReadmeRewriterRunnerResult
            {
                OutputReadme = "outputreadme"
            };

            int exitCode = InvokeReadmeRewriterRunner(result);

            Assert.That(exitCode, Is.EqualTo(0));
        }
    }
}
