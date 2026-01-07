using Moq;
using ReadmeRewriterCLI.RunnerOptions.Config;
using ReadmeRewriterCLI.RunnerOptions.RemoveReplace;
using RepoReadmeRewriter.IOWrapper;
using RepoReadmeRewriter.RemoveReplace.Settings;

namespace CLITests
{
    internal sealed class RemoveReplaceConfigLoader_Tests
    {
        [Test]
        public void Should_LoadAndParseJson()
        {
            var mockDeserializer = new Mock<IRemoveReplaceConfigDeserializer>();
            mockDeserializer.Setup(m => m.LoadAndParseJson("configpath.json", It.IsAny<List<string>>())).Returns(new RemoveReplaceConfig()).Verifiable();
            var configLoader = new RemoveReplaceConfigLoader(
                new Mock<IIOHelper>().Object,
                mockDeserializer.Object,
                new Mock<IConfigFileService>().Object,
                new Mock<IRemoveReplaceWordsParser>().Object);

            RemoveReplaceSettings? removeReplaceSettings = configLoader.Load("configpath.json", out List<string>? _);

            mockDeserializer.VerifyAll();
        }

        [Test]
        public void Should_Return_Null_If_Load_Errors()
        {
            var mockDeserializer = new Mock<IRemoveReplaceConfigDeserializer>();
            _ = mockDeserializer.Setup(m => m.LoadAndParseJson("configpath.json", It.IsAny<List<string>>())).Callback<string, List<string>>((path, errors) => errors.AddRange("error1", "error2"));
            var configLoader = new RemoveReplaceConfigLoader(
                new Mock<IIOHelper>().Object,
                mockDeserializer.Object,
                new Mock<IConfigFileService>().Object,
                new Mock<IRemoveReplaceWordsParser>().Object);

            RemoveReplaceSettings? removeReplaceSettings = configLoader.Load("configpath.json", out List<string> errors);

            Assert.Multiple(() =>
            {
                Assert.That(removeReplaceSettings, Is.Null);
                Assert.That(errors, Is.EquivalentTo(["error1", "error2"]));
            });
        }

        
        [Test]
        public void Should_Have_Null_RemoveReplaceSettings_And_Error_When_Config_Without_Configuration()
        {
            (RemoveReplaceSettings? settings, List<string> errors) = ParseCommentIdentifiers(null);

            Assert.Multiple(() =>
            {
                Assert.That(settings, Is.Null);
                Assert.That(errors.Single(), Is.EqualTo("Config contained no configuration"));
            });
        }

        // ParseRemoveCommentIdentifiers
        [Test]
        public void Should_Error_When_RemoveCommentIdentifiers_Start_Missing()
        {
            (RemoveReplaceSettings? settings, List<string> errors) = ParseCommentIdentifiers(new RemoveCommentIdentifiersConfig
            {
                End = "end"
            });

            Assert.Multiple(() =>
            {
                Assert.That(settings, Is.Null);
                Assert.That(errors.Single(), Is.EqualTo("removeCommentIdentifiers.start is required."));
            });
        }

        [Test]
        public void Should_Error_When_RemoveCommentIdentifiers_Start_Is_Whitespace()
        {
            (RemoveReplaceSettings? settings, List<string> errors) = ParseCommentIdentifiers(new RemoveCommentIdentifiersConfig
            {
                Start = "   ",
            });

            Assert.Multiple(() =>
            {
                Assert.That(settings, Is.Null);
                Assert.That(errors.Single(), Is.EqualTo("removeCommentIdentifiers.start is required."));
            });
        }

        [Test]
        public void Should_Error_When_RemoveCommentIdentifiers_Start_And_End_Are_The_Same()
        {
            (RemoveReplaceSettings? settings, List<string> errors) = ParseCommentIdentifiers(new RemoveCommentIdentifiersConfig
            {
                Start = "same",
                End = "same"
            });

            Assert.Multiple(() =>
            {
                Assert.That(settings, Is.Null);
                Assert.That(errors.Single(), Is.EqualTo("removeCommentIdentifiers start and end cannot be the same."));
            });
        }

        [Test]
        public void Should_Error_When_RemoveCommentIdentifiers_End_Is_Whitespace()
        {
            (RemoveReplaceSettings? settings, List<string> errors) = ParseCommentIdentifiers(new RemoveCommentIdentifiersConfig
            {
                Start = "same",
                End = "  "
            });

            Assert.Multiple(() =>
            {
                Assert.That(settings, Is.Null);
                Assert.That(errors.Single(), Is.EqualTo("removeCommentIdentifiers.end is whitespace."));
            });
        }

