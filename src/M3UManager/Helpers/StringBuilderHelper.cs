using System.Text;

namespace M3UManager.Helpers;

internal static class StringBuilderHelper
{
    internal static StringBuilder AppendIf(this StringBuilder stringBuilder, bool condition, string value)
    {
        if (condition)
            stringBuilder.Append(value);

        return stringBuilder;
    }
    internal static StringBuilder AppendLineIf(this StringBuilder stringBuilder, bool condition, string value)
    {
        if (condition)
            stringBuilder.AppendLine(value);

        return stringBuilder;
    }
}
