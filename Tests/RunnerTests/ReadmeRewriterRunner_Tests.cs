using Moq;
using RepoReadmeRewriter.IOWrapper;
using RepoReadmeRewriter.Messages;
using RepoReadmeRewriter.Processing;
using RepoReadmeRewriter.RemoveReplace.Settings;
using RepoReadmeRewriter.Repo;
using RepoReadmeRewriter.Rewriter;
using RepoReadmeRewriter.Runner;

namespace Tests.RunnerTests
{
    internal sealed class ReadmeRewriterRunner_Tests
    {
        private readonly string _projectDirectoryPath = "projectdirectorypath";

        [TestCase("readmerelative.md")]
        [TestCase(null)]
        public void Should_Error_If_Readme_Does_Not_Exist(string? readmeRelativePath)
        {
            string readmePath = GetExpectedReadmePath(readmeRelativePath);
            IIOHelper ioHelper = MockIOHelper(readmePath, false);

            var mockMessageProvider = new Mock<IMessageProvider>();
            const string readmeDoesNotExistErrorMessage = "readme does not exist";
            _ = mockMessageProvider.Setup(messageProvider => messageProvider.ReadmeFileDoesNotExist(readmePath)).Returns(readmeDoesNotExistErrorMessage);
            var runner = new ReadmeRewriterRunner(ioHelper, new Mock<IRepoReadmeFilePathsProvider>().Object, new Mock<IReadmeRewriter>().Object, mockMessageProvider.Object);

            ReadmeRewriterRunnerResult result = runner.Run(_projectDirectoryPath, readmeRelativePath, "repositoryUrl", "repositoryRef", RewriteTagsOptions.None, null);

            string error = result.Errors.Single();

            Assert.That(error, Is.EqualTo(readmeDoesNotExistErrorMessage));
        }

        private string GetExpectedReadmePath(string? readmeRelativePath) => readmeRelativePath == null
                ? $"{_projectDirectoryPath}/readme.md"
                : $"{_projectDirectoryPath}/{readmeRelativePath}";

        private static IIOHelper MockIOHelper(string expectedReadmePath, bool exists, string readmeContents = "")
        {
            var mockIOHelper = new Mock<IIOHelper>();
            _ = mockIOHelper.Setup(ioHelper => ioHelper.CombinePaths(It.IsAny<string>(), It.IsAny<string>()))
                .Returns<string, string>((path1, path2) => $"{path1}/{path2}");
            _ = mockIOHelper.Setup(ioHelper => ioHelper.FileExists(expectedReadmePath)).Returns(exists);
            _ = mockIOHelper.Setup(ioHelper => ioHelper.ReadAllText(expectedReadmePath)).Returns(readmeContents);
            return mockIOHelper.Object;
        }

        [Test]
        public void Should_Error_If_Null_RepoReadmeFilePaths()
        {
            string expectedReadmePath = GetExpectedReadmePath("readmerelative.md");
            IIOHelper ioHelper = MockIOHelper(expectedReadmePath, true);

            var mockMessageProvider = new Mock<IMessageProvider>();
            _ = mockMessageProvider.Setup(messageProvider => messageProvider.CannotFindGitRepository()).Returns("No git repo");
            var mockRepoReadmeFilePathsProvider = new Mock<IRepoReadmeFilePathsProvider>();

            var runner = new ReadmeRewriterRunner(ioHelper, mockRepoReadmeFilePathsProvider.Object, new Mock<IReadmeRewriter>().Object, mockMessageProvider.Object);

            ReadmeRewriterRunnerResult result = runner.Run(_projectDirectoryPath, "readmerelative.md", "repositoryUrl", "repositoryRef", RewriteTagsOptions.None, null);
            mockRepoReadmeFilePathsProvider.Verify(p => p.Provide(expectedReadmePath));

            string error = result.Errors.Single();

            Assert.That(error, Is.EqualTo("No git repo"));
        }

