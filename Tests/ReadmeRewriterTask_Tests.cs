using Microsoft.Build.Framework;
using Moq;
using MSBuildTaskTestHelpers;
using NugetRepoReadme;
using NugetRepoReadme.RemoveReplace;
using RepoReadmeRewriter.Processing;
using RepoReadmeRewriter.RemoveReplace.Settings;
using RepoReadmeRewriter.Runner;
using Tests.Utils;

namespace Tests
{
    internal sealed class ReadmeRewriterTask_Tests
    {
        private const string RepositoryUrl = "repositoryurl";
        private const string ProjectDirectoryPath = "projectdir";
        private const string ReadmeRelativePath = "readmerelativepath";

        private const string RemoveCommentIdentifiers = "removeCommentIdentifiers";
        private readonly ITaskItem[] _removeReplaceTaskItems = [new Mock<ITaskItem>().Object];
        private readonly ITaskItem[] _removeReplaceWordsTaskItems = [new Mock<ITaskItem>().Object];
        private Mock<IRemoveReplaceSettingsProvider> _mockRemoveReplaceSettingsProvider = new();

        private DummyLogBuildEngine _dummyLogBuildEngine = new();
        private Mock<IReadmeRewriterRunner> _mockRunner;
        private ReadmeRewriterTask _readmeRewriterTask = new();
        private TestRemoveReplaceSettingsResult _removeReplaceSettingsResult = new();

        private sealed class TestRemoveReplaceSettingsResult : IRemoveReplaceSettingsResult
        {
            public IReadOnlyList<string> Errors { get; set; } = [];

            public RemoveReplaceSettings? Settings { get; } = new RemoveReplaceSettings(null, [], []);
        }

        [SetUp]
        public void Setup()
        {
            _mockRemoveReplaceSettingsProvider = new Mock<IRemoveReplaceSettingsProvider>();
            _dummyLogBuildEngine = new DummyLogBuildEngine();
            _mockRunner = new Mock<IReadmeRewriterRunner>();
            _readmeRewriterTask = new ReadmeRewriterTask
            {
                BuildEngine = _dummyLogBuildEngine,
                RemoveReplaceSettingsProvider = _mockRemoveReplaceSettingsProvider.Object,
                MessageProvider = new ConcatenatingArgumentsMessageProvider(),
                RepositoryUrl = RepositoryUrl,
                ProjectDirectoryPath = ProjectDirectoryPath,
                ReadmeRelativePath = ReadmeRelativePath,
                Runner = _mockRunner.Object,
            };
            _removeReplaceSettingsResult = new TestRemoveReplaceSettingsResult();
            _ = _mockRemoveReplaceSettingsProvider.Setup(removeReplaceSettingsProvider => removeReplaceSettingsProvider.Provide(_removeReplaceTaskItems, _removeReplaceWordsTaskItems, RemoveCommentIdentifiers))
                .Returns(_removeReplaceSettingsResult);
            _readmeRewriterTask.RemoveReplaceItems = _removeReplaceTaskItems;
            _readmeRewriterTask.RemoveReplaceWordsItems = _removeReplaceWordsTaskItems;
            _readmeRewriterTask.RemoveCommentIdentifiers = RemoveCommentIdentifiers;
        }

        [Test]
        public void Should_Log_Errors_From_RemoveReplaceSettingsProvider()
        {
            _ = _mockRemoveReplaceSettingsProvider.Setup(removeReplaceSettingsProvider => removeReplaceSettingsProvider.Provide(
                It.IsAny<ITaskItem[]?>(), It.IsAny<ITaskItem[]?>(), It.IsAny<string?>()))
                .Returns(new TestRemoveReplaceSettingsResult
                {
                    Errors = ["error1"],
                });
            bool executeResult = _readmeRewriterTask.Execute();

            Assert.That(_dummyLogBuildEngine.SingleErrorMessage(), Is.EqualTo("error1"));
        }

