using System.Collections.Generic;
using System.Linq;

namespace Aurora.EmailParser
{
    public class EmailParsed
    {
        public EmailParsed(IEnumerable<string> chain)
        {
            QuoteChain = chain.ToArray();
        }

        public string[] QuoteChain { get; set; }
    }
}
