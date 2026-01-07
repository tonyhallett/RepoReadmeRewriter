using Moq;
using ReadmeRewriterCLI.RunnerOptions.Config;
using RepoReadmeRewriter.IOWrapper;

namespace CLITests
{
    internal sealed class ConfigFileService_Tests
    {
        private const string ProjectDir = "projectDir";
        private const string ProjectRelConfigPath = "projectRelConfigPath";
        private const string ConfigDir = "configDir";
        private const string ConfigRelFilePath = "configRelFilePath";
        private const string ProjectRelFilePath = "projectRelFilePath";
        private Mock<IIOHelper> _mockIOHelper;
        private ConfigFileService _configFileService;

        [SetUp]
        public void SetUp()
        {
            _mockIOHelper = new Mock<IIOHelper>();
            _configFileService = new(_mockIOHelper.Object);
        }

        [Test]
        public void Should_Resolve_Config_File_Path_Relative_To_Project_Directory()
        {
            string? resolved = GetConfigPath(true);

            Assert.That(resolved, Is.EqualTo(ProjectRelConfigPath));
        }

        [Test]
        public void Should_Have_Null_Config_File_Path_If_Does_Not_Exist()
        {
            string? resolved = GetConfigPath(false);

            Assert.That(resolved, Is.Null);
        }

        [Test]
        public void Should_Throw_If_Cannot_GetDiectoryName_For_Config_File()
        {
            string message = Assert.Throws<InvalidOperationException>(() => GetConfigPath(true, false)).Message;
            Assert.That(message, Is.EqualTo("Could not determine config directory from config file path."));
        }

        private string? GetConfigPath(bool exists, bool canGetDirectoryName = true)
        {
            string configFilePath = "config.json";

            SetupAbsoluteAndExists(ProjectDir, configFilePath, ProjectRelConfigPath, exists);
            _ = _mockIOHelper.Setup(m => m.GetDirectoryName(ProjectRelConfigPath)).Returns(canGetDirectoryName ? ConfigDir : null);

            return  _configFileService.GetConfigPath(ProjectDir, configFilePath);
        }

        [Test]
        public void Should_Resolve_Config_File_Relative_To_Config_Directory_First()
        {
            string? resolved = ResolveConfigFile(true, true);

            Assert.That(resolved, Is.EqualTo(ConfigRelFilePath));
        }

        [Test]
        public void Should_Resolve_Config_File_Relative_To_Project_Directory_Second()
        {
            string? resolved = ResolveConfigFile(false, true);

            Assert.That(resolved, Is.EqualTo(ProjectRelFilePath));
        }

        [Test]
        public void Should_Have_Null_Project_Relative_Config_File_If_Does_Not_Exist()
        {
            string? resolved = ResolveConfigFile(false, false);

            Assert.That(resolved, Is.Null);
        }

        [Test]
        public void Should_Throw_Exception_From_GetConfigFile_If_GetConfigPath_Not_Called()
        {
            string message = Assert.Throws<InvalidOperationException>(() => _configFileService.GetConfigFile("somepath")).Message;
            Assert.That(message, Is.EqualTo("Call GetConfigPath first"));
        }

        private string? ResolveConfigFile(bool configRelativeFileExists, bool projectRelativeFileExists)
        {
            string aConfigFilePath = "aconfigfilepath";

            SetupAbsoluteAndExists(ConfigDir, aConfigFilePath,  ConfigRelFilePath, configRelativeFileExists);
            SetupAbsoluteAndExists(ProjectDir, aConfigFilePath, ProjectRelFilePath, projectRelativeFileExists);

            _ = GetConfigPath(true);

            return _configFileService.GetConfigFile(aConfigFilePath);
        }

        private void SetupAbsoluteAndExists(string dir, string relOrAbsolute, string absolute, bool exists)
        {
            _ = _mockIOHelper.Setup(m => m.EnsureAbsolute(dir, relOrAbsolute)).Returns(absolute);
            _ = _mockIOHelper.Setup(m => m.FileExists(absolute)).Returns(exists);
        }
    }
}