        [Test]
        public void Should_Invoke_The_Runner_With_The_RemoveReplaceSettings_From_The_Provider()
        {
            _mockRunner.Setup(
                runner => runner.Run(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<RewriteTagsOptions>(),
                _removeReplaceSettingsResult.Settings)).Returns(new ReadmeRewriterRunnerResult()).Verifiable();

            _ = _readmeRewriterTask.Execute();

            _mockRunner.Verify();
        }

        [Test]
        public void Should_Invoke_The_Runner_With_Task_Properties_ProjectDirectoryPath_ReadmeRelativePath()
        {
            _mockRunner.Setup(
                runner => runner.Run(
                ProjectDirectoryPath,
                ReadmeRelativePath,
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<RewriteTagsOptions>(),
                It.IsAny<RemoveReplaceSettings>())).Returns(new ReadmeRewriterRunnerResult()).Verifiable();

            _ = _readmeRewriterTask.Execute();

            _mockRunner.Verify();
        }

        [TestCase(null)]
        [TestCase("readmeRepositoryUrl")]
        public void Should_Invoke_The_Runner_With_One_Of_Task_Properties_ReadmeRepositoryUrl_Or_RepositoryUrl(string? readmeRepositoryUrl)
        {
            _readmeRewriterTask.ReadmeRepositoryUrl = readmeRepositoryUrl;

            _ = _mockRunner.Setup(
                runner => runner.Run(
                It.IsAny<string>(),
                It.IsAny<string>(),
                readmeRepositoryUrl ?? RepositoryUrl,
                It.IsAny<string>(),
                It.IsAny<RewriteTagsOptions>(),
                It.IsAny<RemoveReplaceSettings>())).Returns(new ReadmeRewriterRunnerResult());

            _ = _readmeRewriterTask.Execute();

            _mockRunner.Verify();

        }

        [Test]
        public void Should_Prefer_RepositoryRef_For_Ref()
    => RefTest("repositoryRef", "repositoryCommit", "repositoryBranch", "repositoryRef");

        [Test]
        public void Should_Prefer_RepositoryCommit_For_Ref_If_RepositoryRef_Null()
            => RefTest(null, "repositoryCommit", "repositoryBranch", "repositoryCommit");

        [Test]
        public void Should_Prefer_RepositoryBranch_For_Ref_If_RepositoryRef_And_RepositoryCommit_Null()
            => RefTest(null, null, "repositoryBranch", "repositoryBranch");

        [Test]
        public void Should_Default_Ref_To_Master_If_RepositoryRef_RepositoryCommit_And_RepositoryBranch_Null()
            => RefTest(null, null, null, "master");

        private void RefTest(string? repositoryRef, string? repositoryCommit, string? repositoryBranch, string expectedRef)
        {
            _readmeRewriterTask.RepositoryRef = repositoryRef;
            _readmeRewriterTask.RepositoryCommit = repositoryCommit;
            _readmeRewriterTask.RepositoryBranch = repositoryBranch;

            _ = _mockRunner.Setup(
                runner => runner.Run(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                expectedRef,
                It.IsAny<RewriteTagsOptions>(),
                It.IsAny<RemoveReplaceSettings>())).Returns(new ReadmeRewriterRunnerResult());

            _ = _readmeRewriterTask.Execute();

            _mockRunner.Verify();
        }

        /*
    Not advertising the use of both RewriteTagsOptions.  Needs more consideration.
*/

        [TestCase(RewriteTagsOptions.RewriteBrTags)]
        [TestCase(RewriteTagsOptions.RewriteAll)]
        [TestCase(RewriteTagsOptions.None)]
        public void Should_Parse_RewriteTagsOptions_Property_When_Provided(RewriteTagsOptions rewriteTagsOptions)
        {
            _readmeRewriterTask.RewriteTagsOptions = rewriteTagsOptions.ToString();

            RewriteTagsOptions_Test(rewriteTagsOptions);
        }

        [Test]
        public void Should_Log_Warning_When_RewriteTagsOptions_Property_Is_Malformed_And_Use_None()
        {
            _readmeRewriterTask.RewriteTagsOptions = "malformed";
            RewriteTagsOptions_Test(RewriteTagsOptions.None);
            Assert.That(
                    _dummyLogBuildEngine.SingleWarningMessage(),
                    Is.EqualTo("malformedNone"));
        }

