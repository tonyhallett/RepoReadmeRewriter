using Moq;
using ReadmeRewriterCLI.RunnerOptions;
using ReadmeRewriterCLI.RunnerOptions.CommandLineParsing;
using ReadmeRewriterCLI.RunnerOptions.Config;
using ReadmeRewriterCLI.RunnerOptions.Git;
using RepoReadmeRewriter.IOWrapper;
using RepoReadmeRewriter.Processing;
using RepoReadmeRewriter.RemoveReplace.Settings;

namespace CLITests
{
    internal sealed class OptionsProvider_Tests
    {
        [Test]
        public void Should_Error_When_ProjectDir_Does_Not_Exist()
        {
            string nonExistentDir = "nonexistentdir";
            var mockIOHelper = new Mock<IIOHelper>();
            mockIOHelper.Setup(m => m.DirectoryExists("nonexistentdir")).Returns(false).Verifiable();
            var optionsProvider = new OptionsProvider(
                new Mock<IConfigFileService>().Object,
                new Mock<IGitHelper>().Object,
                new Mock<IRemoveReplaceConfigLoader>().Object,
                mockIOHelper.Object);
            (Options? options, IEnumerable<string>? errors) = optionsProvider.Provide(
                new ReadmeRewriterParseResult(
                    "http:www.example.com/repo.git",
                    "README.md",
                    null,
                    null,
                    GitRefKind.Auto,
                    nonExistentDir,
                    "output/README.md",
                    false,
                    false,
                    false,
                    "",
                    ""));

            mockIOHelper.VerifyAll();
            Assert.Multiple(() =>
            {
                Assert.That(options, Is.Null);
                Assert.That(errors!.Single(), Is.EqualTo($"Project directory does not exist: {nonExistentDir}"));
            });
        }

        private static Mock<IIOHelper> SetupProjectDirExists()
        {
            var mockIOHelper = new Mock<IIOHelper>();
            _ = mockIOHelper.Setup(m => m.DirectoryExists(It.IsAny<string>())).Returns(true);
            return mockIOHelper;
        }

        [Test]
        public void Should_Return_Error_When_Both_RemoveHtml_And_ErrorOnHtml_Are_Set()
        {
            var optionsProvider = new OptionsProvider(
                new Mock<IConfigFileService>().Object,
                new Mock<IGitHelper>().Object,
                new Mock<IRemoveReplaceConfigLoader>().Object,
                SetupProjectDirExists().Object);
            (Options? options, IEnumerable<string>? errors) = optionsProvider.Provide(
                new ReadmeRewriterParseResult(
                    "http:www.example.com/repo.git",
                    "README.md",
                    null,
                    "reporef",
                    GitRefKind.Auto,
                    Environment.CurrentDirectory,
                    "output/README.md",
                    true,
                    true,
                    false,
                    "errorName",
                    "removeName"));
            Assert.Multiple(() =>
            {
                Assert.That(options, Is.Null);
                Assert.That(errors!.Single(), Is.EqualTo("Cannot set both errorName and removeName to true"));
            });
        }

        [TestCase(false, false, false, RewriteTagsOptions.None)]
        [TestCase(true, false, false, RewriteTagsOptions.ErrorOnHtml)]
        [TestCase(false, true, false, RewriteTagsOptions.RemoveHtml)]
        [TestCase(false, false, true, RewriteTagsOptions.ExtractDetailsContentWithoutSummary)]
        [TestCase(false, true, true, RewriteTagsOptions.ExtractDetailsContentWithoutSummary | RewriteTagsOptions.RemoveHtml)]
        public void Should_Create_Correct_RewriteTagsOptions_From_ParseResult(
            bool errorOnHtml,
            bool removeHtml,
            bool extractDetailsSummary,
            RewriteTagsOptions expectedRewriteTagsOptions)
        {
            var optionsProvider = new OptionsProvider(
                new Mock<IConfigFileService>().Object,
                new Mock<IGitHelper>().Object,
                new Mock<IRemoveReplaceConfigLoader>().Object,
                SetupProjectDirExists().Object);
            (Options? options, IEnumerable<string>? errors) = optionsProvider.Provide(
                new ReadmeRewriterParseResult(
                    "http:www.example.com/repo.git",
                    "README.md",
                    null,
                    "reporef",
                    GitRefKind.Auto,
                    Environment.CurrentDirectory,
                    "output/README.md",
                    errorOnHtml,
                    removeHtml,
                    extractDetailsSummary,
                    "",
                    ""));

            Assert.Multiple(() =>
            {
                Assert.That(errors, Is.Null);
                Assert.That(options!.RewriteTagsOptions, Is.EqualTo(expectedRewriteTagsOptions));
            });
        }

