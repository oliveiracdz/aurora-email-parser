using Aurora.EmailParser.Extensions;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;

namespace Aurora.EmailParser
{
    public sealed class EmailPart
    {
        internal EmailPart(IEnumerable<HtmlNode> nodes)
        {
            Html = string.Join(string.Empty, nodes.Select(p => p.OuterHtml));
            Text = string.Join(string.Empty, nodes.Select(p => p.InnerText.Sanitize()));
        }

        public string Html { get; }
        public string Text { get; }
    }
}
