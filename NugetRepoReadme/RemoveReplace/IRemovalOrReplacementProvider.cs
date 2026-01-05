using RepoReadmeRewriter.RemoveReplace.Settings;

namespace NugetRepoReadme.RemoveReplace
{
    internal interface IRemovalOrReplacementProvider
    {
        RemovalOrReplacement? Provide(MetadataItem metadataItem, IAddError addError);
    }
}
