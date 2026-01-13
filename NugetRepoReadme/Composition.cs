using NugetRepoReadme.MSBuild;
using NugetRepoReadme.MSBuildHelpers;
using NugetRepoReadme.NugetValidation;
using NugetRepoReadme.RemoveReplace;
using Pure.DI;
using RepoReadmeRewriter.IOWrapper;
using RepoReadmeRewriter.Processing;
using RepoReadmeRewriter.Runner;
using static Pure.DI.Lifetime;

namespace NugetRepoReadme
{
    /// <summary>
    /// Source Generator composition root.
    /// </summary>
    internal partial class Composition
    {
        public static void Setup() => DI.Setup()
                .Bind<IIOHelper>().As(Singleton).To<IOHelper>()
                .Bind<IMessageProvider>().As(Singleton).To<MessageProvider>()
                .Bind<RepoReadmeRewriter.Messages.IMessageProvider>().As(Singleton).To<MessageProvider>()
                .Bind<IRemoveReplaceSettingsProvider>().As(Singleton).To<RemoveReplaceSettingsProvider>()
                .Bind<IMSBuildMetadataProvider>().As(Singleton).To<MSBuildMetadataProvider>()
                .Bind<IRemoveCommentsIdentifiersParser>().As(Singleton).To<RemoveCommentsIdentifiersParser>()
                .Bind<IRemovalOrReplacementProvider>().As(Singleton).To<RemovalOrReplacementProvider>()
                .Bind<IRemoveReplaceWordsProvider>().As(Singleton).To<RemoveReplaceWordsProvider>()
                .Bind<IReadmeRewriterRunner>().As(Singleton).To<ReadmeRewriterRunner>()
                .Bind<IImageDomainValidator>().As(Singleton).To<NuGetImageDomainValidator>()
                .Bind<INuGetTrustedImageDomains>().As(Singleton).To<NuGetTrustedImageDomains>()
                .Bind<INuGetGitHubBadgeValidator>().As(Singleton).To<NuGetGitHubBadgeValidator>()
                .Builder<ReadmeRewriterTask>("Initialize");
    }
}
