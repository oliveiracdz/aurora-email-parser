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
        [InlineData("Email-Simples", 3)]
        [InlineData("Email-Complexo", 2)]
        public void Quando_HouverQuote_Deve_ExtrairPartes(string file, int chainParts)
        {
            var context = Setup(file);

            context.Original.ShouldNotBe(context.Result);
            context.Chain.Count().ShouldBe(chainParts);

            for (int i = context.Chain.Count() - 1; i > 0;)
            {
                var part = context.Chain.ElementAt(i);
                var prev = context.Chain.ElementAt(--i);

                part.ShouldNotContain(prev);
            }
        }

        [Fact]
        public void Quando_NaoHouverQuote_Deve_RetornarOProprioHtml()
        {
            var context = Setup("EmailWithNoQuotes");

            context.Original.ShouldBe(context.Result);
            context.Chain.Count().ShouldBe(1);
        }

        private static SetupContext Setup(string filename)
        {
            var path = $@"{Directory.GetCurrentDirectory()}/Resources/{filename}.html";
            var resultHtml = EmailParser.Parse(path);
            var original = new HtmlDocument(); original.Load(path);
            var result = new HtmlDocument(); result.LoadHtml(resultHtml.QuoteChain[0]);

            return new SetupContext
            {
                Original = original.DocumentNode.OuterHtml,
                Result = result.DocumentNode.OuterHtml,
                Chain = resultHtml.QuoteChain
            };
        }

        class SetupContext
        {
            public string Original { get; set; }
            public string Result { get; set; }
            public string[] Chain { get; set; }
        }
    }
}