        [Test]
        public void Should_Have_Absolute_OutputReadme_Relative_To_ProjectDir()
        {
            string projectDir = "projectDir";
            string outputReadme = "output/README.md";
            Mock<IIOHelper> mockIOHelper = SetupProjectDirExists();
            _ = mockIOHelper.Setup(m => m.EnsureAbsolute(projectDir, outputReadme)).Returns("absolute");

            var optionsProvider = new OptionsProvider(
                new Mock<IConfigFileService>().Object,
                new Mock<IGitHelper>().Object,
                new Mock<IRemoveReplaceConfigLoader>().Object,
                mockIOHelper.Object);
            (Options? options, IEnumerable<string>? errors) = optionsProvider.Provide(
                new ReadmeRewriterParseResult(
                    "http:www.example.com/repo.git",
                    "README.md",
                    null,
                    "reporef",
                    GitRefKind.Auto,
                    projectDir,
                    outputReadme,
                    false,
                    false,
                    false,
                    "",
                    ""));

            Assert.Multiple(() =>
            {
                Assert.That(errors, Is.Null);
                Assert.That(options!.OutputReadme, Is.EqualTo("absolute"));
            });
        }

        [Test]
        public void Should_Have_Null_RemoveReplaceSettings_If_No_ConfigPath_Provided()
        {
            var optionsProvider = new OptionsProvider(
                new Mock<IConfigFileService>().Object,
                new Mock<IGitHelper>().Object,
                new Mock<IRemoveReplaceConfigLoader>().Object,
                SetupProjectDirExists().Object);

            (Options? options, IEnumerable<string>? errors) = optionsProvider.Provide(
                new ReadmeRewriterParseResult(
                    "http:www.example.com/repo.git",
                    "README.md",
                    null,
                    "reporef",
                    GitRefKind.Auto,
                    Environment.CurrentDirectory,
                    "output/README.md",
                    false,
                    false,
                    false,
                    "",
                    ""));

            Assert.Multiple(() =>
            {
                Assert.That(errors, Is.Null);
                Assert.That(options!.RemoveReplaceSettings, Is.Null);
            });
        }

        [Test]
        public void Should_Error_If_No_Absolute_Config_Path()
        {
            string projectDir = "projectDir";
            string configPath = "configPath";
            var mockConfigFileService = new Mock<IConfigFileService>();

            var optionsProvider = new OptionsProvider(
                mockConfigFileService.Object,
                new Mock<IGitHelper>().Object,
                new Mock<IRemoveReplaceConfigLoader>().Object,
                SetupProjectDirExists().Object);

            (Options? options, IEnumerable<string>? errors) result = optionsProvider.Provide(
                new ReadmeRewriterParseResult(
                    "http:www.example.com/repo.git",
                    "README.md",
                    configPath,
                    "reporef",
                    GitRefKind.Auto,
                    projectDir,
                    "output/README.md",
                    false,
                    false,
                    false,
                    "",
                    ""));

            mockConfigFileService.Verify(m => m.GetConfigPath(projectDir, configPath));
            AssertError(result, $"Config file not found: {configPath}");
        }

        [Test]
        public void Should_Have_Errors_From_RemoveReplaceConfigLoader()
        {
            var mockConfigFileService = new Mock<IConfigFileService>();
            _ = mockConfigFileService.Setup(m => m.GetConfigPath(It.IsAny<string>(), It.IsAny<string>())).Returns("absoluteconfigpath");

            var mockRemoveReplaceConfigLoader = new Mock<IRemoveReplaceConfigLoader>();
            List<string> configLoaderErrors = ["error1", "error2"];
            _ = mockRemoveReplaceConfigLoader.Setup(m => m.Load("absoluteconfigpath", out configLoaderErrors)).Returns((RemoveReplaceSettings?)null);
            var optionsProvider = new OptionsProvider(
                mockConfigFileService.Object,
                new Mock<IGitHelper>().Object,
               mockRemoveReplaceConfigLoader.Object,
                SetupProjectDirExists().Object);

            (Options? options, IEnumerable<string>? errors) = optionsProvider.Provide(
                new ReadmeRewriterParseResult(
                    "http:www.example.com/repo.git",
                    "README.md",
                    "configPath",
                    "reporef",
                    GitRefKind.Auto,
                    "projectDir",
                    "output/README.md",
                    false,
                    false,
                    false,
                    "",
                    ""));

            Assert.Multiple(() =>
            {
                Assert.That(options, Is.Null);
                Assert.That(errors, Is.EquivalentTo(configLoaderErrors));
            });
        }

