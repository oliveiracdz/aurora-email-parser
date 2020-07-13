using System.Collections.Generic;
using System.Linq;

namespace Aurora.EmailParser
{
    public class EmailParseResult
    {
        public EmailParseResult(IEnumerable<string> chain)
        {
            QuoteChain = chain.ToArray();
        }

        public string[] QuoteChain { get; set; }
    }
}
