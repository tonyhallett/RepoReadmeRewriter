using System.Collections.Generic;

namespace RepoReadmeRewriter.ReadmeReplacement
{
    internal interface IFurtherReplacement
    {
        string ReplacementText { get; }

        void SetSourceReplacements(IEnumerable<SourceReplacement> sourceReplacements);
    }
}