        [Test]
        public void Should_Have_RemoveReplaceSettings_From_The_Loader()
        {
            var mockConfigFileService = new Mock<IConfigFileService>();
            _ = mockConfigFileService.Setup(m => m.GetConfigPath(It.IsAny<string>(), It.IsAny<string>())).Returns("absoluteconfigpath");

            var mockRemoveReplaceConfigLoader = new Mock<IRemoveReplaceConfigLoader>();
            List<string> configLoaderErrors = [];
            var removeReplaceSettings = new RemoveReplaceSettings(null, [], []);
            _ = mockRemoveReplaceConfigLoader.Setup(m => m.Load("absoluteconfigpath", out configLoaderErrors)).Returns(removeReplaceSettings);
            var optionsProvider = new OptionsProvider(
                mockConfigFileService.Object,
                new Mock<IGitHelper>().Object,
               mockRemoveReplaceConfigLoader.Object,
                SetupProjectDirExists().Object);

            (Options? options, IEnumerable<string>? errors) = optionsProvider.Provide(
                new ReadmeRewriterParseResult(
                    "http:www.example.com/repo.git",
                    "README.md",
                    "configPath",
                    "reporef",
                    GitRefKind.Auto,
                    "projectDir",
                    "output/README.md",
                    false,
                    false,
                    false,
                    "",
                    ""));

            Assert.Multiple(() =>
            {
                Assert.That(options!.RemoveReplaceSettings, Is.SameAs(removeReplaceSettings));
                Assert.That(errors, Is.Null);
            });
        }

        private static void AssertError((Options? options, IEnumerable<string>? errors) result, string expectedError) => Assert.Multiple(() =>
            {
                Assert.That(result.errors!.Single(), Is.EqualTo(expectedError));
                Assert.That(result.options, Is.Null);
            });

        [Test]
        public void Should_Return_The_RepoRef_If_Provided()
        {
            var optionsProvider = new OptionsProvider(new Mock<IConfigFileService>().Object, new Mock<IGitHelper>().Object, new Mock<IRemoveReplaceConfigLoader>().Object, SetupProjectDirExists().Object);
            (Options? options, IEnumerable<string>? errors) = optionsProvider.Provide(
                new ReadmeRewriterParseResult(
                    "http:www.example.com/repo.git",
                    "README.md",
                    null,
                    "reporef",
                    GitRefKind.Auto,
                    Environment.CurrentDirectory,
                    "output/README.md",
                    false,
                    false,
                    false,
                    "",
                    ""));
            Assert.Multiple(() =>
            {
                Assert.That(errors, Is.Null);
                Assert.That(options!.RepoRef, Is.SameAs("reporef"));
            });
        }

        [TestCaseSource(nameof(AllGitRefKinds))]
        public void Should_Use_Selected_Git_Helper_Method_When_Ref_Not_Provided(GitRefKind gitRefKind)
        {
            var mockGitHelper = new Mock<IGitHelper>();
            const string projectDir = "projectDir";
            _ = mockGitHelper.Setup(m => m.FindGitRoot(projectDir)).Returns("root");
            _ = mockGitHelper.Setup(m => m.Tag("root")).Returns("tag");
            _ = mockGitHelper.Setup(m => m.TagOrSha("root")).Returns("tagOrSha");
            _ = mockGitHelper.Setup(m => m.BranchName("root")).Returns("branch");
            _ = mockGitHelper.Setup(m => m.ShortCommitSha("root")).Returns("short");
            _ = mockGitHelper.Setup(m => m.CommitSha("root")).Returns("commit");

            var optionsProvider = new OptionsProvider(
                new Mock<IConfigFileService>().Object,
                mockGitHelper.Object,
                new Mock<IRemoveReplaceConfigLoader>().Object,
                SetupProjectDirExists().Object);

            (Options? options, IEnumerable<string>? errors) = optionsProvider.Provide(
                new ReadmeRewriterParseResult(
                    "http:www.example.com/repo.git",
                    "README.md",
                    null,
                    null,
                    gitRefKind,
                    projectDir,
                    "output/README.md",
                    false,
                    false,
                    false,
                    "",
                    ""));

            Assert.Multiple(() =>
            {
                Assert.That(errors, Is.Null);
                Assert.That(options!.RepoRef, Is.Not.Null);

                switch (gitRefKind)
                {
                    case GitRefKind.Tag:
                        mockGitHelper.Verify(m => m.Tag("root"));
                        Assert.That(options.RepoRef, Is.EqualTo("tag"));
                        break;
                    case GitRefKind.TagOrSha:
                        mockGitHelper.Verify(m => m.TagOrSha("root"));
                        Assert.That(options.RepoRef, Is.EqualTo("tagOrSha"));
                        break;
                    case GitRefKind.BranchName:
                        mockGitHelper.Verify(m => m.BranchName("root"));
                        Assert.That(options.RepoRef, Is.EqualTo("branch"));
                        break;
                    case GitRefKind.ShortCommitSha:
                        mockGitHelper.Verify(m => m.ShortCommitSha("root"));
                        Assert.That(options.RepoRef, Is.EqualTo("short"));
                        break;
                    case GitRefKind.CommitSha:
                        mockGitHelper.Verify(m => m.CommitSha("root"));
                        Assert.That(options.RepoRef, Is.EqualTo("commit"));
                        break;
                    default:
                        mockGitHelper.Verify(m => m.CommitSha("root"));
                        Assert.That(options.RepoRef, Is.EqualTo("commit"));
                        break;
                }
            });
        }

