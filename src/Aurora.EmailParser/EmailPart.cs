namespace Aurora.EmailParser
{
    public sealed class EmailPart
    {
        internal EmailPart(string html, string text)
        {
            Html = html;
            Text = text;
        }

        public string Html { get; }
        public string Text { get; }
    }
}
