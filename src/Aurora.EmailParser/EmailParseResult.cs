using System.Collections.Generic;
using System.Linq;

namespace Aurora.EmailParser
{
    public sealed class EmailParseResult
    {
        internal EmailParseResult(IEnumerable<EmailPart> chain)
        {
            var message = chain.ElementAt(0);

            MessageHtml = message.Html;
            MessageText = message.Text;
            Quotes = chain.Skip(1).ToArray();
        }

        public string MessageText { get; }
        public string MessageHtml { get; }
        public EmailPart[] Quotes { get; }
    }
}