        [Test]
        public void Should_Trim_RemoveCommentIdentifiers()
        {
            (RemoveReplaceSettings? settings, List<string> errors) = ParseCommentIdentifiers(new RemoveCommentIdentifiersConfig
            {
                Start = " start ",
                End = " end "
            });

            Assert.Multiple(() =>
            {
                Assert.That(errors, Is.Empty);

                Assert.That(settings!.RemoveCommentIdentifiers!.Start, Is.EqualTo("start"));
                Assert.That(settings!.RemoveCommentIdentifiers!.End, Is.EqualTo("end"));

                Assert.That(settings!.RemoveReplaceWords, Is.Empty);
                Assert.That(settings!.RemovalsOrReplacements, Is.Empty);
               
            });
        }

        [Test]
        public void Should_Not_Have_RemoveCommentIdentifiers_End_When_Missing()
        {
            (RemoveReplaceSettings? settings, List<string> errors) = ParseCommentIdentifiers(new RemoveCommentIdentifiersConfig
            {
                Start = " start ",
            });

            Assert.Multiple(() =>
            {
                Assert.That(errors, Is.Empty);

                Assert.That(settings!.RemoveCommentIdentifiers!.End, Is.Null);

            });
        }

        // ParseRemovalsOrReplacements
        [Test]
        public void Should_Error_When_RemovalOrReplacement_Start_Missing_Or_Whitespace()
        {
            (RemoveReplaceSettings? settings, List<string> errors) = ParseRemovalOrReplacements(
            [
                new() {
                    End = "end",
                    CommentOrRegex = "Comment"
                },
                new() {
                    Start = "  ",
                    End = "end",
                    CommentOrRegex = "Comment"
                }
            ]);

            Assert.Multiple(() =>
            {
                Assert.That(settings, Is.Null);
                Assert.That(errors, Is.EqualTo(new List<string> { "removalsOrReplacements[0].start is required.", "removalsOrReplacements[1].start is required." }));
            });
        }

        [Test]
        public void Should_Error_When_RemovalOrReplacement_Start_And_End_The_Same()
        {
            (RemoveReplaceSettings? settings, List<string> errors) = ParseRemovalOrReplacements(
            [
                new() {
                    Start = "same",
                    End = "same",
                    CommentOrRegex = "Comment"
                },
            ]);

            Assert.Multiple(() =>
            {
                Assert.That(settings, Is.Null);
                Assert.That(errors.Single(), Is.EqualTo("removalsOrReplacements[0] start and end cannot be the same."));
            });
        }

        [Test]
        public void Should_Error_When_RemovalOrReplacement_End_Is_Whitespace()
        {
            (RemoveReplaceSettings? settings, List<string> errors) = ParseRemovalOrReplacements(
            [
                new() {
                    Start = "same",
                    End = " ",
                    CommentOrRegex = "Comment"
                },
            ]);
            Assert.Multiple(() =>
            {
                Assert.That(settings, Is.Null);
                Assert.That(errors.Single(), Is.EqualTo("removalsOrReplacements[0].end is whitespace."));
            });
        }

        [Test]
        public void Should_Error_When_RemovalOrReplacement_Unsupported_CommentOrRegex()
        {
            (RemoveReplaceSettings? settings, List<string> errors) = ParseRemovalOrReplacements(
            [
                new() {
                    Start = "start",
                    End = "end",
                    CommentOrRegex = "unsupported"
                },
                
            ]);
            Assert.Multiple(() =>
            {
                Assert.That(settings, Is.Null);
                Assert.That(errors.Single, Is.EqualTo( "removalsOrReplacements[0].commentOrRegex unsupported 'unsupported'."));
            });
        }

        [Test]
        public void Should_Successfully_Parse_RemovalOrReplacements()
        {
            (RemoveReplaceSettings? settings, List<string> errors) = ParseRemovalOrReplacements(
            [
                new() {
                    Start = "start",
                    End = "end",
                    CommentOrRegex = "Comment",
                    ReplacementText = "replacement"
                },
                new() {
                    Start = "start2",
                    CommentOrRegex = "Regex",
                },
            ]);

            Assert.Multiple(() =>
            {
                Assert.That(errors, Is.Empty);
                List<RemovalOrReplacement> removalsOrReplacements  = settings!.RemovalsOrReplacements;
                Assert.That(removalsOrReplacements, Has.Count.EqualTo(2));
                Assert.That(removalsOrReplacements[0].Start, Is.EqualTo("start"));
                Assert.That(removalsOrReplacements[0].End, Is.EqualTo("end"));
                Assert.That(removalsOrReplacements[0].CommentOrRegex, Is.EqualTo(CommentOrRegex.Comment));
                Assert.That(removalsOrReplacements[0].ReplacementText, Is.EqualTo("replacement"));

                Assert.That(removalsOrReplacements[1].Start, Is.EqualTo("start2"));
                Assert.That(removalsOrReplacements[1].End, Is.Null);
                Assert.That(removalsOrReplacements[1].CommentOrRegex, Is.EqualTo(CommentOrRegex.Regex));
                Assert.That(removalsOrReplacements[1].ReplacementText, Is.Null);
            });
        }

