﻿using Aurora.EmailParser.Extensions;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Aurora.EmailParser
{
    public sealed class EmailParser
    {
        public EmailParseResult Parse(string content)
        {
            var root = LoadRootNode(content);
            var chain = new[] { new EmailPart(root.ChildNodes) }.AsEnumerable();

            if (TryFindQuoteNode(root.ChildNodes, out var quote))
            {
                chain = ExtractChain(quote.ParentNode).ToList();
            }

            return new EmailParseResult(chain);
        }

        private static HtmlNode LoadRootNode(string html)
        {
            var document = new HtmlDocument();

            document.LoadHtml(html);

            return document.DocumentNode.SelectSingleNode("//body") ?? document.DocumentNode;
        }

        private static IEnumerable<EmailPart> ExtractChain(HtmlNode node)
        {
            var previousNodes = new List<HtmlNode>();

            foreach (var item in node.ChildNodes)
            {
                var part = item;

                if (IsQuoteNode(item) || node.LastChild == item)
                {
                    yield return new EmailPart(previousNodes);

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
            var text = node.InnerText.Sanitize();

            return QUOTE_TAGS.Any(tag => Regex.Split(text, tag).Length > 1);
        }

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
