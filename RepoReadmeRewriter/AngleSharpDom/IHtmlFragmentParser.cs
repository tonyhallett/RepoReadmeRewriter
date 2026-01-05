using AngleSharp.Dom;
using Markdig.Syntax;

namespace RepoReadmeRewriter.AngleSharpDom
{
    internal interface IHtmlFragmentParser
    {
        INode Parse(HtmlBlock htmlBlock);

        INode Parse(string fragment);
    }
}