        [Test]
        public void Should_Have_RewriteTagsOptions_None_When_Enum_Not_Set_And_No_Bool_Properties() => RewriteTagsOptions_Test(RewriteTagsOptions.None);

        [TestCase("true", RewriteTagsOptions.ErrorOnHtml)]
        [TestCase("True", RewriteTagsOptions.ErrorOnHtml)]
        [TestCase("false", RewriteTagsOptions.None)]
        [TestCase("malformed", RewriteTagsOptions.None)]
        [TestCase(null, RewriteTagsOptions.None)]
        public void Should_Have_RewriteTagsOptions_ErrorOnHtml_When_No_Enum_And_ErrorOnHtml(string? errorOnHtml, RewriteTagsOptions expectedRewriteTagsOptions)
        {
            _readmeRewriterTask.ErrorOnHtml = errorOnHtml;
            RewriteTagsOptions_Test(expectedRewriteTagsOptions);
        }

        [Test]
        public void Should_Have_RewriteTagsOptions_RemoveHtml_When_No_Enum_And_RemoveHtml_And_Not_ErrorOnHtml()
        {
            _readmeRewriterTask.RemoveHtml = "true";

            RewriteTagsOptions_Test(RewriteTagsOptions.RemoveHtml);
        }

        [Test]
        public void Should_Ignore_RemoveHtml_When_ErrorOnHtml()
        {
            _readmeRewriterTask.RemoveHtml = "true";
            _readmeRewriterTask.ErrorOnHtml = "true";

            RewriteTagsOptions_Test(RewriteTagsOptions.ErrorOnHtml);
        }

        [Test]
        public void Should_Add_Flag_ExtractDetailsContentWithoutSummary_When_Property_Is_True()
        {
            _readmeRewriterTask.ErrorOnHtml = "true";
            _readmeRewriterTask.ExtractDetailsContentWithoutSummary = "true";

            RewriteTagsOptions_Test(RewriteTagsOptions.ErrorOnHtml | RewriteTagsOptions.ExtractDetailsContentWithoutSummary);
        }

        private void RewriteTagsOptions_Test(RewriteTagsOptions expectedRewriteTagsOptions)
        {
            _ = _mockRunner.Setup(
                runner => runner.Run(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                expectedRewriteTagsOptions,
                It.IsAny<RemoveReplaceSettings>())).Returns(new ReadmeRewriterRunnerResult());

            _ = _readmeRewriterTask.Execute();

            _mockRunner.Verify();
        }

        [Test]
        public void Should_Write_Rewritten_Readme_To_OutputReadme_When_Runner_Success()
        {
            _ = _mockRunner.Setup(
                runner => runner.Run(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<RewriteTagsOptions>(),
                _removeReplaceSettingsResult.Settings)).Returns(new ReadmeRewriterRunnerResult { OutputReadme = "output" });

            bool taskResult = _readmeRewriterTask.Execute();

            Assert.Multiple(() =>
            {
                Assert.That(taskResult, Is.EqualTo(true));
                Assert.That(_readmeRewriterTask.OutputReadme, Is.EqualTo("output"));
            });
        }

        [Test]
        public void Should_Log_All_Errors_From_Runner()
        {
            var runnerResult = new ReadmeRewriterRunnerResult();
            runnerResult.Errors.Add("error");
            _ = _mockRunner.Setup(
               runner => runner.Run(
               It.IsAny<string>(),
               It.IsAny<string>(),
               It.IsAny<string>(),
               It.IsAny<string>(),
               It.IsAny<RewriteTagsOptions>(),
               _removeReplaceSettingsResult.Settings)).Returns(runnerResult);

            bool taskResult = _readmeRewriterTask.Execute();

            Assert.Multiple(() =>
            {
                Assert.That(taskResult, Is.EqualTo(false));
                Assert.That(_readmeRewriterTask.OutputReadme, Is.Empty);
                Assert.That(_dummyLogBuildEngine.SingleErrorMessage(), Is.EqualTo("error"));
            });
        }
    }
}
