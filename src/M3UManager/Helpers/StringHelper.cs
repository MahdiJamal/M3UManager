using System;

namespace M3UManager.Helpers;

internal static class StringHelper
{
    internal static string[] Split(this string text, StringSplitOptions options, params string[] seprators)
        => text.Split(seprators, options);
    internal static string[] Split(this string text, params string[] seprators)
        => text.Split(seprators, StringSplitOptions.None);
}
