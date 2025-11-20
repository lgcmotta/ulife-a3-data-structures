using System.Globalization;

namespace A3.RouteSearchGraphs.Extensions;

internal static class StringFormattingExtensions
{
    extension(string? value)
    {
        internal string ToTitleCase()
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            var info = CultureInfo.CurrentCulture.TextInfo;

            return info.ToTitleCase(value);
        }

        internal string ToLeftSpacedString()
        {
            return !string.IsNullOrWhiteSpace(value) ? value.PadLeft(value.Length + 1) : string.Empty;
        }
    }
}