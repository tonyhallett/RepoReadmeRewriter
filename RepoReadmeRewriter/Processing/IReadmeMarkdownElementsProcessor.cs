using RepoReadmeRewriter.Repo;

namespace RepoReadmeRewriter.Processing
{
    internal interface IReadmeMarkdownElementsProcessor
    {
        IMarkdownElementsProcessResult Process(
            RelevantMarkdownElements relevantMarkdownElements,
            RepoPaths? repoPaths,
            RewriteTagsOptions rewriteTagsOptions,
            IReadmeRelativeFileExists readmeRelativeFileExists);
    }
}