        [Test]
        public void Should_Error_When_ReplacementFromFile_True_And_No_ReplacementText()
        {
            (RemoveReplaceSettings? settings, List<string> errors) = ParseRemovalOrReplacements(
            [
                new() {
                    Start = "start",
                    End = "end",
                    CommentOrRegex = "Comment",
                    ReplacementFromFile = true
                },
            ]);

            Assert.Multiple(() =>
            {
                Assert.That(settings, Is.Null);
                Assert.That(errors.Single(), Is.EqualTo("removalsOrReplacements[0].replacementText is required when replacementFromFile is true."));
            });
        }

        [Test]
        public void Should_Error_When_Fail_To_Load_ReplacementText_From_File()
        {
            string replacementFilePath = "path/to/replacement.txt";
            var mockConfigFileService = new Mock<IConfigFileService>();
            
            (RemoveReplaceSettings? settings, List<string> errors) = ParseRemovalOrReplacements(
            [
                new() {
                    Start = "start",
                    End = "end",
                    CommentOrRegex = "Comment",
                    ReplacementText = replacementFilePath,
                    ReplacementFromFile = true
                },
            ], mockConfigFileService.Object);

            mockConfigFileService.Verify(m => m.GetConfigFile(replacementFilePath));
            Assert.Multiple(() =>
            {
                Assert.That(settings, Is.Null);
                Assert.That(errors.Single(), Is.EqualTo("removalsOrReplacements[0].replacementText - failed to load from file 'path/to/replacement.txt'."));
            });
        }

        [Test]
        public void Should_Get_ReplacementText_From_File()
        {
            string replacementFilePath = "path/to/replacement.txt";
            var mockConfigFileService = new Mock<IConfigFileService>();
            _ = mockConfigFileService.Setup(m => m.GetConfigFile(replacementFilePath)).Returns("fullpath");
            var mockIOHelper = new Mock<IIOHelper>();
            _ = mockIOHelper.Setup(m => m.ReadAllText("fullpath")).Returns("fromfile");

            (RemoveReplaceSettings? settings, List<string> errors) = ParseRemovalOrReplacements(
            [
                new() {
                    Start = "start",
                    End = "end",
                    CommentOrRegex = "Comment",
                    ReplacementText = replacementFilePath,
                    ReplacementFromFile = true
                },
            ], mockConfigFileService.Object, mockIOHelper.Object);

            
            Assert.Multiple(() =>
            {
                Assert.That(errors, Is.Empty);
                Assert.That(settings!.RemovalsOrReplacements[0].ReplacementText, Is.EqualTo("fromfile"));
            });
        }

        //ParseRemoveReplaceWords

        [Test]
        public void Should_Have_Error_If_Failed_To_Load_RemoveReplaceWords_Files_And_Should_Not_Read_And_Parse()
        {
            var mockConfigFileService = new Mock<IConfigFileService>(MockBehavior.Strict);
            _ = mockConfigFileService.Setup(m => m.GetConfigFile("badPath")).Returns((string?)null);
            _ = mockConfigFileService.Setup(m => m.GetConfigFile("badPath2")).Returns((string?)null);
            _ = mockConfigFileService.Setup(m => m.GetConfigFile("goodPath")).Returns("abspath");

            (RemoveReplaceSettings? settings, List<string> errors) = ParseRemoveReplaceWords(
            [
                "badPath",
                "goodPath",
                "badPath2"
            ], mockConfigFileService.Object, new Mock<IIOHelper>(MockBehavior.Strict).Object);

            
            Assert.Multiple(() =>
            {
                Assert.That(settings, Is.Null);
                Assert.That(errors, Is.EqualTo(new List<string> { "Failed to load removeReplaceWords file 'badPath'.", "Failed to load removeReplaceWords file 'badPath2'." }));
            });
        }