        [TestCase(null)]
        [TestCase("repositoryRef")]
        public void Should_Rewrite_When_No_Errors_Passing_Run_Args_And_RepoReadmeFilePaths(string? repositoryRef)
        {
            ReadmeRewriterRunnerResult result = RewriteTest(
                repositoryRef ?? "master",
                new ReadmeRewriterResult("rewritten", Enumerable.Empty<string>(), Enumerable.Empty<string>(), false, false));

            Assert.That(result.Success, Is.True);
            Assert.That(result.OutputReadme, Is.EqualTo("rewritten"));
        }

        [Test]
        public void Should_Have_Errors_For_All_Errors_From_ReadmeRewriter()
        {
            var mockMessageProvider = new Mock<IMessageProvider>();
            _ = mockMessageProvider.Setup(messageProvider => messageProvider.UnsupportedImageDomain("unsupportedimagedomain"))
                .Returns("unsupported image domain error");
            _ = mockMessageProvider.Setup(messageProvider => messageProvider.MissingReadmeAsset("missingreadmeasset"))
                .Returns("missing readme asset error");
            _ = mockMessageProvider.Setup(messageProvider => messageProvider.ReadmeHasUnsupportedHTML())
                .Returns("unsupported html error");
            _ = mockMessageProvider.Setup(messageProvider => messageProvider.CouldNotParseRepositoryUrl("repositoryUrl"))
                .Returns("could not parse repository url error");
            ReadmeRewriterRunnerResult result = RewriteTest(
                "repositoryRef",
                new ReadmeRewriterResult(
                    "rewritten", new List<string> { "unsupportedimagedomain" }, new List<string> { "missingreadmeasset" }, true, true),
                mockMessageProvider.Object);
            Assert.That(result.Success, Is.False);
            Assert.That(result.OutputReadme, Is.Null);
            List<string> expectedErrors = new()
            {
                "unsupported image domain error",
                "missing readme asset error",
                "unsupported html error",
                "could not parse repository url error",
            };
            Assert.That(result.Errors, Is.EqualTo(expectedErrors));
        }

        private ReadmeRewriterRunnerResult RewriteTest(string repositoryRef, ReadmeRewriterResult readmeRewriterResult, IMessageProvider? messageProvider = null)
        {
            messageProvider ??= new Mock<IMessageProvider>().Object;
            IIOHelper ioHelper = MockIOHelper(GetExpectedReadmePath("readmerelative.md"), true, "readmecontents");
            var mockRepoReadmeFilePathsProvider = new Mock<IRepoReadmeFilePathsProvider>();
            _ = mockRepoReadmeFilePathsProvider.Setup(repoReadmeFilePathsProvider => repoReadmeFilePathsProvider.Provide(It.IsAny<string>()))
                .Returns(new RepoReadmeFilePaths("repodirpath", "readmedirpath", "reporelreadmepath"));

            const RewriteTagsOptions rewriteTagsOptions = RewriteTagsOptions.ExtractDetailsContentWithoutSummary;
            var removeReplaceSettings = new RemoveReplaceSettings(null, new List<RemovalOrReplacement>(), new List<RemoveReplaceWord>());

            var mockReadmeRewriter = new Mock<IReadmeRewriter>();
            mockReadmeRewriter.Setup(rewriter => rewriter.Rewrite(
                rewriteTagsOptions,
                "readmecontents",
                "reporelreadmepath",
                "repositoryUrl",
                repositoryRef,
                removeReplaceSettings,
                It.Is<ReadmeRelativeFileExists>(readmeRelativeFileExists => readmeRelativeFileExists.RepoDirectoryPath == "repodirpath" && readmeRelativeFileExists.ReadmeDirectoryPath == "readmedirpath")))
                .Returns(readmeRewriterResult).Verifiable();

            var runner = new ReadmeRewriterRunner(ioHelper, mockRepoReadmeFilePathsProvider.Object, mockReadmeRewriter.Object, messageProvider);

            ReadmeRewriterRunnerResult result = runner.Run(
                _projectDirectoryPath,
                "readmerelative.md",
                "repositoryUrl",
                repositoryRef,
                rewriteTagsOptions,
                removeReplaceSettings);

            mockReadmeRewriter.Verify();

            return result;
        }
    }
}
