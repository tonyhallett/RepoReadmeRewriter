using Markdig.Syntax;

namespace RepoReadmeRewriter.MarkdigHelpers
{
    internal static class BlockExtensions
    {
        public static int NewLineLength(this Block block) => (int)block.NewLine & 0x03;
    }
}
