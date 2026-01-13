using System.Collections.Generic;

namespace RepoReadmeRewriter.ReadmeReplacement
{
    internal interface IReplacementResult
    {
        string? Result { get; }

        IEnumerable<IFurtherReplacement> FurtherReplacements { get; }

        void ApplyFurtherReplacements();
    }
}
