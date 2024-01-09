using System;

namespace M3UManager.Utilities
{
    internal static class StringUtility
    {
        internal static string[] Split(this string text, string seprator, StringSplitOptions options = StringSplitOptions.None)
            => text.Split(new string[] { seprator }, options);
        internal static string[] Split(this string text, params string[] seprators)
            => text.Split(seprators, StringSplitOptions.None);
        internal static string[] Split(this string text, StringSplitOptions options, params string[] seprators)
            => text.Split(seprators, options);
    }
}
