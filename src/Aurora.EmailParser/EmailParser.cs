using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Aurora.EmailParser
{
    public class EmailParser
    {
        /* Refs: 
         * https://www.mailgun.com/blog/open-sourcing-our-email-signature-parsing-library/
         * https://sigparser.com/
         */

        public static EmailParsed Parse(string path)
        {
            var document = new HtmlDocument();
            document.Load(path);

            var root = document.DocumentNode.SelectSingleNode("//body") ?? document.DocumentNode;
            var chain = new[] { ConcatNodes(root.ChildNodes) }.AsEnumerable();

            if (TryFindQuoteNode(root.ChildNodes, out var quote))
            {
                chain = ExtractChain(quote.ParentNode).ToList();
            }

            return new EmailParsed(chain);
        }

        private static IEnumerable<string> ExtractChain(HtmlNode node)
        {
            var previousNodes = new List<HtmlNode>();

            foreach (var item in node.ChildNodes)
            {
                var part = item;

                if (IsQuoteNode(item) || node.LastChild == item)
                {
                    yield return ConcatNodes(previousNodes);

                    previousNodes.Clear();
                }

                previousNodes.Add(part);
            }
        }

        private static bool TryFindQuoteNode(IEnumerable<HtmlNode> nodes, out HtmlNode quote)
        {
            foreach (var node in nodes)
            {
                if (IsQuoteNode(node))
                {
                    quote = node;
                    return true;
                }

                if (TryFindQuoteNode(node.ChildNodes, out quote))
                {
                    return true;
                }
            }

            quote = null;
            return false;
        }

        private static bool IsQuoteNode(HtmlNode node)
        {
            var text = SanitizeText(node.InnerText);

            return QUOTE_TAGS.Any(tag => Regex.Split(text, tag).Length > 1);
        }

        private static string ConcatNodes(IEnumerable<HtmlNode> nodes) => string.Join(string.Empty, nodes.Select(p => p.OuterHtml));
        private static string SanitizeText(string text) => text.Replace("&nbsp;", string.Empty).Trim();

        private static readonly string[] QUOTE_TAGS = new[] {
            "^----------",
            @"^Em \w{3}., \d+ de \w{3}. de \d{4} às ",
            @"^(\w*)-feira, \d+ de (\w*) de \d{4} ",
            "^________________________________",
            "^De:",
            "^From:"
        };
    }
}
