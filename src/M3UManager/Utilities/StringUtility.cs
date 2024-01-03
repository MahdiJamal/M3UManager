using System;

namespace M3UManager.Utilities
{
    internal static class StringUtility
    {
        internal static string[] Split(this string text, string seprator, StringSplitOptions options = StringSplitOptions.None)
            => text.Split(new string[] { seprator }, options);
    }
}
