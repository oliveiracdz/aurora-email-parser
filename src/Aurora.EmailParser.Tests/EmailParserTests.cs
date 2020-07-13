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
        [InlineData("Email-Simples", 2)]
        [InlineData("Email-Complexo", 1)]
        public void Quando_HouverQuote_Deve_ExtrairPartes(string emailFile, int quotesLength)
        {
            var context = Setup(emailFile);

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
        public void Quando_NaoHouverQuote_Deve_RetornarOProprioHtml()
        {
            var context = Setup("EmailWithNoQuotes");

            context.ParsedHtml.ShouldBe(context.OriginalHtml);
            context.Quotes.ShouldBeEmpty();
        }

        private static SetupContext Setup(string filename)
        {
            var path = $@"{Directory.GetCurrentDirectory()}/Resources/{filename}.html";
            var parseResult = EmailParser.Parse(path);
            var original = new HtmlDocument(); original.Load(path);
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