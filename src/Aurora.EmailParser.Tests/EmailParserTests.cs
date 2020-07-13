using HtmlAgilityPack;
using Shouldly;
using System.IO;
using System.Linq;
using Xunit;

namespace Aurora.EmailParser.Tests
{
    public class EmailParserTests
    {
        [Theory]
        [InlineData("Simple", 2)]
        //[InlineData("WithNestedQuotes", 5, Skip = "Skipped")]
        public void ShouldFindQuotes(string email, int quotesLength)
        {
            var context = SetupEmail(email);

            context.ParsedHtml.ShouldNotBe(context.OriginalHtml);
            context.Quotes.Count().ShouldBe(quotesLength);

            for (int i = context.Quotes.Count() - 1; i > 0;)
            {
                var part = context.Quotes.ElementAt(i);
                var prev = context.Quotes.ElementAt(--i);

                part.Html.ShouldNotContain(prev.Html);
            }
        }

        [Fact]
        public void ShouldHandleEmptyQuotes()
        {
            var context = SetupEmail("WithNoQuotes");

            context.ParsedHtml.ShouldBe(context.OriginalHtml);
            context.Quotes.ShouldBeEmpty();
        }

        private static SetupContext SetupEmail(string filename)
        {
            var path = $@"{Directory.GetCurrentDirectory()}/Emails/{filename}.html";
            var original = new HtmlDocument(); original.Load(path);

            var parser = new EmailParser();
            var parseResult = parser.Parse(original.DocumentNode.OuterHtml);
            var result = new HtmlDocument(); result.LoadHtml(parseResult.MessageHtml);

            return new SetupContext
            {
                OriginalHtml = original.DocumentNode.OuterHtml,
                ParsedHtml = result.DocumentNode.OuterHtml,
                Quotes = parseResult.Quotes
            };
        }

        class SetupContext
        {
            public string OriginalHtml { get; set; }
            public string ParsedHtml { get; set; }
            public EmailPart[] Quotes { get; set; }
        }
    }
}