using System;

namespace M3UManager.Utilities;

internal static class StringUtility
{
    internal static string[] Split(this string text, StringSplitOptions options, params string[] seprators)
        => text.Split(seprators, options);
    internal static string[] Split(this string text, params string[] seprators)
        => text.Split(seprators, StringSplitOptions.None);
}