        [Test]
        public void Should_Successfully_Read_And_Parse_RemoveReplaceWords_When_Files_Exist()
        {
            var mockConfigFileService = new Mock<IConfigFileService>(MockBehavior.Strict);
            _ = mockConfigFileService.Setup(m => m.GetConfigFile("goodPath")).Returns("abspath1");
            _ = mockConfigFileService.Setup(m => m.GetConfigFile("goodPath2")).Returns("abspath2");

            string[] lines1 = ["line1"];
            string[] lines2 = ["line2"];
            var mockIOHelper = new Mock<IIOHelper>();
            _ = mockIOHelper.Setup(m => m.ReadAllLines("abspath1")).Returns(lines1);
            _ = mockIOHelper.Setup(m => m.ReadAllLines("abspath2")).Returns(lines2);

            var mockRemoveReplaceWordsParser = new Mock<IRemoveReplaceWordsParser>();
            RemoveReplaceWord removeReplaceWord1 = new("word1", "replace1");
            RemoveReplaceWord removeReplaceWord2 = new("word2", "replace2");
            RemoveReplaceWord removeReplaceWord3 = new("word3", "replace3");
            RemoveReplaceWord removeReplaceWord4 = new("word4", "replace4");

            _ = mockRemoveReplaceWordsParser.Setup(m => m.Parse(lines1)).Returns([removeReplaceWord1, removeReplaceWord2]);
            _ = mockRemoveReplaceWordsParser.Setup(m => m.Parse(lines2)).Returns([removeReplaceWord3, removeReplaceWord4]);

            (RemoveReplaceSettings? settings, List<string> errors) = ParseRemoveReplaceWords(
            [
                "goodPath",
                "goodPath2"
            ], mockConfigFileService.Object,mockIOHelper.Object, mockRemoveReplaceWordsParser.Object);

            // should not ask for config if has errored or load at all

            Assert.Multiple(() =>
            {
                Assert.That(errors, Is.Empty);
                Assert.That(settings!.RemoveReplaceWords, Is.EqualTo(new List<RemoveReplaceWord> { removeReplaceWord1, removeReplaceWord2, removeReplaceWord3, removeReplaceWord4 }));
            });
        }

        private static (RemoveReplaceSettings? settings, List<string> errors) ParseRemoveReplaceWords(
           List<string> removeReplaceWordsFilePaths,
           IConfigFileService? configFileService = null,
           IIOHelper? ioHelper = null,
           IRemoveReplaceWordsParser? removeReplaceWordsParser = null)
            => Load(new RemoveReplaceConfig
            {
                RemoveReplaceWordsFilePaths = removeReplaceWordsFilePaths
            }, configFileService, ioHelper, removeReplaceWordsParser);

        private static (RemoveReplaceSettings? settings, List<string> errors) ParseRemovalOrReplacements(
            List<RemovalOrReplacementConfig> removalOrReplacements,
            IConfigFileService? configFileService = null,
            IIOHelper? ioHelper = null)
             => Load(new RemoveReplaceConfig
             {
                 RemovalsOrReplacements = removalOrReplacements
             }, configFileService, ioHelper);

        private static (RemoveReplaceSettings? settings, List<string> errors) ParseCommentIdentifiers(RemoveCommentIdentifiersConfig? removeCommentIdentifiersConfig)
            => Load(new RemoveReplaceConfig
            {
                RemoveCommentIdentifiers = removeCommentIdentifiersConfig
            });

        private static (RemoveReplaceSettings? settings, List<string> errors) Load(
            RemoveReplaceConfig removeReplaceConfig,
            IConfigFileService? configFileService = null,
            IIOHelper? ioHelper = null,
            IRemoveReplaceWordsParser? removeReplaceWordsParser = null)
        {
            var mockDeserializer = new Mock<IRemoveReplaceConfigDeserializer>();
            _ = mockDeserializer.Setup(m => m.LoadAndParseJson("configpath.json", It.IsAny<List<string>>())).Returns(removeReplaceConfig);
            var configLoader = new RemoveReplaceConfigLoader(
                ioHelper ?? new Mock<IIOHelper>().Object,
                mockDeserializer.Object,
                configFileService ?? new Mock<IConfigFileService>().Object,
                removeReplaceWordsParser ?? new Mock<IRemoveReplaceWordsParser>().Object);

            RemoveReplaceSettings? removeReplaceSettings = configLoader.Load("configpath.json", out List<string>? errors);
            return (removeReplaceSettings, errors);
        }
    }
}
