namespace Aurora.EmailParser.Extensions
{
    internal static class StringExtensions
    {
        internal static string Sanitize(this string text) => text.Replace("&nbsp;", string.Empty).Trim();
    }
}