        private static IEnumerable<GitRefKind> AllGitRefKinds => Enum.GetValues<GitRefKind>();

        [Test]
        public void Should_Error_If_RepoRef_Cannot_Be_Obtained()
        {
            var mockGitHelper = new Mock<IGitHelper>();
            const string projectDir = "projectDir";
            _ = mockGitHelper.Setup(m => m.FindGitRoot(projectDir)).Returns("root");

            var optionsProvider = new OptionsProvider(
                new Mock<IConfigFileService>().Object,
                mockGitHelper.Object,
                new Mock<IRemoveReplaceConfigLoader>().Object,
                SetupProjectDirExists().Object);

            (Options? options, IEnumerable<string>? errors) = optionsProvider.Provide(
                new ReadmeRewriterParseResult(
                    "http:www.example.com/repo.git",
                    "README.md",
                    null,
                    null,
                    GitRefKind.CommitSha,
                    projectDir,
                    "output/README.md",
                    false,
                    false,
                    false,
                    "",
                    ""));

            Assert.Multiple(() =>
            {
                Assert.That(options, Is.Null);
                Assert.That(errors!.Single(), Is.EqualTo("Unable to obtain repo reference"));
            });
        }

        [Test]
        public void Should_Default_The_RepoRef_To_Master_When_Auto_And_CommitSha_Null()
        {
            var mockGitHelper = new Mock<IGitHelper>();
            const string projectDir = "projectDir";
            _ = mockGitHelper.Setup(m => m.FindGitRoot(projectDir)).Returns("root");

            var optionsProvider = new OptionsProvider(
                new Mock<IConfigFileService>().Object,
                mockGitHelper.Object,
                new Mock<IRemoveReplaceConfigLoader>().Object,
                SetupProjectDirExists().Object);

            (Options? options, IEnumerable<string>? errors) = optionsProvider.Provide(
                new ReadmeRewriterParseResult(
                    "http:www.example.com/repo.git",
                    "README.md",
                    null,
                    null,
                    GitRefKind.Auto,
                    projectDir,
                    "output/README.md",
                    false,
                    false,
                    false,
                    "",
                    ""));

            Assert.Multiple(() =>
            {
                Assert.That(errors, Is.Null);
                Assert.That(options!.RepoRef, Is.EqualTo("master"));
            });
        }

        [Test]
        public void Should_Error_If_GitHelper_Throws()
        {
            var mockGitHelper = new Mock<IGitHelper>();
            const string projectDir = "projectDir";
            _ = mockGitHelper.Setup(m => m.FindGitRoot(projectDir)).Returns("root");
            _ = mockGitHelper.Setup(m => m.CommitSha("root")).Throws(new Exception("git error"));

            var optionsProvider = new OptionsProvider(
                new Mock<IConfigFileService>().Object,
                mockGitHelper.Object,
                new Mock<IRemoveReplaceConfigLoader>().Object,
                SetupProjectDirExists().Object);

            (Options? options, IEnumerable<string>? errors) = optionsProvider.Provide(
                new ReadmeRewriterParseResult(
                    "http:www.example.com/repo.git",
                    "README.md",
                    null,
                    null,
                    GitRefKind.Auto,
                    projectDir,
                    "output/README.md",
                    false,
                    false,
                    false,
                    "",
                    ""));

            Assert.Multiple(() =>
            {
                Assert.That(errors!.Single(), Is.EqualTo("git error"));
                Assert.That(options, Is.Null);
            });
        }

        [Test]
        public void Should_Error_If_Cannot_Find_Git_Root()
        {
            var mockGitHelper = new Mock<IGitHelper>();
            string projectDir = "projectDir";

            var optionsProvider = new OptionsProvider(
                new Mock<IConfigFileService>().Object,
                mockGitHelper.Object,
                new Mock<IRemoveReplaceConfigLoader>().Object,
                SetupProjectDirExists().Object);
            (Options? options, IEnumerable<string>? errors) = optionsProvider.Provide(
                new ReadmeRewriterParseResult(
                    "http:www.example.com/repo.git",
                    "README.md",
                    null,
                    null,
                    GitRefKind.Auto,
                    projectDir,
                    "output/README.md",
                    false,
                    false,
                    false,
                    "",
                    ""));

            mockGitHelper.Verify(m => m.FindGitRoot(projectDir));
            Assert.Multiple(() =>
            {
                Assert.That(errors!.Single(), Is.EqualTo("no git root"));
                Assert.That(options, Is.Null);
            });
        }
    }
}
