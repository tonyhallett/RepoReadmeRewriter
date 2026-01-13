using System;
using AngleSharp.Dom;
using Markdig;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace RepoReadmeRewriter.Processing
{
    internal sealed class RewritableMarkdownElementsProvider : IRewritableMarkdownElementsProvider
    {
        public RelevantMarkdownElements GetRelevantMarkdownElementsWithSourceLocation(string readme, bool excludeHtml)
        {
            MarkdownPipeline pipeline = new MarkdownPipelineBuilder()
            .UsePipeTables()
            .UsePreciseSourceLocation()
            .Build();
            MarkdownDocument document = Markdown.Parse(readme, pipeline);
            return new RelevantMarkdownElements(
                document.Descendants<LinkInline>(),
                excludeHtml ? [] : document.Descendants<HtmlBlock>(),
                excludeHtml ? [] : document.Descendants<HtmlInline>());
        }
    }
}
